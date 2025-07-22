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

namespace omtplugin
{
    internal class OBSSource : IDisposable
    {
        private static OBS.obs_function_create createDelegate;
        private static OBS.obs_function_destroy destroyDelegate;
        private static OBS.obs_function_get_name getNameDelegate;
        private static OBS.obs_function_get_properties getPropertiesDelegate;
        private static OBS.obs_function_update updateDelegate;
        private static OBS.obs_function_get_defaults getDefaultsDelegate;
        private static OBS.obs_source_info info;

        private static IntPtr SourceID = Marshal.StringToCoTaskMemUTF8("omtsource");
        static OBSSource()
        {
            createDelegate = new OBS.obs_function_create(Create);
            destroyDelegate = new OBS.obs_function_destroy(Destroy);
            getNameDelegate = new OBS.obs_function_get_name(GetName);
            getPropertiesDelegate = new OBS.obs_function_get_properties(GetProperties);
            updateDelegate = new OBS.obs_function_update(Update);
            getDefaultsDelegate = new OBS.obs_function_get_defaults(GetDefaults);

            info.id = SourceID;
            info.icon_type = OBS.obs_icon_type.OBS_ICON_TYPE_CAMERA;
            info.type = OBS.obs_source_type.OBS_SOURCE_TYPE_INPUT;
            info.output_flags = OBS.OBS_SOURCE_ASYNC_VIDEO | OBS.OBS_SOURCE_AUDIO;
            info.get_name = Marshal.GetFunctionPointerForDelegate(getNameDelegate);
            info.create = Marshal.GetFunctionPointerForDelegate(createDelegate);
            info.destroy = Marshal.GetFunctionPointerForDelegate(destroyDelegate);
            info.get_properties = Marshal.GetFunctionPointerForDelegate(getPropertiesDelegate);
            info.get_defaults = Marshal.GetFunctionPointerForDelegate(getDefaultsDelegate);
            info.update = Marshal.GetFunctionPointerForDelegate(updateDelegate);
        }

        private IntPtr source;
        private bool threadRunning = false;
        private Thread? thread;
        private bool disposedValue;
        private OMTReceive? receiver;
        private IntPtr settings;
        private string? address = null;
        private OMTQuality suggestedQuality = OMTQuality.Default;
        private bool previewMode = false;

        private object lockSync = new object();

        public OBSSource(IntPtr source, IntPtr settings)
        {
            this.source = source;
            this.receiver = null;
            this.thread = null;
            this.threadRunning = false;
            UpdateSettings(settings);
        }

