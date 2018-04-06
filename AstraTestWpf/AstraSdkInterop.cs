using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace AstraTestWpf
{
    internal static class AstraSdkInterop
    {
        /// <summary>
        /// For some reason there is no property/methods in .Net wrapper to control depth registration mode.
        /// This why we have to implement this functionality by our own via direct call to <c>astra_depthstream_set_registration</c>
        /// function in <c>astra.DLL</c>.
        /// </summary>
        /// <remarks>
        /// Registration mode allows to align depth with color.
        /// That is, when registration mode is ON then each pixel on depth map corresponds to appropriate pixel on color image from sensor.
        /// </remarks>
        /// <param name="depthStream">Depth stream.</param>
        /// <param name="enabled"><c>true</c> to turn registration mode ON, <c>false</c> - to turn registration mode OFF.</param>
        /// <returns><c>true</c> - success, <c>false</c> - failed.</returns>
        public static bool SetDepthRegistration(this Astra.DepthStream depthStream, bool enabled)
        {
            var streamHandle = depthStream.GetHandle();
            if (streamHandle == IntPtr.Zero)
                return false;
            var res = astra_depthstream_set_registration(streamHandle, enabled);
            return res == 0;
        }

        /// <summary>
        /// We have to get handle of stream to perform calls to API of <c>astra.DLL</c>.
        /// And we can do it using Reflection (to access protected property <c>Handle</c> of <c>DataStream</c> class).
        /// </summary>
        /// <param name="stream">Stream object. Can be <c>null</c>.</param>
        /// <returns>Astra SDK handle of stream object. Can be <c>IntPtr.Zero</c>.</returns>
        private static IntPtr GetHandle(this Astra.ImageStream stream)
        {
            if (stream == null)
                return IntPtr.Zero;
            var streamHandleObj = handleOfImageStreamPropertyInfo.GetValue(stream);
            if (streamHandleObj == null || !(streamHandleObj is IntPtr))
                return IntPtr.Zero;
            return (IntPtr)streamHandleObj;
        }

        private static readonly PropertyInfo handleOfImageStreamPropertyInfo
            = typeof(Astra.DataStream).GetProperty("Handle", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);

        [DllImport("astra")]
        private static extern int astra_depthstream_set_registration(IntPtr streamHandle, bool enabled);
    }
}
