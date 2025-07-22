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
using System.Runtime.Serialization;

namespace omtplugin
{
    internal class OBS
    {
        private const string DLL_PATH = "obs";
        private const string DLL_PATH_FRONTEND = "obs-frontend-api";

        public const int OBS_SOURCE_VIDEO = (1 << 0);
        public const int OBS_SOURCE_AUDIO = (1 << 1);
        public const int OBS_SOURCE_ASYNC = (1 << 2);
        public const int OBS_SOURCE_ASYNC_VIDEO = (OBS_SOURCE_ASYNC | OBS_SOURCE_VIDEO);

        public const int OBS_OUTPUT_VIDEO = (1 << 0);
        public const int OBS_OUTPUT_AUDIO = (1 << 1);
        public const int OBS_OUTPUT_AV = (OBS_OUTPUT_VIDEO | OBS_OUTPUT_AUDIO);

        public enum obs_source_type
        {
            OBS_SOURCE_TYPE_INPUT,
            OBS_SOURCE_TYPE_FILTER,
            OBS_SOURCE_TYPE_TRANSITION,
            OBS_SOURCE_TYPE_SCENE
        }
        public enum obs_icon_type
        {
            OBS_ICON_TYPE_UNKNOWN,
            OBS_ICON_TYPE_IMAGE,
            OBS_ICON_TYPE_COLOR,
            OBS_ICON_TYPE_SLIDESHOW,
            OBS_ICON_TYPE_AUDIO_INPUT,
            OBS_ICON_TYPE_AUDIO_OUTPUT,
            OBS_ICON_TYPE_DESKTOP_CAPTURE,
            OBS_ICON_TYPE_WINDOW_CAPTURE,
            OBS_ICON_TYPE_GAME_CAPTURE,
            OBS_ICON_TYPE_CAMERA,
            OBS_ICON_TYPE_TEXT,
            OBS_ICON_TYPE_MEDIA,
            OBS_ICON_TYPE_BROWSER,
            OBS_ICON_TYPE_CUSTOM,
            OBS_ICON_TYPE_PROCESS_AUDIO_OUTPUT,
        }

        public enum video_format
        {
            VIDEO_FORMAT_NONE,

            /* planar 4:2:0 formats */
            VIDEO_FORMAT_I420, /* three-plane */
            VIDEO_FORMAT_NV12, /* two-plane, luma and packed chroma */

            /* packed 4:2:2 formats */
            VIDEO_FORMAT_YVYU,
            VIDEO_FORMAT_YUY2, /* YUYV */
            VIDEO_FORMAT_UYVY,

            /* packed uncompressed formats */
            VIDEO_FORMAT_RGBA,
            VIDEO_FORMAT_BGRA,
            VIDEO_FORMAT_BGRX,
            VIDEO_FORMAT_Y800, /* grayscale */

            /* planar 4:4:4 */
            VIDEO_FORMAT_I444,

            /* more packed uncompressed formats */
            VIDEO_FORMAT_BGR3,

            /* planar 4:2:2 */
            VIDEO_FORMAT_I422,

            /* planar 4:2:0 with alpha */
            VIDEO_FORMAT_I40A,

            /* planar 4:2:2 with alpha */
            VIDEO_FORMAT_I42A,

            /* planar 4:4:4 with alpha */
            VIDEO_FORMAT_YUVA,

            /* packed 4:4:4 with alpha */
            VIDEO_FORMAT_AYUV,

            /* planar 4:2:0 format, 10 bpp */
            VIDEO_FORMAT_I010, /* three-plane */
            VIDEO_FORMAT_P010, /* two-plane, luma and packed chroma */

            /* planar 4:2:2 format, 10 bpp */
            VIDEO_FORMAT_I210,

            /* planar 4:4:4 format, 12 bpp */
            VIDEO_FORMAT_I412,

            /* planar 4:4:4:4 format, 12 bpp */
            VIDEO_FORMAT_YA2L,