        private void StartThread()
        {
            lock (lockSync)
            {
                if (thread == null)
                {
                    threadRunning = true;
                    thread = new Thread(VideoProc);
                    thread.IsBackground = true;
                    thread.Start();
                    OMTLogging.Write("VideoProc.Started", "OMTSource");
                }
            }

        }
        private void StopThread()
        {
            lock (lockSync)
            {
                if (thread != null)
                {
                    threadRunning = false;
                    thread.Join(5000);
                    thread = null;
                    OMTLogging.Write("VideoProc.Stopped", "OMTSource");
                }
            }
        }
        private void UpdateReceiver()
        {
            try
            {
                lock (lockSync)
                {
                    if (receiver != null)
                    {
                        if (receiver.Address != this.address)
                        {
                            CreateReceiver();
                        }
                    }
                    else
                    {
                        CreateReceiver();
                    }
                    if (receiver != null)
                    {
                        receiver.SetSuggestedQuality(suggestedQuality);
                        if (previewMode)
                        {
                            receiver.SetFlags(OMTReceiveFlags.Preview);
                        }
                        else
                        {
                            receiver.SetFlags(OMTReceiveFlags.None);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTSource.UpdateReceiver");
            }
        }

        private void CreateReceiver()
        {
            try
            {
                StopThread();
                if (receiver != null)
                {
                    receiver.Dispose();
                }
                if (!String.IsNullOrEmpty(this.address))
                {
                    receiver = new OMTReceive(this.address, OMTFrameType.Video | OMTFrameType.Audio, OMTPreferredVideoFormat.UYVYorBGRA, OMTReceiveFlags.None);
                    StartThread();
                    OMTLogging.Write("New Receiver: " + this.address, "OMTSource");
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTSource.CreateReceiver");
            }
        }

        public void UpdateSettings(IntPtr settings)
        {
            try
            {
                this.settings = settings;
                if (this.settings != IntPtr.Zero)
                {
                    IntPtr pSource = OBS.obs_data_get_string(settings, "sourceProperty");
                    if (pSource != IntPtr.Zero)
                    {
                        string? source = Marshal.PtrToStringUTF8(pSource);
                        if (!String.IsNullOrEmpty(source))
                        {
                            this.address = source;
                        }
                        else
                        {
                            this.address = null;
                        }
                    }
                    else
                    {
                        this.address = null;
                    }
                    Int64 q = OBS.obs_data_get_int(settings, "qualityProperty");
                    this.suggestedQuality = (OMTQuality)q;

                    bool p = OBS.obs_data_get_bool(settings, "previewProperty");
                    this.previewMode = p;
                }
                UpdateReceiver();
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTSource.UpdateSettings");
            }               
        }

        private void VideoProc()
        {
            try
            {
                OBS.obs_source_frame frame = new OBS.obs_source_frame();
                frame.linesize = new uint[8];
                frame.data = new nint[8];                
                frame.color_matrix = new float[16];
                frame.color_range_min = new float[3];
                frame.color_range_max = new float[3];

                OBS.obs_source_audio audio = new OBS.obs_source_audio();
                audio.data = new nint[8];

                if (OBS.video_format_get_parameters(OBS.video_colorspace.VIDEO_CS_DEFAULT, OBS.video_range_type.VIDEO_RANGE_DEFAULT,
                    frame.color_matrix, frame.color_range_min, frame.color_range_max) == true) {
                    OMTLogging.Write("ColorFormatRetrieved", "OMTSource");
                }
                OMTMediaFrame mediaFrame = new OMTMediaFrame();

                while (threadRunning)
                {
                    OMTReceive? r = receiver;
                    if (r == null) break; ;
                    if (r.Receive(OMTFrameType.Video | OMTFrameType.Audio, 100, ref mediaFrame))
                    {
                        if (mediaFrame.DataLength > 0)
                        {
                            if (mediaFrame.Type == OMTFrameType.Audio)
                            {
                                audio.format = OBS.audio_format.AUDIO_FORMAT_FLOAT_PLANAR;
                                audio.speakers = OBS.speaker_layout.SPEAKERS_STEREO;
                                audio.samples_per_sec = (uint)mediaFrame.SampleRate;
                                audio.timestamp = (ulong)mediaFrame.Timestamp * 100;
                                audio.frames = (uint)mediaFrame.SamplesPerChannel;
                                audio.data[0] = mediaFrame.Data;
                                audio.data[1] = mediaFrame.Data + (mediaFrame.SamplesPerChannel * 4);
                                OBS.obs_source_output_audio(this.source, ref audio);
                            } else if (mediaFrame.Type == OMTFrameType.Video)
                            {
                                frame.width = (uint)mediaFrame.Width;
                                frame.height = (uint)mediaFrame.Height;
                                frame.timestamp = (ulong)mediaFrame.Timestamp * 100;
                                frame.data[0] = mediaFrame.Data;

                                if (mediaFrame.Codec == (int)OMTCodec.UYVY)
                                {
                                    frame.format = OBS.video_format.VIDEO_FORMAT_UYVY;
                                    frame.linesize[0] = frame.width * 2;
                                    OBS.obs_source_output_video(this.source, ref frame);
                                }
                                else if (mediaFrame.Codec == (int)OMTCodec.BGRA)
                                {
                                    frame.format = OBS.video_format.VIDEO_FORMAT_BGRA;
                                    frame.linesize[0] = frame.width * 4;
                                    OBS.obs_source_output_video(this.source, ref frame);
                                }
                            }                   
                        }
                    }                    
                }
                OMTLogging.Write("ThreadExited", "OMTSource");
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTSource.VideoProc");
            }
        }

        public static void Register()
        {
            OBS.obs_register_source_s(ref info, Marshal.SizeOf<OBS.obs_source_info>());
        }

        private static IntPtr Create(IntPtr settings, IntPtr source)
        {
            try
            {
                OBSSource instance = new OBSSource(source, settings);
                GCHandle handle = GCHandle.Alloc(instance);
                IntPtr p = GCHandle.ToIntPtr(handle);
                return p;
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTSource.Create");
            }
            return IntPtr.Zero;

        }
        private static void Destroy(IntPtr data)
        {
            try
            {
                GCHandle handle = GCHandle.FromIntPtr(data);
                if (handle.IsAllocated)
                {
                    OBSSource? source = (OBSSource?)handle.Target;
                    if (source != null)
                    {
                        source.Dispose();
                    }
                    handle.Free();
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTSource.Destroy");
            }
        }

        private static OBSSource? GetInstance(IntPtr data)
        {
            try
            {
                GCHandle handle = GCHandle.FromIntPtr(data);
                if (handle.IsAllocated)
                {
                    OBSSource? source = (OBSSource?)handle.Target;
                    if (source != null)
                    {
                        return source;
                    }
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTSource.GetInstance");
            }
            return null;
        }

        private static void Update(IntPtr data, IntPtr settings)
        {
            OBSSource? source = GetInstance(data);
            if (source != null)
            {
                source.UpdateSettings(settings);
            }
        }
        private static IntPtr GetName(IntPtr type_data)
        {
            return Marshal.StringToHGlobalAnsi("OMT Source");
        }
        private static UInt32 GetWidth(IntPtr data)
        {
            return 1920;
        }
        private static UInt32 GetHeight(IntPtr data)
        {
            return 1080;
        }
        private static IntPtr GetProperties(IntPtr data)
        {
            try
            {
                IntPtr properties = OBS.obs_properties_create();
                if (properties != IntPtr.Zero)
                {
                    IntPtr sourceProperty = OBS.obs_properties_add_list(properties, "sourceProperty", "Source", OBS.obs_combo_type.OBS_COMBO_TYPE_LIST, OBS.obs_combo_format.OBS_COMBO_FORMAT_STRING);
                    if (sourceProperty != IntPtr.Zero)
                    {
                        OBS.obs_property_list_add_string(sourceProperty, "", "");
                        string[] sources = OMTDiscovery.GetInstance().GetAddresses();
                        if (sources != null)
                        {
                            foreach (string source in sources)
                            {
                                OBS.obs_property_list_add_string(sourceProperty, source, source);
                            }
                        }
                    }
                    IntPtr qualityProperty = OBS.obs_properties_add_list(properties, "qualityProperty", "Suggested Quality", OBS.obs_combo_type.OBS_COMBO_TYPE_LIST, OBS.obs_combo_format.OBS_COMBO_FORMAT_INT);
                    if (qualityProperty != IntPtr.Zero)
                    {
                        foreach (OMTQuality quality in Enum.GetValues<OMTQuality>())
                        {
                            OBS.obs_property_list_add_int(qualityProperty, quality.ToString(), (long)quality);
                        }
                    }
                    IntPtr previewProperty = OBS.obs_properties_add_bool(properties, "previewProperty", "Preview Mode");
                }
                return properties;
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTSource.GetProperties");
            }
            return IntPtr.Zero;
        }

        private static void GetDefaults(IntPtr settings)
        {
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    StopThread();
                    if (receiver != null)
                    {
                        receiver.Dispose();
                        receiver = null;
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
