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

using libomtnet;
using System.Runtime.InteropServices;

namespace omtplugin
{
    public class UnmanagedExports
    {
        private const UInt32 LIBOBS_API_MAJOR_VER = 30;
        private const UInt32 LIBOBS_API_MINOR_VER = 0;
        private const UInt32 LIBOBS_API_PATCH_VER = 0;

        private static UInt32 MAKE_SEMANTIC_VERSION(UInt32 major, UInt32 minor, UInt32 patch)
        {
            return ((major << 24) | (minor << 16) | patch);
        }

        private static void LoadLibraries()
        {
            if (OMTPlatform.GetPlatformType() == OMTPlatformType.Win32)
            {
                string libvmxPath = AppContext.BaseDirectory + @"\..\..\obs-plugins\64bit\libvmx.dll";
                OMTPlatform.GetInstance().OpenLibrary(libvmxPath);
            }
        }

        private static IntPtr obs_module_pointer;
        [UnmanagedCallersOnly(EntryPoint = "obs_module_load")]
        public static bool ObsModuleLoad()
        {
            LoadLibraries();
            OBSSource.Register();
            OBSOutput.Register();
            //Start listening for sources right away
            OMTDiscovery discovery = OMTDiscovery.GetInstance();
            return true;
        }

        [UnmanagedCallersOnly(EntryPoint = "obs_module_set_pointer")]
        public static void ObsModuleSetPointer(IntPtr module)
        {
            obs_module_pointer = module;
        }

        [UnmanagedCallersOnly(EntryPoint = "obs_current_module")]
        public static IntPtr ObsCurrentModule()
        {
            return obs_module_pointer;
        }

        [UnmanagedCallersOnly(EntryPoint = "obs_module_ver")]
        public static UInt32 ObsModuleVer()
        {
            return MAKE_SEMANTIC_VERSION(LIBOBS_API_MAJOR_VER, LIBOBS_API_MINOR_VER, LIBOBS_API_PATCH_VER);
        }
    }
}
