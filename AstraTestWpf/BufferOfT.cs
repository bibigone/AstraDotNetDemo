using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AstraTestWpf
{
    /// <summary>
    /// Base class for helper buffers that allow to get and show depth and color frames.
    /// </summary>
    /// <typeparam name="T">Type of frame element.</typeparam>
    /// <remarks>
    /// This class works in two steps:
    /// 1. Data is copied from <c>Astra.ImageFrame</c> to internal buffer (array of bytes).
    ///    This operation is performed from background thread as a rule during processing of received frame from Astra SDK.
    /// 2. After that in UI thread <c>WritableBitmap</c> is updated based on data in internal intermediate buffer.
    ///    This image is available as property <c>ImageSource</c> and can be used in UI.
    /// </remarks>
    internal abstract class Buffer<T> where T : struct
    {
        /// <summary>
        /// Constructs object. Call from UI thread because during construction <c>ImageSource</c> is being created.
        /// </summary>
        /// <param name="dispatcher">Dispatcher of owner thread. As a rule, UI thread.</param>
        /// <param name="width">Width of frame in pixels.</param>
        /// <param name="height">Height of frame in pixels.</param>
        /// <param name="pixelFormat">Pixel format of frame.</param>
        /// <param name="bytesPerPixel">Byte count per one pixel.</param>
        protected Buffer(Dispatcher dispatcher, int width, int height, Astra.PixelFormat pixelFormat, int bytesPerPixel)
        {
            if (dispatcher.Thread != Thread.CurrentThread)
            {
                throw new InvalidOperationException(
                    "Call this constructor from UI thread please, because it creates ImageSource object for UI");
            }

            this.dispatcher = dispatcher;
            this.width = width;
            this.height = height;
            this.pixelFormat = pixelFormat;

            innerBuffer = new byte[width * height * bytesPerPixel];
            writeableBitmap = new WriteableBitmap(width, height, 96, 96, PixelFormats.Bgr24, null);
        }

        /// <summary>
        /// Image with visualized frame. You can use this property in WPF controls/windows.
        /// </summary>
        public BitmapSource ImageSource => writeableBitmap;

        /// <summary>
        /// Updates <c>ImageSource</c> based on <c>Astra.ImageFrame</c>, received from Astra sensor.
        /// </summary>
        /// <param name="frame">Frame, received from Astra sensor. Can be <c>null</c>.</param>
        /// <returns><c>true</c> - updated, <c>false</c> - not updated (frame is not compatible, or old frame).</returns>
        public bool Update(Astra.ImageFrame<T> frame)
        {
            // Is compatible?
            if (frame == null
                || frame.Width != width || frame.Height != height
                || frame.PixelFormat != pixelFormat)
            {
                return false;
            }

            // Is new frame?
            var frameIndex = Interlocked.Exchange(ref lastFrameIndex, frame.FrameIndex);
            if (frameIndex == frame.FrameIndex)
            {
                return false;
            }

            // 1st step: new frame => update
            FillInnerBuffer(frame.DataPtr, frame.ByteLength);

            // 2nd step: we can update WritableBitmap only from its owner thread (as a rule, UI thread)
            dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(FillWritableBitmap));

            // Updated
            return true;
        }

        private unsafe void FillInnerBuffer(IntPtr srcPtr, long byteLength)
        {
            // This method can be called from some background thread,
            // thus use synchronization
            lock (innerBuffer)
            {
                fixed (void* dstPtr = innerBuffer)
                {
                    Buffer.MemoryCopy((void*)srcPtr, dstPtr, innerBuffer.Length, byteLength);
                }
            }
        }

        private void FillWritableBitmap()
        {
            writeableBitmap.Lock();
            try
            {
                var backBuffer = writeableBitmap.BackBuffer;
                var backBufferStride = writeableBitmap.BackBufferStride;

                // This method works in UI thread, and uses innerBuffer
                // that is filled in Update() method from some background thread
                lock (innerBuffer)
                {
                    // We use parallelism here to speed up
                    Parallel.For(0, height, y => FillWritableBitmapLine(y, backBuffer, backBufferStride));
                }

                // Inform UI infrastructure that we have updated content of image
                writeableBitmap.AddDirtyRect(new System.Windows.Int32Rect(0, 0, width, height));
            }
            finally
            {
                writeableBitmap.Unlock();
            }
        }

        protected abstract void FillWritableBitmapLine(int y, IntPtr backBuffer, int backBufferStride);

        protected readonly Dispatcher dispatcher;
        protected readonly int width;
        protected readonly int height;
        protected readonly Astra.PixelFormat pixelFormat;
        protected readonly byte[] innerBuffer;
        protected readonly WriteableBitmap writeableBitmap;
        protected long lastFrameIndex = long.MinValue;
    }
}
