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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using libomtnet;
using System.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace omtplugin
{
    internal class OBSSource : OBSBase
    {
        private OBS.obs_function_create createDelegate;
        private OBS.obs_function_destroy destroyDelegate;
        private OBS.obs_function_get_name getNameDelegate;
        private OBS.obs_function_get_properties getPropertiesDelegate;
        private OBS.obs_function_update updateDelegate;
         private OBS.obs_function_get_defaults getDefaultsDelegate;
        private OBS.obs_source_info info;

        private string sourceId;
        private string sourceName;
        private IntPtr pSourceId; //"omtsource"
        private IntPtr pSourceName;
        public OBSSource(string id, string name, OBS.obs_icon_type icon_type, OBS.obs_source_type type, uint flags)
        {
            sourceId = id;
            sourceName = name;
            pSourceId = Marshal.StringToCoTaskMemUTF8(id);
            pSourceName = Marshal.StringToCoTaskMemUTF8(name);

            createDelegate = new OBS.obs_function_create(Create);
            destroyDelegate = new OBS.obs_function_destroy(Destroy);
            getNameDelegate = new OBS.obs_function_get_name(GetName);
            getPropertiesDelegate = new OBS.obs_function_get_properties(GetProperties);
            updateDelegate = new OBS.obs_function_update(Update);
            getDefaultsDelegate = new OBS.obs_function_get_defaults(GetDefaults);

            info.id = pSourceId;
            info.icon_type = icon_type;
            info.type = type;
            info.output_flags = flags;
            info.get_name = Marshal.GetFunctionPointerForDelegate(getNameDelegate);
            info.create = Marshal.GetFunctionPointerForDelegate(createDelegate);
            info.destroy = Marshal.GetFunctionPointerForDelegate(destroyDelegate);
            info.get_properties = Marshal.GetFunctionPointerForDelegate(getPropertiesDelegate);
            info.get_defaults = Marshal.GetFunctionPointerForDelegate(getDefaultsDelegate);
            info.update = Marshal.GetFunctionPointerForDelegate(updateDelegate);
        }

        protected override void DisposeInternal()
        {
            if (pSourceId != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(pSourceId);
                pSourceId = IntPtr.Zero;
            }
            if (pSourceName != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(pSourceName);
                pSourceName = IntPtr.Zero;
            }
            base.DisposeInternal();
        }

        public string SourceID {  get { return sourceId; } }
        public string SourceName {  get { return sourceName; } }
               
        public void Register()
        {
            OBS.obs_register_source_s(ref info, Marshal.SizeOf<OBS.obs_source_info>());
        }

        protected virtual OBSSourceInstance? CreateInstance(IntPtr source, IntPtr settings)
        {
            return null;
        }
        private IntPtr Create(IntPtr settings, IntPtr source)
        {
            try
            {
                OBSSourceInstance? instance = CreateInstance(source, settings);
                if (instance != null)
                {
                    OMTLogging.Write("Created: " + instance.ToIntPtr(), "OMTSource");
                    return instance.ToIntPtr();
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTSource.Create");
            }
            return IntPtr.Zero;
        }
        private void Destroy(IntPtr data)
        {
            try
            {
                OBSSourceInstance? instance = OBSSourceInstance.FromIntPtr(data);
                if (instance != null)
                {
                    instance.Dispose();
                    OMTLogging.Write("Destroyed: " + data, "OMTSource");
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTSource.Destroy");
            }
        }
        private void Update(IntPtr data, IntPtr settings)
        {
            OBSSourceInstance? source = OBSSourceInstance.FromIntPtr(data);
            if (source != null)
            {
                source.UpdateSettings(settings);
            }
        }
        private IntPtr GetName(IntPtr type_data)
        {
            return  pSourceName;
        }
        private UInt32 GetWidth(IntPtr data)
        {
            OBSSourceInstance? source = OBSSourceInstance.FromIntPtr(data);
            if (source != null)
            {
                return source.GetWidth();
            }
            return 0;
        }
        private UInt32 GetHeight(IntPtr data)
        {
            OBSSourceInstance? source = OBSSourceInstance.FromIntPtr(data);
            if (source != null)
            {
                return source.GetHeight();
            }
            return 0;
        }
        private IntPtr GetProperties(IntPtr data)
        {
            OBSSourceInstance? source = OBSSourceInstance.FromIntPtr(data);
            if (source != null)
            {
                return source.GetProperties();
            }
            return IntPtr.Zero;
        }
        protected virtual void GetDefaults(IntPtr settings)
        {
        }
    }
}
