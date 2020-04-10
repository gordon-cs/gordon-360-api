echo $env:TEST_COPY_DESTINATION
echo "Entering build directory..."
cd $env:TRAVIS_BUILD_DIR
echo "Creating credential object..."
$cctpass = ConvertTo-SecureString $env:DEPLOY_PASSWORD -AsPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential ($env:DEPLOY_USER, $cctpass)
echo "Adding deployment server to trusted hosts file..."
$trustedHosts = (Get-Item WSMan:\localhost\Client\TrustedHosts).Value
Set-Item wsman:\localhost\client\trustedhosts "$env:DEPLOY_SERVER" -Force
echo "Opening remote session..."
try {
  $session = New-PSSession -ComputerName $env:DEPLOY_SERVER -Credential $credential -SessionOption (New-PSSessionOption -SkipCACheck -SkipCNCheck -SkipRevocationCheck)
  echo "Creating copytest folder..."
  Invoke-Command -Session $session {mkdir $($using:env:TEST_COPY_DESTINATION) -Force | Out-Null}
  Invoke-Command -Session $session {cd $($using:env:TEST_COPY_DESTINATION)}
  $apiFolderExists = (Invoke-Command -Session $session {Test-Path -Path ($using:env:TEST_COPY_DESTINATION + "\360ApiTrain") -PathType Container})
  if ($apiFolderExists) {
    echo "Creating backup directory..."
    $renamedDirName = ($env:TEST_COPY_DESTINATION + "\360ApiTrain_backup_" + (Get-Date -Format "MM-dd-yyyy_HH-mm-ss"))
    echo ("Backing up previous build...")
    Invoke-Command -Session $session {mkdir $($using:renamedDirName) -Force | Out-Null}
    Invoke-Command -Session $session {Copy-Item ./360ApiTrain/* -Destination $($using:renamedDirName) -Recurse -Force}
  }
  echo "Copying files to remote destination..."
  cp -Path "VSOutput\360ApiTrain" -Destination $env:TEST_COPY_DESTINATION -ToSession $session -Recurse -Force
  echo "Closing remote Powershell session..."
  $session | Remove-PSSession
  echo "Done"
  exit 0
}
catch {
    echo "Copy failed"
    exit 1
}
