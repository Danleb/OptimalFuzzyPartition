echo.
echo Post-build event start========================================================

set solutionPath=%1
set targetDir=%2
set targetFileName=%3
set targetName=%4

echo Solution path = %solutionPath%
echo Target dir = %targetDir%
echo Target file name = %targetFileName%
echo Target name = %targetName%

set targetPdbFileName=%targetName%.pdb
set netFrameworkTargetPdbPath=%targetDir%\..\net48\%targetPdbFileName%
echo PDB path = %netFrameworkTargetPdbPath%

set netFrameworkTargetPath=%targetDir%\..\net48\%targetFileName%
echo NetFramework4.8 target path = %netFrameworkTargetPath%

set unityDirPath=%solutionPath%\FuzzyPartitionUnityProject\Assets\Plugins\FuzzyPartitionAlgoithm\
echo Unity dir path = %unityDirPath%

xcopy %netFrameworkTargetPath% %unityDirPath% /Y
xcopy %netFrameworkTargetPdbPath% %unityDirPath% /Y

echo Post-build event end==========================================================
echo.
