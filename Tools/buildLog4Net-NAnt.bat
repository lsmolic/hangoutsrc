@echo off
echo Mono version 2.4.2.3 Build 3
echo Prepending 'C:\PROGRA~1\MONO-2~1.3\bin' to PATH
PATH=C:\PROGRA~1\MONO-2~1.3\bin;%PATH%


..\Vendor\nant-0.85\bin\NAnt.exe -buildfile:..\ClientToStateServerMono.build buildLog