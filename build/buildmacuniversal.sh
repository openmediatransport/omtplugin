dotnet publish ../omtplugin.sln -r osx-arm64 -c Release
dotnet publish ../omtplugin.sln -r osx-x64 -c Release
lipo -create -output omtplugin.dylib ../bin/Release/net8.0/osx-x64/native/omtplugin.dylib ../bin/Release/net8.0/osx-arm64/native/omtplugin.dylib
mkdir omtplugin.plugin
mkdir omtplugin.plugin/Contents
mkdir omtplugin.plugin/Contents/MacOS
rm omtplugin.plugin/Contents/MacOS/omtplugin
mv omtplugin.dylib omtplugin.plugin/Contents/MacOS/omtplugin