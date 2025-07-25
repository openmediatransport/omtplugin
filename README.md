# Open Media Transport plugin for OBS

omtplugin is a cross platform Open Media Transport plugin for OBS written in C#.NET

The plugin uses C#.NET interop to implement the OBS plugin interfaces directly without requiring the plugin template.
This may also serve as a reference for those interested in implementing their own basic OBS plugins in .NET

## Features

### OMT Source

Receive audio/video from OMT sources on the network.

### OMT Output

Send audio/video from the main OBS output by clicking the Start/Stop option in the Tools menu.

## Requirements

OBS 31 or higher

## Installation

### Windows

1. Download the Windows binaries from Releases on GitHub
2. Copy omtplugin.dll and libvmx.dll into your 64bit obs-plugins folder 
( usually C:\Program Files\obs-studio\obs-plugins\64bit )

### MacOS

1. Download the MacOS binaries from Releases on GitHub
2. Copy omtplugin.app from the download into the ~/Library/Application Support/obs-studio/plugins folder


