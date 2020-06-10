echo "Post-build event start"
echo %1
echo %2
set solutionPath=%1
set targetPath=%2
set unityDirPath="%1%\FuzzyPartitionUnityProject\Assets\Plugins\FuzzyPartitionAlgoithm\"
echo %unityDirPath%
xcopy %targetPath% %unityDirPath% /Y