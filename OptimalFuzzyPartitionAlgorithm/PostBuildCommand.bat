echo "Post-build event start========================================================"
echo %1
echo %2
echo %3
echo %4

set solutionPath=%1
set targetDir=%2
set targetFileName=%3
set targetName=%4

set targetPdbFileName="%targetName%.pdb"
set netFrameworkTargetPdbPath="%targetDir%\..\net48\%targetPdbFileName%"
echo %netFrameworkTargetPdbPath%

set netFrameworkTargetPath="%targetDir%\..\net48\%targetFileName%"
echo %netFrameworkTargetPath%

set unityDirPath="%1%\FuzzyPartitionUnityProject\Assets\Plugins\FuzzyPartitionAlgoithm\"
echo %unityDirPath%

xcopy %netFrameworkTargetPath% %unityDirPath% /Y
xcopy %netFrameworkTargetPdbPath% %unityDirPath% /Y