            /* planar 4:2:2 format, 16 bpp */
            VIDEO_FORMAT_P216, /* two-plane, luma and packed chroma */

            /* planar 4:4:4 format, 16 bpp */
            VIDEO_FORMAT_P416, /* two-plane, luma and packed chroma */

            /* packed 4:2:2 format, 10 bpp */
            VIDEO_FORMAT_V210,

            /* packed uncompressed 10-bit format */
            VIDEO_FORMAT_R10L,
        }
        public enum video_colorspace
        {
            VIDEO_CS_DEFAULT,
            VIDEO_CS_601,
            VIDEO_CS_709,
            VIDEO_CS_SRGB,
            VIDEO_CS_2100_PQ,
            VIDEO_CS_2100_HLG,
        }
        public enum video_range_type
        {
            VIDEO_RANGE_DEFAULT,
            VIDEO_RANGE_PARTIAL,
            VIDEO_RANGE_FULL,
        }

        public enum speaker_layout
        {
            SPEAKERS_UNKNOWN,     /**< Unknown setting, fallback is stereo. */
            SPEAKERS_MONO,        /**< Channels: MONO */
            SPEAKERS_STEREO,      /**< Channels: FL, FR */
            SPEAKERS_2POINT1,     /**< Channels: FL, FR, LFE */
            SPEAKERS_4POINT0,     /**< Channels: FL, FR, FC, RC */
            SPEAKERS_4POINT1,     /**< Channels: FL, FR, FC, LFE, RC */
            SPEAKERS_5POINT1,     /**< Channels: FL, FR, FC, LFE, RL, RR */
            SPEAKERS_7POINT1 = 8, /**< Channels: FL, FR, FC, LFE, RL, RR, SL, SR */
        }
        public enum audio_format
        {
            AUDIO_FORMAT_UNKNOWN,

            AUDIO_FORMAT_U8BIT,
            AUDIO_FORMAT_16BIT,
            AUDIO_FORMAT_32BIT,
            AUDIO_FORMAT_FLOAT,

            AUDIO_FORMAT_U8BIT_PLANAR,
            AUDIO_FORMAT_16BIT_PLANAR,
            AUDIO_FORMAT_32BIT_PLANAR,
            AUDIO_FORMAT_FLOAT_PLANAR,
        }
        public enum obs_frontend_event
        {
            OBS_FRONTEND_EVENT_STREAMING_STARTING,
            OBS_FRONTEND_EVENT_STREAMING_STARTED,
            OBS_FRONTEND_EVENT_STREAMING_STOPPING,
            OBS_FRONTEND_EVENT_STREAMING_STOPPED,
            OBS_FRONTEND_EVENT_RECORDING_STARTING,
            OBS_FRONTEND_EVENT_RECORDING_STARTED,
            OBS_FRONTEND_EVENT_RECORDING_STOPPING,
            OBS_FRONTEND_EVENT_RECORDING_STOPPED,
            OBS_FRONTEND_EVENT_SCENE_CHANGED,
            OBS_FRONTEND_EVENT_SCENE_LIST_CHANGED,
            OBS_FRONTEND_EVENT_TRANSITION_CHANGED,
            OBS_FRONTEND_EVENT_TRANSITION_STOPPED,
            OBS_FRONTEND_EVENT_TRANSITION_LIST_CHANGED,
            OBS_FRONTEND_EVENT_SCENE_COLLECTION_CHANGED,
            OBS_FRONTEND_EVENT_SCENE_COLLECTION_LIST_CHANGED,
            OBS_FRONTEND_EVENT_PROFILE_CHANGED,
            OBS_FRONTEND_EVENT_PROFILE_LIST_CHANGED,
            OBS_FRONTEND_EVENT_EXIT,

