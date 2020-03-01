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
  Invoke-Command -Session $session {mkdir {0} -Force $($using:env:TEST_COPY_DESTINATION)}
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
