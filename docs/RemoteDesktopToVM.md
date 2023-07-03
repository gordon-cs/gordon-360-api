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

## Install Remote Desktop

### Windows

If you are on Windows, Remote Desktop is installed by default. You don't need to do anything else.

### MacOS

Install the [Microsoft Remote Desktop](https://apps.apple.com/us/app/microsoft-remote-desktop/id1295203466) client from the Mac App Store.

### Linux

Install an appropriate remote desktop client. For Ubuntu, Remmina Remote Desktop Client should already be installed.
## Connect to the VM

Go to [desktops.gordon.edu](https://desktops.gordon.edu/) and sign-in with your Gordon account. Select either CPS Server 1 or CPS Server 2 to download the remote desktop profile to connect to that server. The servers are the same, but they have separate resources, so choose the one that fewer people are currently using.

### Windows or MacOS

Double click the downloaded link to open the connection to the remote desktop.  Continue below with [Authentication].(#authentication)

### Linux/Remmina

If using Remmina on Linux, use the following steps to configure the RDP connection, otherwise skip to [Other Systems] (#other-systems).

1. Click on the "Show Applications" button at the bottom left of the screen and type "remmina" in the search bar.  Right-click on the Remmina icon and select "Add to Favorites" then left-click to open the app.

1. Open the file browser and navigate the Downloads folder drag the `cpub-CPS_Server_1-CPS_Server_1-CmsRdsh.rdp` file (or the corresponding "2" file if you chose that machine) to the Remmina app window.  This will create an entry named `RD-BROKER.GORDON.EDU`.

1. Right-click on `RD-BROKER.GORDON.EDU` entry and select "Edit"  If desired, you can change the name for this connection.

1. Under the "Basic" tab enter your username, password (optional), and domain (both `GORDON` and `gordon.edu` work).  For Resolution, select "Use client resolution"

1. Select the "Advanced" tab.  Feel free to change the quality of the connection then scroll down to the very bottom to find multiple checkboxes and make the following changes:
   - Deselect "Share Printers"
   - Select "Server detection using Remote Desktop Gateway"
   - If present, select "Use base credentials for gateway too"
   - Select "Ignore certificate"

1. Click "Save and Connect" and continue 

## Authentication

Sign in with your Gordon account and accept the MFA prompt in the Microsoft Authenticator app.  Note: When using Remmina, you may be prompted for your password twice before the MFA prompt is sent.

