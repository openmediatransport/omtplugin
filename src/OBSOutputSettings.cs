using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using libomtnet;
namespace omtplugin
{
    internal class OBSOutputSettings : OBSSource
    {
        private IntPtr pInstance;
        private OBSSourceInstance? instance = null;
        private IntPtr configPath = IntPtr.Zero;
        private string? name = null;
        private bool enabled = false;

        public OBSOutputSettings() 
            : base("omtoutputsettings", "OMT Output Settings", OBS.obs_icon_type.OBS_ICON_TYPE_CAMERA, 
                  OBS.obs_source_type.OBS_SOURCE_TYPE_FILTER, OBS.OBS_SOURCE_ASYNC_VIDEO | OBS.OBS_SOURCE_AUDIO | OBS.OBS_SOURCE_CAP_DISABLED)
        {
            configPath = OBS.obs_module_get_config_path(UnmanagedExports.obs_module_pointer, "omtplugin.json");
            if (configPath != IntPtr.Zero)
            {
                string? szConfigPath = Marshal.PtrToStringUTF8(configPath);
                if (szConfigPath != null)
                {
                    OMTLogging.Write("ConfigFilename: " + szConfigPath, "OMTOutput");
                    string? szPath = Path.GetDirectoryName(szConfigPath);
                    if (szPath != null)
                    {
                        if (Directory.Exists(szPath) == false)
                        {
                            Directory.CreateDirectory(szPath);
                            OMTLogging.Write("CreatedConfigPath: " + szPath, "OMTOutput");
                        }
                    }
                }
            }
        }

        public string? Name { get { return name; } }
        public bool Enabled {  get { return enabled; } }

        protected override void GetDefaults(nint settings)
        {
            if (settings != IntPtr.Zero)
            {
                OBS.obs_data_set_default_string(settings, "nameProperty", OBSOutput.DEFAULT_OUTPUT_NAME);
            }
            base.GetDefaults(settings);
        }

        protected override OBSSourceInstance CreateInstance(nint source, nint settings)
        {
            instance = new OBSOutputSettingsInstance(this, source, settings);
            return instance;
        }

        private void CreateInstance()
        {
            if (pInstance == IntPtr.Zero)
            {
                IntPtr settings = LoadSettings();
                pInstance = OBS.obs_source_create(SourceID, SourceName, settings, IntPtr.Zero);
                ReleaseSettings(settings);
            }
        }

        public IntPtr LoadSettings()
        {
            IntPtr data = IntPtr.Zero;
            if (configPath != IntPtr.Zero)
            {
                data = OBS.obs_data_create_from_json_file(configPath);
                OMTLogging.Write("Loaded config file", "OBSOutput");
            }
            return data;
        }
        public void SaveSettings(IntPtr settings)
        {
            if (settings != IntPtr.Zero)
            {
                enabled = OBS.obs_data_get_bool(settings, "enabledProperty");
                IntPtr n = OBS.obs_data_get_string(settings, "nameProperty");
                if (n != IntPtr.Zero)
                {
                    name = Marshal.PtrToStringUTF8(n);
                } else
                {
                    name = "";
                }
                if (configPath != IntPtr.Zero)
                {
                    OBS.obs_data_save_json(settings, configPath);
                    OMTLogging.Write("Saved config file", "OBSOutput");
                }
            }
        }

        public void ReleaseSettings(IntPtr settings)
        {
            if (settings != IntPtr.Zero)
            {
                OBS.obs_data_release(settings);
            }
        }

        public void Configure()
        {
            CreateInstance();
        }

        public void ShowSettings()
        {
            CreateInstance();
            OMTLogging.Write("ShowSettings", "OMTOutput");
            OBS.obs_frontend_open_source_properties(pInstance);
        }
        protected override void DisposeInternal()
        {
            if (pInstance != IntPtr.Zero)
            {
                OBS.obs_source_release(pInstance);
                pInstance = IntPtr.Zero;
            }
            if (configPath != IntPtr.Zero)
            {
                OBS.bfree(configPath);
                configPath = IntPtr.Zero;
            }
            base.DisposeInternal();
        }
    }
}
