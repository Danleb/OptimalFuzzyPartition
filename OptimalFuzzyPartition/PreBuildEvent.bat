echo.
echo Pre-build event start========================================================
echo Current dir = %cd%

set projectPath=%1
echo Project path =  %projectPath%

set resourcesFolder=%projectPath%Resources\
cd %resourcesFolder%

set scriptPath=%projectPath%Resources\ResX2Xaml.py
echo Python script path = %scriptPath%

Rem Comment the >NUL to get all localization string in output.
python %scriptPath% >NUL

echo Pre-build event end==========================================================
echo.
