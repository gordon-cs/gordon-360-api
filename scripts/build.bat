REM Restore dependencies needed to build the solution
nuget restore Gordon360.sln

REM Execute MSBuild to build the solution
"C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\msbuild" Gordon360.sln

REM Publish the solution to a local directory
"C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\msbuild" Gordon360.sln -p:DeployOnBuild=true -p:PublishProfile=..\travis-publish-profiles\travis-dev.pubxml

echo "Testing ability to copy to remote server..."
@echo off
powershell -ExecutionPolicy Bypass -File ./scripts/copyFiles.ps1
