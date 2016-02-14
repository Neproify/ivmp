@echo off
FOR /F "tokens=* USEBACKQ" %%F IN (`git rev-list --count HEAD`) DO (
SET VERSION=r%%F
)
echo f | xcopy /f "../bin/ScriptHookDotNet.dll" "../bin/ScriptHookDotNet.asi"
echo f | xcopy /f "../bin/ivmp_client_core.dll" "../bin/scripts/ivmp_client_core.net.dll"
7z a IVMP-%VERSION%-Client.zip ../bin/scripts
7z a IVMP-%VERSION%-Client.zip ../bin/Lidgren.Network.dll
7z a IVMP-%VERSION%-Client.zip ../bin/ScriptHook.dll
7z a IVMP-%VERSION%-Client.zip ../bin/ScriptHookDotNet.asi