            OBS_FRONTEND_EVENT_REPLAY_BUFFER_STARTING,
            OBS_FRONTEND_EVENT_REPLAY_BUFFER_STARTED,
            OBS_FRONTEND_EVENT_REPLAY_BUFFER_STOPPING,
            OBS_FRONTEND_EVENT_REPLAY_BUFFER_STOPPED,

            OBS_FRONTEND_EVENT_STUDIO_MODE_ENABLED,
            OBS_FRONTEND_EVENT_STUDIO_MODE_DISABLED,
            OBS_FRONTEND_EVENT_PREVIEW_SCENE_CHANGED,

            OBS_FRONTEND_EVENT_SCENE_COLLECTION_CLEANUP,
            OBS_FRONTEND_EVENT_FINISHED_LOADING,

            OBS_FRONTEND_EVENT_RECORDING_PAUSED,
            OBS_FRONTEND_EVENT_RECORDING_UNPAUSED,

            OBS_FRONTEND_EVENT_TRANSITION_DURATION_CHANGED,
            OBS_FRONTEND_EVENT_REPLAY_BUFFER_SAVED,

            OBS_FRONTEND_EVENT_VIRTUALCAM_STARTED,
            OBS_FRONTEND_EVENT_VIRTUALCAM_STOPPED,

            OBS_FRONTEND_EVENT_TBAR_VALUE_CHANGED,
            OBS_FRONTEND_EVENT_SCENE_COLLECTION_CHANGING,
            OBS_FRONTEND_EVENT_PROFILE_CHANGING,
            OBS_FRONTEND_EVENT_SCRIPTING_SHUTDOWN,
            OBS_FRONTEND_EVENT_PROFILE_RENAMED,
            OBS_FRONTEND_EVENT_SCENE_COLLECTION_RENAMED,
            OBS_FRONTEND_EVENT_THEME_CHANGED,
            OBS_FRONTEND_EVENT_SCREENSHOT_TAKEN,

            OBS_FRONTEND_EVENT_CANVAS_ADDED,
            OBS_FRONTEND_EVENT_CANVAS_REMOVED,
        }

        public delegate IntPtr obs_function_get_name(IntPtr type_data);
        public delegate IntPtr obs_function_create(IntPtr settings, IntPtr source);
        public delegate void obs_function_destroy(IntPtr data);
        public delegate UInt32 obs_source_info_get_width(IntPtr data);
        public delegate UInt32 obs_source_info_get_height(IntPtr data);
        public delegate IntPtr obs_function_get_properties(IntPtr data);
        public delegate void obs_function_update(IntPtr data, IntPtr settings);
        public delegate void obs_function_get_defaults(IntPtr settings);
        public delegate bool obs_function_start(IntPtr data);
        public delegate void obs_function_stop(IntPtr data, UInt64 ts);
        public delegate void obs_function_raw_video(IntPtr data, ref video_data frame);
        public delegate void obs_function_raw_audio(IntPtr data, ref audio_data frames);
        public delegate void obs_function_encoded_packet(IntPtr data, IntPtr packet);
        public delegate void obs_fontend_event_cb(obs_frontend_event evt, IntPtr private_data);
        public delegate void obs_frontend_cb(IntPtr private_data);

        //MAX_AV_PLANES = 8

