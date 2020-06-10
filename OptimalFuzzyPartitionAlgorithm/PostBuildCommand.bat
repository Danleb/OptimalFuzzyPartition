echo "Post-build event start"
echo %1
echo %2
echo %3
set solutionPath=%1
set targetDir=%2
set targetFileName=%3
set netFrameworkTargetPath="%targetDir%\..\net48\%targetFileName%"
echo %netFrameworkTargetPath%
set unityDirPath="%1%\FuzzyPartitionUnityProject\Assets\Plugins\FuzzyPartitionAlgoithm\"
echo %unityDirPath%
xcopy %netFrameworkTargetPath% %unityDirPath% /Y