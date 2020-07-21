echo "Building application for deployment..."
set pubProfile = "..\travis-publish-profiles\%PUB_PROFILE%"
"C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\msbuild" Gordon360.sln -p:DeployOnBuild=true -p:PublishProfile=%pubProfile%

echo "Preparing to copy API to remote server..."
@echo off
powershell -ExecutionPolicy Bypass -File ./scripts/copyFiles.ps1
