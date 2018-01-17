using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AstraTestWpf
{
    /// <summary>
    /// View model for <c>SensorWindow</c> that encapsulates all logic around working with depth sensor.
    /// </summary>
    internal sealed class SensorViewModel : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Creates view model for <c>SensorWindow</c> WPF window.
        /// </summary>
        /// <param name="settings">Settings: resolution and FPS of depth and color streams.</param>
        /// <param name="dispatcher">Dispatcher of WPF window. That is dispatcher of UI thread.</param>
        /// <param name="streamSet">Stream set for sensor. Should be created using <c></c>.</param>
        /// <param name="withColor">Show color stream along with depth stream?</param>
        /// <param name="withBodyTracking">Perform body tracking and visualize skeleton?</param>
        public SensorViewModel(
            Properties.Settings settings,
            Dispatcher dispatcher,
            Astra.StreamSet streamSet,
            bool withColor,
            bool withBodyTracking)
        {
            this.dispatcher = dispatcher;
            this.streamSet = streamSet;

            // Creation of stream reader
            streamReader = streamSet.CreateReader();

            try
            {
                // Get depth steam from reader
                depthStream = streamReader.GetStream<Astra.DepthStream>();

                // Chose appropriate mode
                var depthMode = SetDepthMode(settings.DepthFps, settings.DepthWidth, settings.DepthHeight);
                DepthMode = FormatMode(depthMode);

                // Depth field of view and Chip ID
                DepthFieldOfView = FormatFieldOfView(depthStream);
                ChipId = depthStream.ChipId.ToString();

                // To visualize depth map on UI
                depthBuffer = new DepthBuffer(dispatcher, depthMode.Width, depthMode.Height);

                // Try to initialize color stream (optional)
                if (withColor)
                {
                    colorStream = streamReader.GetStream<Astra.ColorStream>();
                    var colorMode = TrySetColorMode(settings.ColorFps, settings.ColorWidth, settings.ColorHeight);
                    if (colorMode == null)
                    {
                        colorStream = null;
                    }
                    else
                    {
                        ColorMode = FormatMode(colorMode);
                        ColorFieldOfView = FormatFieldOfView(colorStream);
                        colorBuffer = new ColorBuffer(dispatcher, colorMode.Width, colorMode.Height);
                    }
                }

                // Body tracking (optional)
                if (withBodyTracking)
                {
                    bodyStream = streamReader.GetStream<Astra.BodyStream>();
                    bodyVisualizer = new BodyVisualizer(dispatcher, depthMode.Width, depthMode.Height);
                }

                // Start streaming
                depthStream.Start();
                colorStream?.Start();
                bodyStream?.Start();

                // Run background thread to process frames
                running = true;
                backgroundProcessingThread = new Thread(BackgroundProcessingLoop) { IsBackground = true };
                backgroundProcessingThread.Start();
            }
            catch
            {
                streamReader.Dispose();
                streamSet.Dispose();
                throw;
            }
        }

        private static double ToDegrees(double radians) => Math.Round(radians * 180.0 / Math.PI, 1);

        private static string FormatMode(Astra.ImageMode mode) => $"{mode.Width}x{mode.Height} at {mode.FramesPerSecond} FPS";

        private static string FormatFieldOfView(Astra.ImageStream stream)
            => $"{ToDegrees(stream.HorizontalFieldOfView)}° x {ToDegrees(stream.VerticalFieldOfView)}°";

        private Astra.ImageMode SetDepthMode(int fps, int width, int height)
        {
            // Choose mode based on FPS and resolution.
            // Note! AvailableModes returns not supported modes among supported.
            // For example: 60 FPS is not supported by Astra PRO but it is returned by AvailableModes.
            // Be careful!
            var mode = depthStream.AvailableModes
                .FirstOrDefault(m => m.PixelFormat == DepthBuffer.PixelFormat
                    && m.FramesPerSecond == fps
                    && m.Width == width
                    && m.Height == height);
            if (mode == null)
                throw new NotSupportedException($"Depth stream doesn't support required mode ({width}x{height} at {fps} FPS)");
            depthStream.SetMode(mode);
            depthStream.IsMirroring = false;        // Turn off mirroring by default
            return mode;
        }

        private Astra.ImageMode TrySetColorMode(int fps, int width, int height)
        {
            try
            {
                var mode = colorStream.AvailableModes
                    .FirstOrDefault(m => m.PixelFormat == ColorBuffer.PixelFormat
                        && m.FramesPerSecond == fps
                        && m.Width == width
                        && m.Height == height);
                if (mode == null)
                    return null;
                colorStream.SetMode(mode);
                colorStream.IsMirroring = false;        // Turn off mirroring by default
                return mode;
            }
            catch (Astra.AstraException exc)
            {
                Trace.TraceWarning("Cannot initialize color stream: " + exc.Message);
                return null;
            }
        }

        /// <summary>String with information about current depth mode.</summary>
        public string DepthMode { get; }

        /// <summary>String with information about field of view of depth sensor.</summary>
        public string DepthFieldOfView { get; }

        /// <summary>Visualized depth map received from sensor.</summary>
        public BitmapSource DepthImageSource => depthBuffer?.ImageSource;

        /// <summary>Is depth map mirroring? Can be changed on the fly.</summary>
        public bool IsDepthMirroring
        {
            get => depthStream.IsMirroring;

            set
            {
                if (value != IsDepthMirroring)
                {
                    depthStream.IsMirroring = value;
                    RaisePropertyChanged(nameof(IsDepthMirroring));
                }
            }
        }

        /// <summary>Some ID of sensor. But for some reason Astra SDK always returns <c>1</c>.</summary>
        public string ChipId { get; }

        /// <summary>String with information about current frame rate.</summary>
        public string FramesPerSecond
        {
            get
            {
                var fps = frameRateCalculator.FramesPerSecond;
                return fps > float.Epsilon ? fps.ToString("0.0") : string.Empty;
            }
        }

        /// <summary>String with information about current color mode.</summary>
        public string ColorMode { get; }

        /// <summary>String with information about field of view of color camera.</summary>
        public string ColorFieldOfView { get; }

        /// <summary>Color frame received from sensor.</summary>
        /// <remarks>Can be <c>null</c> here because color stream is optional.</remarks>
        public BitmapSource ColorImageSource => colorBuffer?.ImageSource;

        /// <summary>Is color image mirroring? Can be changed on the fly.</summary>
        public bool IsColorMirroring
        {
            get => colorStream?.IsMirroring ?? false;

            set
            {
                if (colorStream != null && value != IsColorMirroring)
                {
                    colorStream.IsMirroring = value;
                    RaisePropertyChanged(nameof(IsColorMirroring));
                }
            }
        }

        /// <summary>Visualization of body tracking - skeletons. Shown on UI as overlay over depth map image.</summary>
        /// <remarks>Can be <c>null</c> here because body tracking is optional.</remarks>
        public ImageSource BodyImageSource => bodyVisualizer?.ImageSource;

        #region IDisposable

        /// <summary>
        /// Stops streaming, destroys all objects.
        /// </summary>
        public void Dispose()
        {
            running = false;
            backgroundProcessingThread.Join();
            StopStreamNoThrow(bodyStream, nameof(bodyStream));
            StopStreamNoThrow(colorStream, nameof(colorStream));
            StopStreamNoThrow(depthStream, nameof(depthStream));
            SafeDispose(streamReader, nameof(streamReader));
            SafeDispose(streamSet, nameof(streamSet));
            frameRateCalculator.Reset();
        }

        private static void StopStreamNoThrow(Astra.DataStream stream, string streamName)
        {
            try
            {
                stream?.Stop();
            }
            catch (Astra.AstraException exc)
            {
                Trace.TraceWarning($"Cannot stop stream {streamName}: {exc.Message}");
            }
        }

        private static void SafeDispose(IDisposable astraObject, string objName)
        {
            try
            {
                astraObject?.Dispose();
            }
            catch (Astra.AstraException exc)
            {
                Trace.TraceWarning($"Cannot dispose object {objName}: {exc.Message}");
            }
        }

        #endregion

        #region INotifyPropertyChanged

        /// <summary>
        /// For UI binding.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            if (Thread.CurrentThread != dispatcher.Thread)
            {
                // If this method is called from non-UI thread, then redirect it to UI thread
                dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() => RaisePropertyChanged(propertyName)));
                return;
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Processing of frames

        // Works in background thread
        private void BackgroundProcessingLoop()
        {
            while (running)
            {
                try
                {
                    if (streamReader.TryOpenFrame(10, out var frame))
                    {
                        using (frame)
                        {
                            HandleFrame(frame);
                        }
                    }
                }
                catch (Astra.AstraException exc)
                {
                    // As a rule, exception here means that sensor was unplugged.
                    InformUserAboutError(exc);
                    running = false;
                    break;
                }

                Thread.Sleep(5);            // To avoid too high load of one CPU core
            }
        }

        // Handling of new frame
        private void HandleFrame(Astra.ReaderFrame frame)
        {
            HandleDepthFrame(frame);
            if (colorBuffer != null)
                HandleColorFrame(frame);
            if (bodyVisualizer != null)
                HandleBodyFrame(frame);
        }

        private void HandleDepthFrame(Astra.ReaderFrame frame)
        {
            var depthFrame = frame.GetFrame<Astra.DepthFrame>();
            if (depthBuffer.Update(depthFrame))
                if (frameRateCalculator.RegisterFrame())
                    RaisePropertyChanged(nameof(FramesPerSecond));
        }

        private void HandleColorFrame(Astra.ReaderFrame frame)
        {
            var colorFrame = frame.GetFrame<Astra.ColorFrame>();
            colorBuffer.Update(colorFrame);
        }

        private void HandleBodyFrame(Astra.ReaderFrame frame)
        {
            var bodyFrame = frame.GetFrame<Astra.BodyFrame>();
            bodyVisualizer.Update(bodyFrame);
        }

        private void InformUserAboutError(Astra.AstraException exc)
        {
            var message = $"Depth sensor error: {exc.Message}. Looks like that depth sensor was unplugged from USB port.";
            dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(() => ShowErrorBox(message)));
        }

        private static void ShowErrorBox(string message)
        {
            System.Windows.MessageBox.Show(message, "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }

        #endregion

        private readonly Dispatcher dispatcher;
        private readonly Astra.StreamSet streamSet;
        private readonly Astra.StreamReader streamReader;
        private readonly Astra.DepthStream depthStream;
        private readonly Astra.ImageStream colorStream;
        private readonly Astra.BodyStream bodyStream;
        private readonly DepthBuffer depthBuffer;
        private readonly ColorBuffer colorBuffer;
        private readonly BodyVisualizer bodyVisualizer;
        private readonly FrameRateCalculator frameRateCalculator = new FrameRateCalculator(smoothCoeff: 0.75f);
        private readonly Thread backgroundProcessingThread;
        private volatile bool running;
    }
}
