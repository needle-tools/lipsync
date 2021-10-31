echo OFF

:nextFile
   if "%~1" equ "" goto exitLoop
	 %~dp0/rhubarb/rhubarb --recognizer phonetic --dialogFile "%~n1.txt" --output "%~n1.rhubarb" --exportFormat json "%~1"
   shift
   goto nextFile
:exitLoop

timeout 10