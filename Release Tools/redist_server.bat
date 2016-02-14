@echo off
FOR /F "tokens=* USEBACKQ" %%F IN (`git rev-list --count HEAD`) DO (
SET VERSION=r%%F
)
7z a IVMP-%VERSION%-Server.zip ../bin/Lidgren.Network.dll
7z a IVMP-%VERSION%-Server.zip ../bin/ivmp_server_core.exe