# How to connect to a CPS Server virtual machine

## Table of Contents
- [Setup Multifactor Authentication](#setup-multifactor-authentication)
- [Connecting from Off Campus](#connecting-from-off-campus)
- [Connect from Ubuntu Linux](#connect-from-ubuntu-linux)
- [Connect from Windows](#connect-from-windows)
- [Connect from MacOS](#connect-from-macos)

## Setup Multifactor Authentication

Follow [these instructions](https://cts.gordon.edu/knowledge/change-mfa/#to-change-your-default-security-verification-method) to update your default security verification method to "Notify me through app". This is necessary for you to be able to login to the VMs.

## Connecting from Off-Campus

If you are trying to connect from off-campus, follow [these](https://cts.gordon.edu/knowledge/gordon-vpn/) instructions to setup the VPN along with Microsoft Azure Multifactor Authentication. After everything is set up with the VPN, you will be able to follow the instructions in the rest of this document without problems.


## Connect to the VM

Go to [desktops.gordon.edu](https://desktops.gordon.edu/) and sign-in with your Gordon account. Select either CPS Server 1 or CPS Server 2 to download the remote desktop profile to connect to that server. The servers are the same, but they have separate resources, so choose the one that fewer people are currently using.

Double click the downloaded link to open the connection to the remote desktop. Sign in with your Gordon account and accept the MFA prompt in the Microsoft Authenticator app. If you don't have a remote desktop client installed, see below.

## Install Remote Desktop

### Windows

If you are on Windows, Remote Desktop is installed by default. You don't need to do anything else.

### MacOS

Install the [Microsoft Remote Desktop](https://apps.apple.com/us/app/microsoft-remote-desktop/id1295203466) client from the Mac App Store.

### Linux

Install an appropriate remote desktop client. For Ubuntu, Remmina Remote Desktop Client should already be installed.
