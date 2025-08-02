using libomtnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace omtplugin
{
    internal class OBSOutputSettingsInstance : OBSSourceInstance
    {

        private bool enabled = false;
        private OBSOutputSettings parent;

        public OBSOutputSettingsInstance(OBSOutputSettings parent, nint source, nint settings) : base(source, settings)
        {
            this.parent = parent;
            UpdateSettings(settings);
        }

        protected override void DisposeInternal()
        {
            base.DisposeInternal();
        }

        public override void UpdateSettings(IntPtr settings)
        {
            try
            {
                if (settings != IntPtr.Zero)
                {
                    OMTLogging.Write("UpdateSettings", "OMTOutput");
                    IntPtr mainOutput = OBSOutput.GetMainOutput();
                    if (mainOutput != IntPtr.Zero)
                    {
                        enabled = OBS.obs_data_get_bool(settings, "enabledProperty");
                        IntPtr n = OBS.obs_data_get_string(settings, "nameProperty");
                        if (n != IntPtr.Zero)
                        {
                            string? newName = Marshal.PtrToStringUTF8(n);
                            if (newName != parent.Name)
                            {
                                parent.Name = newName;
                                OMTLogging.Write("NewName: " + newName, "OMTOutput");
                                OBSOutput.StopInstance(mainOutput);
                            }
                        }
                        if (enabled)
                        {
                            OBSOutput.StartInstance(mainOutput);
                        }
                        else
                        {
                            OBSOutput.StopInstance(mainOutput);
                        }
                    }
                    parent.SaveSettings(settings);
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTOutputSettings.UpdateSettings");
            }
            base.UpdateSettings(settings);
        }

        public override nint GetProperties()
        {
            try
            {
                IntPtr properties = OBS.obs_properties_create();
                if (properties != IntPtr.Zero)
                {
                    OBS.obs_properties_set_flags(properties, OBS.OBS_PROPERTIES_DEFER_UPDATE);
                    IntPtr enabledProperty = OBS.obs_properties_add_bool(properties, "enabledProperty", "Output Enabled");
                    IntPtr nameProperty = OBS.obs_properties_add_text(properties, "nameProperty", "Output Name", 0);
                    OMTLogging.Write("GetProperties", "OMTOutput");
                }
                return properties;
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTSource.GetProperties");
            }
            return base.GetProperties();
        }
    }
}
