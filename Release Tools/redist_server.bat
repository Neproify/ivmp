@echo off
FOR /F "tokens=* USEBACKQ" %%F IN (`git rev-list --count HEAD`) DO (
SET REVISION=r%%F
)
SET VERSION="0.1"
7z a IVMP-%VERSION%-%REVISION%-Server.zip ../bin/Lidgren.Network.dll
7z a IVMP-%VERSION%-%REVISION%-Server.zip ../bin/ivmp_server_core.exe