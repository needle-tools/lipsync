echo OFF

:nextFile
   if "%~1" equ "" goto exitLoop
	 %~dp0/rhubarb/rhubarb --recognizer phonetic --output "%~n1.rhubarb" --exportFormat json "%~1"
   shift
   goto nextFile
:exitLoop

timeout 10