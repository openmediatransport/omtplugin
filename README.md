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
2. Copy omtplugin.dll into your 64bit obs-plugins folder 
( usually C:\Program Files\obs-studio\obs-plugins\64bit )
3. Copy libvmx.dll into your obs 64bit bin folder
( usually C:\Program Files\obs-studio\bin\64bit )

### MacOS

1. Download the MacOS binaries from Releases on GitHub
2. Go to Applications in Finder and right click OBS.app and select Show Package Contents
3. Copy omtplugin.app from the download into the Contents/Plugins folder
4. Copy libvmx.dylib from the download into the Contents/MacOS folder

