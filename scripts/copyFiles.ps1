echo $env:TEST_COPY_DESTINATION
echo "Entering build directory..."
cd $env:TRAVIS_BUILD_DIR
echo "Creating credential object..."
$cctpass = ConvertTo-SecureString $env:DEPLOY_PASSWORD -AsPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential ($env:DEPLOY_PASSWORD, $cctpass)
echo "Adding deployment server to trusted hosts file..."
$trustedHosts = (Get-Item WSMan:\localhost\Client\TrustedHosts).Value
Set-Item wsman:\localhost\client\trustedhosts "$env:DEPLOY_SERVER" -Force
echo "Opening remote session..."
$session = New-PSSession -ComputerName $env:DEPLOY-_SERVER -Credential $credential -SessionOption (New-PSSessionOption -SkipCACheck -SkipCNCheck -SkipRevocationCheck)
echo "Copying files to remote destination..."
cp -Path .\* -Destination $env:TEST_COPY_DESTINATION -ToSession $session
echo "Closing remote Powershell session..."
$session | Remove-PSSession
echo "Done"
