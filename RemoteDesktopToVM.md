# How to connect to a CS-RDSH virtual machine

## Table of Contents

- [Connect from Ubuntu Linux](connect-from-ubuntu-linux)
- [Connect from Windows](connect-from-windows)
- [Connect from MacOS](connect-from-macos)

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

1. Go to search bar, on the left side of the taskbar on your machine and type in 'Remote Desktop'.
2. Click on the Remote Desktop app. Not the Remote Desktop Connection app.
3. In the Remote Desktop window, click on 'Add' at the top right of the window.
4. Click on 'Desktop'.
5. Under 'PC name', type in 'CS-RDSH-02' (not case-sensitive).
6. Steps 7 through 9 are optional.
7. Under 'User account', you may enter your Gordon username ('@gordon.edu' is not needed).
8. Under 'Password', enter your Gordon password.
9. 'Display name' is optional.
10. Click on the saved desktop you just created.
11. A pop-up will be shown asking 'Accept Certificate?'. You may check the one available option to never see the pop-up again and click     on 'accept'.

## Connect from MacOS

(to be written)
