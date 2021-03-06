@echo off
FOR /F "tokens=* USEBACKQ" %%F IN (`git rev-list --count HEAD`) DO (
SET REVISION=r%%F
)
SET VERSION="0.5"
echo f | xcopy /f "../bin/ScriptHookDotNet.dll" "../bin/ScriptHookDotNet.asi"
echo f | xcopy /f "../bin/ivmp_client_core.dll" "../bin/scripts/ivmp_client_core.net.dll"
echo f | xcopy /f "../client/scriptimg/script.img" "../bin/common/data/cdimages/script.img"
7z a IVMP-%VERSION%-%REVISION%-Client.zip ../bin/scripts
7z a IVMP-%VERSION%-%REVISION%-Client.zip ../bin/Lidgren.Network.dll
7z a IVMP-%VERSION%-%REVISION%-Client.zip ../bin/SharpDX.dll
7z a IVMP-%VERSION%-%REVISION%-Client.zip ../bin/SharpDX.Mathematics.dll
7z a IVMP-%VERSION%-%REVISION%-Client.zip ../bin/Jint.dll
7z a IVMP-%VERSION%-%REVISION%-Client.zip ../bin/ScriptHook.dll
7z a IVMP-%VERSION%-%REVISION%-Client.zip ../bin/ScriptHookDotNet.asi
7z a IVMP-%VERSION%-%REVISION%-Client.zip ../bin/common