echo "Build directory:"
echo %TRAVIS_BUILD_DIR%

REM Restore dependencies needed to build the solution
nuget restore Gordon360.sln

REM Execute MSBuild to build the solution
"C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\msbuild" Gordon360.sln

echo "Testing ability to copy to remote server..."
@ECHO off
powershell -ExecutionPolicy Bypass -File ./scripts/copyFiles.ps1
