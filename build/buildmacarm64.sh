dotnet publish ../omtplugin.sln -r osx-arm64 -c Release
mkdir bin/Release/net8.0/osx-arm64/native/omtplugin.plugin
mkdir bin/Release/net8.0/osx-arm64/native/omtplugin.plugin/Contents
mkdir bin/Release/net8.0/osx-arm64/native/omtplugin.plugin/Contents/MacOS
rm bin/Release/net8.0/osx-arm64/native/omtplugin.plugin/Contents/MacOS/omtplugin
mv bin/Release/net8.0/osx-arm64/native/omtplugin.dylib bin/Release/net8.0/osx-arm64/native/omtplugin.plugin/Contents/MacOS/omtplugin