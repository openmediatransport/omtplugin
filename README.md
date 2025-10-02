# Open Media Transport Plugin

omtplugin is a cross platform Open Media Transport plugin for OBS (https://obsproject.com)

## Download

https://github.com/openmediatransport/omtplugin/releases

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
2. Copy omtplugin.dll and libvmx.dll into the following folder:

```
C:\ProgramData\obs-studio\plugins\omtplugin\bin\64bit
```

This folder will need to be created if it does not already exist.

**Note:** Replace C: if Windows has been installed to a different drive.

### MacOS

1. Download the MacOS pkg from Releases on GitHub
2. Open the package and follow the instructions to install


