using System;
using System.Windows.Threading;

namespace AstraTestWpf
{
    /// <summary>
    /// Helper buffer for depth data. For details see description of base class.
    /// </summary>
    internal sealed class DepthBuffer : Buffer<short>
    {
        public static readonly Astra.PixelFormat PixelFormat = Astra.PixelFormat.DepthMM;
        public static readonly int BytesPerPixel = sizeof(short);

        /// <summary>
        /// Constructs object. Call from UI thread because during construction <c>ImageSource</c> is being created.
        /// </summary>
        /// <param name="dispatcher">Dispatcher of owner thread. As a rule, UI thread.</param>
        /// <param name="width">Width of depth map in pixels.</param>
        /// <param name="height">Height of depth map in pixels.</param>
        public DepthBuffer(Dispatcher dispatcher, int width, int height)
            : base(dispatcher, width, height, PixelFormat, BytesPerPixel)
        { }

        protected override unsafe void FillWritableBitmapLine(int y, IntPtr backBuffer, int backBufferStride)
        {
            byte* dstPtr = (byte*)backBuffer + y * backBufferStride;
            fixed (void* innerBufferPtr = innerBuffer)
            {
                short* srcPtr = (short*)innerBufferPtr + y * width;
                for (var x = 0; x < width; x++)
                {
                    var v = *(srcPtr++) >> 3;
                    // Some random heuristic to colorize depth map slightly like height-based colorization of earth maps
                    // (from blue though green to red)
                    *(dstPtr++) = (byte)(Math.Max(0, 220 - 3 * Math.Abs(150 - v) / 2));
                    *(dstPtr++) = (byte)(Math.Max(0, 220 - Math.Abs(350 - v)));
                    *(dstPtr++) = (byte)(Math.Max(0, 220 - Math.Abs(550 - v)));
                }
            }
        }
    }
}
