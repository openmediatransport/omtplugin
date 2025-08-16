# Open Media Transport plugin for OBS

omtplugin is a cross platform Open Media Transport plugin for OBS

## Features

Send and receive high quality audio/video between OBS and other OMT compatible devices over a local network!

For more information about Open Media Transport in general, see the README here:
https://github.com/openmediatransport/

### OMT Source

Receive audio/video from OMT sources on the network.

Supports HDR workflows using the Color Space dropdown in the OMT Source properties.

### OMT Output

Send audio/video from the main OBS output.

This can be enabled from the Tools - OMT Output Settings menu.

This supports HDR OBS workflows and will send 10bit+ when P010 or P216 is enabled in OBS settings.

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


