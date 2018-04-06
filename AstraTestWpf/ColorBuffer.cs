using System;
using System.Windows.Threading;

namespace AstraTestWpf
{
    /// <summary>
    /// Helper buffer for color data. For details see description of base class.
    /// </summary>
    internal sealed class ColorBuffer : Buffer<byte>
    {
        public static readonly Astra.PixelFormat PixelFormat = Astra.PixelFormat.RGB888;
        public static readonly int BytesPerPixel = 3;

        /// <summary>
        /// Constructs object. Call from UI thread because during construction <c>ImageSource</c> is being created.
        /// </summary>
        /// <param name="dispatcher">Dispatcher of owner thread. As a rule, UI thread.</param>
        /// <param name="width">Width of color frame in pixels.</param>
        /// <param name="height">Height of color frame in pixels.</param>
        public ColorBuffer(Dispatcher dispatcher, int width, int height)
            : base(dispatcher, width, height, PixelFormat, BytesPerPixel)
        { }

        protected override unsafe void FillWritableBitmapLine(int y, IntPtr backBuffer, int backBufferStride)
        {
            byte* dstPtr = (byte*)backBuffer + y * backBufferStride;
            fixed (byte* innerBufferPtr = innerBuffer)
            {
                byte* srcPtr = innerBufferPtr + y * width * 3;
                for (var x = 0; x < width; x++)
                {
                    // reverse order to convert from RGB to BGR
                    *(dstPtr++) = *(srcPtr + 2);
                    *(dstPtr++) = *(srcPtr + 1);
                    *(dstPtr++) = *srcPtr;
                    srcPtr += 3;
                }
            }
        }
    }
}