        [StructLayout(LayoutKind.Sequential)]
        public struct video_data
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public IntPtr[] data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public UInt32[] linesize;
            public UInt64 timestamp;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct audio_data
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public IntPtr[] data;
            public UInt32 frames;
            public UInt64 timestamp;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct obs_source_audio
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public IntPtr[] data;
            public UInt32 frames;
            public speaker_layout speakers;
            public audio_format format;
            public UInt32 samples_per_sec;
            public UInt64 timestamp;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct obs_source_frame
        {
            [MarshalAs(UnmanagedType.ByValArray,SizeConst =8)]
            public IntPtr[] data;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public UInt32[] linesize;
            public UInt32 width;
            public UInt32 height;
            public UInt64 timestamp;
            public video_format format;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] color_matrix;
            public byte full_range;
            public UInt16 max_luminance;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] color_range_min;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
            public float[] color_range_max;
            public byte flip;
            public byte flags;
            public byte trc;
            public int refs;
            public bool prev_frame;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct obs_output_info
        {
            public IntPtr id;
            public UInt32 flags;
            public IntPtr get_name;
            public IntPtr create;
            public IntPtr destroy;
            public IntPtr start;
            public IntPtr stop;
            public IntPtr raw_video;
            public IntPtr raw_audio;
            public IntPtr encoded_packet;
            public IntPtr update;
            public IntPtr get_defaults;
            public IntPtr get_properties;
            public IntPtr unused1;
            public IntPtr get_total_bytes;
            public IntPtr get_dropped_frames;
            public IntPtr type_data;
            public IntPtr free_type_data;
            public IntPtr get_congestion;
            public IntPtr get_connect_time_ms;
            public IntPtr encoded_video_codecs;
            public IntPtr encoded_audio_codecs;
            public IntPtr raw_audio2;
            public IntPtr protocols;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct audio_output_info
        {
            public IntPtr name;
            public UInt32 samples_per_sec;
            public audio_format format;
            public speaker_layout speakers;
            public IntPtr input_callback;
            public IntPtr input_param;
        }
        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
        public struct obs_source_info
        {
            public IntPtr id;
            public obs_source_type type;
            public UInt32 output_flags;
            public IntPtr get_name;
            public IntPtr create;
            public IntPtr destroy;
            public IntPtr get_width;
            public IntPtr get_height;

            public IntPtr get_defaults;
            public IntPtr get_properties;
            public IntPtr update;
            public IntPtr activate;
            public IntPtr deactivate;
            public IntPtr show;
            public IntPtr hide;
            public IntPtr video_tick;
            public IntPtr video_render;
            public IntPtr filter_video;
            public IntPtr filter_audio;
            public IntPtr enum_active_sources;
            public IntPtr save;
            public IntPtr load;
            public IntPtr mouse_click;
            public IntPtr mouse_move;
            public IntPtr mouse_wheel;
            public IntPtr focus;
            public IntPtr key_click;
            public IntPtr filter_remove;
            public IntPtr type_data;
            public IntPtr free_type_data;
            public IntPtr audio_render;
            public IntPtr enum_all_sources;
            public IntPtr transition_start;
            public IntPtr transition_stop;
            public IntPtr get_defaults2;
            public IntPtr get_properties2;
            public IntPtr audio_mix;
            public obs_icon_type icon_type;
            public IntPtr media_play_pause;
            public IntPtr media_restart;
            public IntPtr media_stop;
            public IntPtr media_next;
            public IntPtr media_previous;
            public IntPtr media_get_duration;
            public IntPtr media_get_time;
            public IntPtr media_set_time;
            public IntPtr media_get_state;
            public UInt32 version;
            public IntPtr unversioned_id;
            public IntPtr missing_files;
            public IntPtr video_get_color_space;
            public IntPtr filter_add;

        }

        public enum obs_combo_type
        {
            OBS_COMBO_TYPE_INVALID,
            OBS_COMBO_TYPE_EDITABLE,
            OBS_COMBO_TYPE_LIST,
            OBS_COMBO_TYPE_RADIO,
        }
        public enum obs_combo_format
        {
            OBS_COMBO_FORMAT_INVALID,
            OBS_COMBO_FORMAT_INT,
            OBS_COMBO_FORMAT_FLOAT,
            OBS_COMBO_FORMAT_STRING,
            OBS_COMBO_FORMAT_BOOL,
        }

        [DllImport(DLL_PATH_FRONTEND)]
        public static extern void obs_frontend_add_event_callback(IntPtr callback, IntPtr private_data);
        [DllImport(DLL_PATH_FRONTEND, CharSet=CharSet.Ansi)]
        public static extern void obs_frontend_add_tools_menu_item(string name, IntPtr callback, IntPtr private_data);
        [DllImport(DLL_PATH_FRONTEND)]
        public static extern IntPtr obs_frontend_get_main_window();

        [DllImport(DLL_PATH, CharSet=CharSet.Ansi)]
        public static extern IntPtr obs_output_create(string id, string name, IntPtr settings, IntPtr hotkey_data);
        [DllImport(DLL_PATH)]
        public static extern void obs_output_release(IntPtr output);

        [DllImport(DLL_PATH)]
        public static extern IntPtr obs_data_create();

        [DllImport(DLL_PATH)]
        public static extern void obs_data_release(IntPtr data);

        [DllImport(DLL_PATH)]
        public static extern void obs_register_source_s(ref obs_source_info info, IntPtr size);
        [DllImport(DLL_PATH)]
        public static extern video_format video_output_get_format(IntPtr video);
        [DllImport(DLL_PATH)]
        public static extern UInt32 video_output_get_width(IntPtr video);
        [DllImport(DLL_PATH)]
        public static extern UInt32 video_output_get_height(IntPtr video);
        [DllImport(DLL_PATH)]
        public static extern double video_output_get_frame_rate(IntPtr video);
        [DllImport(DLL_PATH)]
        public static extern IntPtr audio_output_get_info(IntPtr audio);

        [DllImport(DLL_PATH)]
        public static extern IntPtr obs_output_video(IntPtr output);
        [DllImport(DLL_PATH)]
        public static extern IntPtr obs_output_audio(IntPtr output);

        [DllImport(DLL_PATH)]
        public static extern bool obs_output_start(IntPtr output);
        [DllImport(DLL_PATH)]
        public static extern void obs_output_stop(IntPtr output);

        [DllImport(DLL_PATH)]
        public static extern bool obs_output_begin_data_capture(IntPtr output, int flags);

        [DllImport(DLL_PATH)]
        public static extern void obs_output_end_data_capture(IntPtr output);

        [DllImport(DLL_PATH)]
        public static extern void obs_register_output_s(ref obs_output_info info, IntPtr size);

        [DllImport(DLL_PATH)]
        public static extern void obs_source_output_video(IntPtr source, ref obs_source_frame frame);

        [DllImport(DLL_PATH)]
        public static extern void obs_source_output_audio(IntPtr source, ref obs_source_audio audio);

        [DllImport(DLL_PATH)]
        public static extern bool video_format_get_parameters(video_colorspace color_space, video_range_type range, float[] matrix, float[] min_range, float[] max_range);

        [DllImport(DLL_PATH)]
        public static extern IntPtr obs_properties_create();

        [DllImport(DLL_PATH)]
        public static extern void obs_properties_destroy(IntPtr properties);

        [DllImport(DLL_PATH, CharSet=CharSet.Ansi)]
        public static extern IntPtr obs_properties_add_list(IntPtr properties, string name, string description, obs_combo_type type, obs_combo_format format);


        [DllImport(DLL_PATH, CharSet = CharSet.Ansi)]
        public static extern IntPtr obs_properties_add_bool(IntPtr properties, string name, string description);


        [DllImport(DLL_PATH, CharSet = CharSet.Ansi)]
        public static extern IntPtr obs_property_list_add_string(IntPtr property, string name, string val);

        [DllImport(DLL_PATH, CharSet = CharSet.Ansi)]
        public static extern IntPtr obs_property_list_add_int(IntPtr property, string name, Int64 val);

        [DllImport(DLL_PATH)]
        public static extern IntPtr obs_property_list_clear(IntPtr property);

        [DllImport(DLL_PATH, CharSet = CharSet.Ansi)]
        public static extern IntPtr obs_data_get_string(IntPtr data, string name);

        [DllImport(DLL_PATH)]
        public static extern Int64 obs_data_get_int(IntPtr data, string name);

        [DllImport(DLL_PATH)]
        public static extern bool obs_data_get_bool(IntPtr data, string name);
    }
}
