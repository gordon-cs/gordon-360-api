Write-Output "Entering build directory..."
Set-Location $env:TRAVIS_BUILD_DIR
Write-Output "Creating credential object to connect to deploy server..."
$cctpass = ConvertTo-SecureString $env:DEPLOY_PASSWORD -AsPlainText -Force
$credential = New-Object System.Management.Automation.PSCredential ($env:DEPLOY_USER, $cctpass)
Write-Output "Adding deploy server to trusted hosts file..."
Set-Item wsman:\localhost\client\trustedhosts "$env:DEPLOY_SERVER" -Force
Write-Output "Connecting to deploy server..."
try {
  $session = New-PSSession -ComputerName $env:DEPLOY_SERVER -Credential $credential -SessionOption (New-PSSessionOption -SkipCACheck -SkipCNCheck -SkipRevocationCheck)
  Write-Output "Entering deploy destination on remote server..."
  Invoke-Command -Session $session {Set-Location $($using:env:DEPLOY_DESTINATION)}
#   The SITE_DIR environment variable will be set according to the branch being deployed
  $apiFolderExists = (Invoke-Command -Session $session {Test-Path -Path ($($using:env:DEPLOY_DESTINATION) + "\" + $($using:env:SITE_DIR)) -PathType Container})
  if ($apiFolderExists) {
    Write-Output "Creating backup directory..."
    $backupDir = ($env:DEPLOY_DESTINATION + "\" + $env:SITE_DIR + "_backup_" + (Get-Date -Format "MM-dd-yyyy_HH-mm-ss"))
    Write-Output ("Backing up previous build...")
    # Piping to Out-Null suppresses output, which would otherwise reveal the api location on the console
    Invoke-Command -Session $session {mkdir $($using:backupDir) -Force | Out-Null}
    Invoke-Command -Session $session {Copy-Item ("./" + $($using:env:SITE_DIR) + "/*") -Destination $($using:backupDir) -Recurse -Force}
  }
  Write-Output "Debugging info..."
  Write-Output $($using:env:SITE_DIR)
  Write-Output $env:DEPLOY_DESTINATION
  Write-Output "Copying API files to remote destination..."
  Copy-Item -Path ("VSOutput\360ApiTrain" -Destination $env:DEPLOY_DESTINATION -ToSession $session -Recurse -Force
  Write-Output "Closing remote Powershell session..."
  $session | Remove-PSSession
  Write-Output "Done"
  exit 0
}
catch {
    Write-Output "Copy failed"
    exit 1
}
