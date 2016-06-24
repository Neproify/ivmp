@echo off
FOR /F "tokens=* USEBACKQ" %%F IN (`git rev-list --count HEAD`) DO (
SET REVISION=r%%F
)
SET VERSION="0.4"
7z a IVMP-%VERSION%-%REVISION%-Server.zip ../bin/Lidgren.Network.dll
7z a IVMP-%VERSION%-%REVISION%-Server.zip ../bin/ivmp_server_core.exe
7z a IVMP-%VERSION%-%REVISION%-Server.zip ../bin/SharpDX.dll
7z a IVMP-%VERSION%-%REVISION%-Server.zip ../bin/SharpDX.Mathematics.dll
7z a IVMP-%VERSION%-%REVISION%-Server.zip ../bin/Jint.dll
7z a IVMP-%VERSION%-%REVISION%-Server.zip ./server/*