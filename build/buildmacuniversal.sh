dotnet publish ../omtplugin.sln -r osx-arm64 -c Release
dotnet publish ../omtplugin.sln -r osx-x64 -c Release
lipo -create -output omtplugin.dylib ../bin/Release/net8.0/osx-x64/native/omtplugin.dylib ../bin/Release/net8.0/osx-arm64/native/omtplugin.dylib
install_name_tool -add_rpath "@loader_path/../Frameworks" omtplugin.dylib
mkdir omtplugin.plugin
mkdir omtplugin.plugin/Contents
mkdir omtplugin.plugin/Contents/MacOS
mkdir omtplugin.plugin/Contents/Frameworks
rm omtplugin.plugin/Contents/MacOS/omtplugin
mv omtplugin.dylib omtplugin.plugin/Contents/MacOS/omtplugin
cp ../../libvmx/build/libvmx.dylib omtplugin.plugin/Contents/Frameworks