/*
* MIT License
*
* Copyright (c) 2025 Open Media Transport Contributors
*
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
*
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*
*/

using System.Runtime.InteropServices;

namespace omtplugin
{
    internal class OBSSourceInstance : OBSBase
    {
        protected IntPtr instance;
        protected IntPtr source;
        public OBSSourceInstance(IntPtr source, IntPtr settings)
        {
            this.source = source;
            GCHandle handle = GCHandle.Alloc(this);
            instance = GCHandle.ToIntPtr(handle);
        }
        public virtual void UpdateSettings(IntPtr settings)
        {
        }

        public virtual IntPtr GetProperties()
        {
            return IntPtr.Zero;
        }
        public virtual UInt32 GetWidth()
        {
            return 1920;
        }
        public virtual UInt32 GetHeight()
        {
            return 1080;
        }
        public IntPtr ToIntPtr()
        {
            return instance;
        }
        public static OBSSourceInstance? FromIntPtr(IntPtr instance)
        {
            if (instance != IntPtr.Zero)
            {
                GCHandle handle = GCHandle.FromIntPtr(instance);
                if (handle.IsAllocated)
                {
                    if (handle.Target != null)
                    {
                        return (OBSSourceInstance)handle.Target;
                    }

                }
            }
            return null;
        }
        protected override void DisposeInternal()
        {
            if (instance != IntPtr.Zero)
            {
                GCHandle handle = GCHandle.FromIntPtr(instance);
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
                instance = IntPtr.Zero;
            }
        }
    }
}
