# How to connect to a CS-RDSH virtual machine

## Table of Contents

- [Connect from Ubuntu Linux](#connect-from-ubuntu-linux)
- [Connect from Windows](#connect-from-windows)
- [Connect from MacOS](#connect-from-macos)

## Connect from Ubuntu Linux

Click on the icon in the panel at the top left of the screen and type "remote" in textbox that appears.  Click on "Remmina Remote Desktop Client."

The Remmina control window will open on your desktop and you'll see a new icon for it in the left-hand panel.  Right-click on the icon and select "Lock to Launcher".  From now on you can just click on the icon in the panel to start Remmina.

**Note:** As of 6/12/2018 CS-RDSH-01 does not have the necessary software so these instructions prefer CS-RDSH-02.

1. Click on "New" in the Remmina menu panel and enter the following:
    * Name: `CS-RDSH-02`
    * Group: _leave blank_
    * Protocol: `RDP - Remote Desktop Protocol`
    * Server: `cs-rdsh-02.gordon.edu`
    * User name: _use firstname.lastname form without @gordon.edu_
    * Password: _enter your Gordon password, or leave it blank, which would require you to enter this every time you connect_
    * Domain: `GORDON`
    * Resolution: `Use Client Resolution` 
    * Color Depth: change to `True color (24 bpp)`
2. Click on the "Advanced" tab and change the value of Quality to "Best (slowest)"
3. Click "Save" at the bottom of the window.
4. Double click on `CS-RDSH-02` to initiate a connection to the virtual machine.
5. If, upon start up, it asks you "Accept Certificate?", just refuse. 
6. You can close the Server Manager window that automatically opens each time you start the virtual machine.

If you were connecting in order to run the server locally, click [here](https://github.com/gordon-cs/gordon-360-api/blob/develop/README.md#running-the-server-locally) to return to those instructions.


## Connect from Windows

Ensure that you are connected to the Gordon network (either locally or through a VPN)

*Option 1: If you have RDC installed by default (might work better remotely)*
1. Open Remote Desktop Connection
   - Search "RDC" in the start menu and it will appear there
   - Note: Might be the Remote Desktop app, and not the Remote Desktop Connection app.
2. In the address bar (under "Computer" or "PC Name"), type in `CS-RDSH-02` (not case-sensitive).
3. To enter your Gordon credentials:
   - If this is your first time connecting:
     - Click "Connect." Your computer might try to use your local account by default. In that case you'll need to select "More choices" at the bottom of the dialog box and then enter your Gordon account details as `gordon.edu\firstname.lastname` and your account password.
   - If you have connected to the machine before:
     - Ensure that the listed account is the one you want to use. If it isn't, click "edit" and then follow the step above
4. Click "Connect"

*Option 2: If you have Remote Desktop installed by default (might work better on campus)*
1. Open Remote Desktop
   - Go to search bar, on the left side of the taskbar on your machine and type in 'Remote Desktop'.
   - Click on the Remote Desktop app. Not the Remote Desktop Connection app.
2. Add the PC
   - In the Remote Desktop window, click on 'Add' at the top right of the window.
   - Click on 'Desktop'.
   - Under 'PC name', type in `CS-RDSH-02` (not case-sensitive).
3. Optional: Enter credentials
   - Under 'User account', you may enter your Gordon username ('@gordon.edu' is not needed).
   - Under 'Password', enter your Gordon password.
   - 'Display name' is optional.
4. Run and connect
   - Click on the saved desktop you just created.
   - A pop-up will be shown asking 'Accept Certificate?'. You may check the one available option to never see the pop-up again and click on 'accept'.


## Connect from MacOS

(to be written)
