@echo off

:: Execute the script "only" with the first "fsianycpu.exe" found
set FSHARPSDK=^
C:\Program Files (x86)\Microsoft SDKs\F#\4.1\Framework\v4.0\;^
C:\Program Files (x86)\Microsoft SDKs\F#\3.1\Framework\v4.0\;^
C:\Program Files (x86)\Microsoft SDKs\F#\3.0\Framework\v4.0\

:: Execute this script from the root (.SLN file) and point to folder you want
:: to add to the solution as folder(s)/files(s).

for %%i in (fsianycpu.exe) do (
	"%%~$FSHARPSDK:i" ^
		SM.JavaToVisualStudioSolution.fsx ^
		"Examples\Digst.OioIdws.Java\certs" ^
		%* > _certs.txt
)
for %%i in (fsianycpu.exe) do (
	"%%~$FSHARPSDK:i" ^
		SM.JavaToVisualStudioSolution.fsx ^
		"Examples\Digst.OioIdws.Java\libs" ^
		%* > _libs.txt
)
for %%i in (fsianycpu.exe) do (
	"%%~$FSHARPSDK:i" ^
		SM.JavaToVisualStudioSolution.fsx ^
		"Examples\Digst.OioIdws.Java\system-user-scenario-hok" ^
		%* > _system-user-scenario-hok.txt
)
for %%i in (fsianycpu.exe) do (
	"%%~$FSHARPSDK:i" ^
		SM.JavaToVisualStudioSolution.fsx ^
		"Examples\Digst.OioIdws.Java\service-hok" ^
		%* > _service-hok.txt
)