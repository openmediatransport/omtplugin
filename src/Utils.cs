using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace omtplugin
{
    internal class Utils
    {
        public static void P010ToP216(IntPtr srcY, int srcStrideY, IntPtr srcUV, int srcStrideUV, IntPtr dst, int dstStride, int width, int height)
        {
            unsafe
            {
                //Copy Y Plane
                IntPtr dstY = dst;
                for (int y = 0; y < height; y++)
                {
                    Buffer.MemoryCopy((void*)srcY, (void*)dstY, width * 2, width * 2);
                    srcY += srcStrideY;
                    dstY += dstStride;
                }

                //Copy UV Plane, duplicating each line to convert from 4:2:0 to 4:2:2
                IntPtr dstUV = dst + (dstStride * height);
                int uvStride = width * 2;
                for (int y = 0; y < (height >> 1); y++)
                {
                    Buffer.MemoryCopy((void*)srcUV, (void*)dstUV, uvStride, uvStride);
                    dstUV += dstStride;
                    Buffer.MemoryCopy((void*)srcUV, (void*)dstUV, uvStride, uvStride);
                    dstUV += dstStride;
                    srcUV += srcStrideUV;
                }
            }
        }

        public static void P216ToP010(IntPtr src, int srcStride, IntPtr dstY, int dstStrideY, IntPtr dstUV, int dstStrideUV, int width, int height)
        {
            unsafe
            {
                //Copy Y Plane
                IntPtr srcY = src;
                for (int y = 0; y < height; y++)
                {
                    Buffer.MemoryCopy((void*)srcY, (void*)dstY, width * 2, width * 2);
                    srcY += srcStride;
                    dstY += dstStrideY;
                }

                //Copy UV Plane, skipping every second line to convert from 4:2:2 to 4:2:0
                IntPtr srcUV = src + (srcStride * height);
                int uvStride = width * 2;
                for (int y = 0; y < (height >> 1); y++)
                {
                    Buffer.MemoryCopy((void*)srcUV, (void*)dstUV, uvStride, uvStride);
                    dstUV += dstStrideUV;
                    srcUV += srcStride;
                    srcUV += srcStride;
                }

            }
        }
    }
}
