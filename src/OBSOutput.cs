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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace omtplugin
{
    internal class OBSOutput : OBSBase
    {
        private static OBS.obs_function_create createDelegate;
        private static OBS.obs_function_destroy destroyDelegate;
        private static OBS.obs_function_get_name getNameDelegate;
        private static OBS.obs_function_start startDelegate;
        private static OBS.obs_function_stop stopDelegate;
        private static OBS.obs_function_raw_video rawVideoDelegate;
        private static OBS.obs_function_raw_audio rawAudioDelegate;
        private static OBS.obs_function_update updateDelegate;
        private static OBS.obs_function_get_properties getPropertiesDelegate;
        private static OBS.obs_function_get_defaults getDefaultsDelegate;
        private static OBS.obs_function_encoded_packet encodedPacketDelegate;
        private static OBS.obs_frontend_cb menuDelegate;

        private static OBS.obs_output_info info;

        private const string OutputIDString = "omtoutput";
        private static IntPtr OutputID = Marshal.StringToCoTaskMemUTF8(OutputIDString);

        private static OBSOutputSettings? outputSettings = null;
        
        static OBSOutput()
        {
            createDelegate = new OBS.obs_function_create(Create);
            destroyDelegate = new OBS.obs_function_destroy(Destroy);
            getNameDelegate = new OBS.obs_function_get_name(GetName);
            startDelegate = new OBS.obs_function_start(Start);
            stopDelegate = new OBS.obs_function_stop(Stop);
            rawVideoDelegate = new OBS.obs_function_raw_video(RawVideo);
            rawAudioDelegate = new OBS.obs_function_raw_audio(RawAudio);
            getPropertiesDelegate = new OBS.obs_function_get_properties(GetProperties);
            updateDelegate = new OBS.obs_function_update(Update);
            getDefaultsDelegate = new OBS.obs_function_get_defaults(GetDefaults);
            eventCallback = new OBS.obs_fontend_event_cb(EventCallback);
            encodedPacketDelegate = new OBS.obs_function_encoded_packet(EncodedPacket);
            menuDelegate = new OBS.obs_frontend_cb(MenuClick);

            info.id = OutputID;
            info.flags = OBS.OBS_OUTPUT_AV;
            info.get_name = Marshal.GetFunctionPointerForDelegate(getNameDelegate);
            info.create = Marshal.GetFunctionPointerForDelegate(createDelegate);
            info.destroy = Marshal.GetFunctionPointerForDelegate(destroyDelegate);
            info.start = Marshal.GetFunctionPointerForDelegate(startDelegate);
            info.stop = Marshal.GetFunctionPointerForDelegate(stopDelegate);
            info.raw_audio = Marshal.GetFunctionPointerForDelegate(rawAudioDelegate);
            info.raw_video = Marshal.GetFunctionPointerForDelegate(rawVideoDelegate);
            info.get_properties = Marshal.GetFunctionPointerForDelegate(getPropertiesDelegate);
            info.get_defaults = Marshal.GetFunctionPointerForDelegate(getDefaultsDelegate);
            info.update = Marshal.GetFunctionPointerForDelegate(updateDelegate);
            info.encoded_packet = Marshal.GetFunctionPointerForDelegate(encodedPacketDelegate);
        }

        private IntPtr output;
        private IntPtr settings;
        private OMTSend? send;
        private OMTMediaFrame videoFrame;
        private OMTMediaFrame audioFrame;
        private static OBS.obs_fontend_event_cb eventCallback;
        private object lockSync = new object();

        private static IntPtr mainInstance;

        private IntPtr instance;

        private int strideHeight = 0;
        private OBS.video_format sourceFormat = 0;
        private IntPtr tempData = IntPtr.Zero;
        public const string DEFAULT_OUTPUT_NAME = "OBS Output";

        public OBSOutput(IntPtr output, IntPtr settings)
        {            
            this.output = output;
            this.settings = settings;
            GCHandle handle = GCHandle.Alloc(this);
            instance = GCHandle.ToIntPtr(handle);
        }

        private static void MenuClick(IntPtr private_data)
        {
            SettingsMenu();
        }

        private static void SettingsMenu()
        {
            if (outputSettings != null)
            {
                outputSettings.ShowSettings();
            }
        }

        private static void EncodedPacket(IntPtr data, IntPtr packet)
        {

        }
        public IntPtr ToIntPtr()
        {
            return instance;
        }
        public static void StartInstance(IntPtr instance)
        {
            try
            {
                bool result = OBS.obs_output_start(instance);
                if (result)
                {
                    OMTLogging.Write("StartInstance: " + instance, "OMTOutput");
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTOutput.StartInstance");
            }
        }
        public static void StopInstance(IntPtr instance)
        {
            try
            {
                OBS.obs_output_stop(instance);
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTOutput.StopInstance");
            }
        }
        public static OBSOutput? FromIntPtr(IntPtr instance)
        {
            if (instance != IntPtr.Zero)
            {
                GCHandle handle = GCHandle.FromIntPtr(instance);
                if (handle.IsAllocated)
                {
                    if (handle.Target != null)
                    {
                        return (OBSOutput)handle.Target;
                    }

                }
            }
            return null;
        }

        private static void ConfigureOutputSettings()
        {
            try
            {
                if (outputSettings != null)
                {
                    outputSettings.Configure();
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTOutput.ConfigureSettings");
            }
        }

        public static void UpdateMainOutput()
        {
            if (outputSettings != null)
            {
                DestroyMainOutput();
                if (outputSettings.Enabled)
                {
                    CreateMainOutput();
                    if (mainInstance != IntPtr.Zero)
                    {
                        StartInstance(mainInstance);
                    }
                }
            }
        }

        private static void CreateMainOutput()
        {
            try
            {
                IntPtr data = OBS.obs_data_create();
                mainInstance = OBS.obs_output_create(OutputIDString, "OMT Output", data, IntPtr.Zero);
                if (mainInstance != IntPtr.Zero)
                {
                    OMTLogging.Write("CreateMainOutput: " + OutputIDString + "," + mainInstance, "OMTOutput");
                }
                OBS.obs_data_release(data);
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTOutput.CreateMainOutput");
            }
        }
        private static void DestroyMainOutput()
        {
            try
            {
                if (mainInstance != IntPtr.Zero)
                {
                    StopInstance(mainInstance);
                    OBS.obs_output_release(mainInstance);
                    mainInstance = IntPtr.Zero;
                    OMTLogging.Write("DestroyMainOutput", "OMTOutput");
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTOutput.DestroyMainOutput");
            }
        }

        private static void EventCallback(OBS.obs_frontend_event evt, IntPtr private_data)
        {
            try
            {
                if (evt == OBS.obs_frontend_event.OBS_FRONTEND_EVENT_FINISHED_LOADING || evt == OBS.obs_frontend_event.OBS_FRONTEND_EVENT_PROFILE_CHANGED)
                {
                    ConfigureOutputSettings();
                }
                else if (evt == OBS.obs_frontend_event.OBS_FRONTEND_EVENT_EXIT || evt == OBS.obs_frontend_event.OBS_FRONTEND_EVENT_PROFILE_CHANGING)
                {
                    DestroyMainOutput();
                }
                if (evt == OBS.obs_frontend_event.OBS_FRONTEND_EVENT_EXIT)
                {
                    DestroyFrontEnd();
                    DestroyOutputSettings();
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTOutput.EventCallback");
            }
        }

        private static void Update(IntPtr data, IntPtr settings)
        {
        }

        public static void Register()
        {
            try
            {
                OBS.obs_register_output_s(ref info, Marshal.SizeOf<OBS.obs_output_info>());
                CreateOutputSettings();
                CreateFrontEnd();
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(),"OMTOutput.Register");
            }
        }

        private static void CreateFrontEnd()
        {
            IntPtr mainWindow = OBS.obs_frontend_get_main_window();
            if (mainWindow != IntPtr.Zero)
            {
                OBS.obs_frontend_add_event_callback(Marshal.GetFunctionPointerForDelegate(eventCallback), IntPtr.Zero);
                OBS.obs_frontend_add_tools_menu_item("OMT Output Settings", Marshal.GetFunctionPointerForDelegate(menuDelegate), new nint(0));
            }
        }

        private static void DestroyFrontEnd()
        {
        }

        private static void CreateOutputSettings()
        {
            outputSettings = new OBSOutputSettings();
            outputSettings.Register();
        }

        private static void DestroyOutputSettings()
        {
            if (outputSettings != null)
            {
                outputSettings.Dispose();
            }
        }

        public bool StartOutput()
        {
            try
            {
                OMTLogging.Write("StartOutput: " + this.output.ToString(), "OMTOutput");
                lock (lockSync)
                {
                    if (send == null)
                    {
                        IntPtr audio = OBS.obs_output_audio(this.output);
                        if (audio != IntPtr.Zero)
                        {
                            IntPtr video = OBS.obs_output_video(this.output);
                            if (video != IntPtr.Zero)
                            {
                                videoFrame = new OMTMediaFrame();
                                audioFrame = new OMTMediaFrame();

                                videoFrame.Type = OMTFrameType.Video;
                                audioFrame.Type = OMTFrameType.Audio;

                                videoFrame.Width = (int)OBS.video_output_get_width(video);
                                videoFrame.Height = (int)OBS.video_output_get_height(video);
                                videoFrame.FrameRate = (float)OBS.video_output_get_frame_rate(video);
                                OBS.video_format fmt = OBS.video_output_get_format(video);

                                OMTLogging.Write("OMTOutputFormat: " + videoFrame.Width + "x" + videoFrame.Height + " " + videoFrame.FrameRate.ToString() + " fps " + fmt.ToString(),"OMTOutput");
                                                                
                                if (fmt == OBS.video_format.VIDEO_FORMAT_NV12)
                                {
                                    videoFrame.Codec = (int)OMTCodec.NV12;
                                    strideHeight = (int)(videoFrame.Height * 1.5F);
                                }
                                else if (fmt == OBS.video_format.VIDEO_FORMAT_UYVY)
                                {
                                    videoFrame.Codec = (int)OMTCodec.UYVY;
                                    strideHeight = (int)(videoFrame.Height);
                                }
                                else if (fmt == OBS.video_format.VIDEO_FORMAT_YUY2)
                                {
                                    videoFrame.Codec = (int)OMTCodec.YUY2;
                                    strideHeight = (int)(videoFrame.Height);
                                }
                                else if (fmt == OBS.video_format.VIDEO_FORMAT_BGRA)
                                {
                                    videoFrame.Flags = OMTVideoFlags.Alpha | OMTVideoFlags.PreMultiplied;
                                    videoFrame.Codec = (int)OMTCodec.BGRA;
                                    strideHeight = (int)(videoFrame.Height);
                                }
                                else if (fmt == OBS.video_format.VIDEO_FORMAT_BGRX)
                                {
                                    videoFrame.Codec = (int)OMTCodec.BGRA;
                                    videoFrame.Flags = OMTVideoFlags.None;
                                    strideHeight = (int)(videoFrame.Height);
                                } else if (fmt == OBS.video_format.VIDEO_FORMAT_P216 || fmt == OBS.video_format.VIDEO_FORMAT_P010)
                                {
                                    videoFrame.Codec = (int)OMTCodec.P216;
                                    videoFrame.Flags = OMTVideoFlags.HighBitDepth;
                                    strideHeight = (int)videoFrame.Height * 2;
                                }
                                else
                                {
                                    OMTLogging.Write("Video Format not supported: " + fmt.ToString(), "OMTOutput");
                                    return false;
                                }
                                sourceFormat = fmt;

                                IntPtr pInfo = OBS.audio_output_get_info(audio);
                                OBS.audio_output_info info = Marshal.PtrToStructure<OBS.audio_output_info>(pInfo);

                                if (info.format != OBS.audio_format.AUDIO_FORMAT_FLOAT_PLANAR)
                                {
                                    OMTLogging.Write("Audio Format not supported: " + info.format.ToString(), "OMTOutput");
                                    return false;
                                }

                                audioFrame.Channels = 2;
                                audioFrame.SampleRate = (int)info.samples_per_sec;
                                audioFrame.Codec = (int)OMTCodec.FPA1;

                                string? name = DEFAULT_OUTPUT_NAME;
                                if (outputSettings != null)
                                {
                                    if (!String.IsNullOrEmpty(outputSettings.Name))
                                    {
                                        name = outputSettings.Name;
                                    }
                                }      
                                send = new OMTSend(name, OMTQuality.Default);

                                OBS.obs_output_begin_data_capture(this.output, 0);
                                return true;
                            }
                            else
                            {
                                OMTLogging.Write("No video context", "OMTOutput");
                            }
                        }
                        else
                        {
                            OMTLogging.Write("No audio context", "OMTOutput");
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTOutput.StartOutput");
            }            
            return false;
        }
        public void StopOutput()
        {
            try
            {
                OMTLogging.Write("StopOutput: " + this.output.ToString(), "OMTOutput");
                lock (lockSync)
                {
                    if (this.output != IntPtr.Zero)
                    {
                        OBS.obs_output_end_data_capture(this.output);
                    }
                    if (send != null)
                    {
                        send.Dispose();
                        send = null;
                    }
                    if (tempData != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(tempData);
                        tempData = IntPtr.Zero;
                    }
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTOutput.StopOutput");
            }
        }

        public void ProcessVideo(ref OBS.video_data frame)
        {
            if (frame.data != null)
            {
                if (frame.data[0] != IntPtr.Zero)
                {
                    videoFrame.Stride = (int)frame.linesize[0];
                    if (sourceFormat == OBS.video_format.VIDEO_FORMAT_P010)
                    {
                        //Convert to P216
                        if (tempData == IntPtr.Zero)
                        {
                            int len = videoFrame.Stride * strideHeight;
                            tempData = Marshal.AllocHGlobal(len);
                        }
                        Utils.P010ToP216(frame.data[0], (int)frame.linesize[0], frame.data[1], (int)frame.linesize[1], tempData, videoFrame.Stride, videoFrame.Width, videoFrame.Height);
                        videoFrame.Data = tempData;
                    } else
                    {
                        videoFrame.Data = frame.data[0];
                    }
                    videoFrame.Timestamp = (Int64)frame.timestamp / 100;
                    videoFrame.DataLength = videoFrame.Stride * strideHeight;
                    lock (lockSync)
                    {
                        if (send != null)
                        {
                            send.Send(videoFrame);
                        }
                    }
                }
            }
        }
        public void ProcessAudio(ref OBS.audio_data frames)
        {
            if (frames.data != null)
            {
                if (frames.data[0] != IntPtr.Zero)
                {
                    audioFrame.Data = frames.data[0];
                    audioFrame.Timestamp = (Int64)frames.timestamp / 100;
                    audioFrame.SamplesPerChannel = (int)frames.frames;
                    audioFrame.DataLength = audioFrame.SamplesPerChannel * 4 * 2;
                    lock (lockSync)
                    {
                        if (send != null)
                        {
                            send.Send(audioFrame);
                        }
                    }
                }
            }
        }
        private static void RawVideo(IntPtr data, ref OBS.video_data frame)
        {
            try
            {
                OBSOutput? output = OBSOutput.FromIntPtr(data);
                if (output != null)
                {
                    output.ProcessVideo(ref frame);
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTOutput.RawVideo");
            }

        }
        private static void RawAudio(IntPtr data, ref OBS.audio_data frames)
        {
            try
            {
                OBSOutput? output = OBSOutput.FromIntPtr(data);
                if (output != null)
                {
                    output.ProcessAudio(ref frames);
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTOutput.RawAudio");
            }
        }

        private static bool Start(IntPtr data)
        {
            OBSOutput? output = OBSOutput.FromIntPtr(data);
            if (output != null)
            {
                return output.StartOutput();
            }
            return false;
        }
        private static void Stop(IntPtr data, UInt64 ts)
        {
            OBSOutput? output = OBSOutput.FromIntPtr(data);
            if (output != null)
            {
                output.StopOutput();
            }
        }
        private static IntPtr GetName(IntPtr type_data)
        {
            return Marshal.StringToHGlobalAnsi("OMT Output");
        }
        private static void GetDefaults(IntPtr settings)
        {
        }
        private static IntPtr GetProperties(IntPtr data)
        {
            return IntPtr.Zero;
        }
        private static IntPtr Create(IntPtr settings, IntPtr source)
        {
            try
            {
                OBSOutput output = new OBSOutput(source, settings);
                IntPtr data = output.ToIntPtr();
                OMTLogging.Write("Create: Data: " + data.ToString() + " Source: " + source.ToString(),"OMTOutput");
                return data;
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTOutput.Create");
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
                    OBSOutput? output = (OBSOutput?)handle.Target;
                    if (output != null)
                    {
                        output.Dispose();
                    }
                    handle.Free();
                }
            }
            catch (Exception ex)
            {
                OMTLogging.Write(ex.ToString(), "OMTOutput.Destroy");
            }
        }

        protected override void DisposeInternal()
        {
            if (this.send != null)
            {
                this.send.Dispose();
                this.send = null;
            }
            if (instance != IntPtr.Zero)
            {
                GCHandle handle = GCHandle.FromIntPtr(instance);
                if (handle.IsAllocated)
                {
                    handle.Free();
                }
                instance = IntPtr.Zero;
            }
            base.DisposeInternal();
        }

    }
}
