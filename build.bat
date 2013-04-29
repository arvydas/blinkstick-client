rem "c:\Program Files (x86)\Mono-2.11.2\bin\xbuild.bat" /p:Configuration=Release /verbosity:detailed /t:Clean BlinkStick.sln
rem "c:\Program Files (x86)\Mono-2.11.2\bin\xbuild.bat" /p:Configuration=Release /verbosity:detailed /t:Build BlinkStick.sln

rem "c:\Program Files (x86)\Xamarin Studio\bin\mdtool" -v build "--configuration:Release|x86" BlinkStick.sln
"c:\Program Files (x86)\Xamarin Studio\bin\mdtool" -v build "-c:Release|x86" BlinkStick.sln 

rem pause
