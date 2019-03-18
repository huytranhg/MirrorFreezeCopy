# MirrorFreezeCopy
MirrorFreezeCopy Windows Service

MirrorFreezeCopy's built can be downloaded from this link: https://1drv.ms/u/s!AoP1RrY3fZ23eHjvU00JTqWUm5s

***Prerequisites***
- MirrorFreezeCopy can be run on Windows 7 or higher version of Windows OS.
- It requires Microsoft .NET Framework 4.5 or higher version to be run.
- Official link to download .NET framework 4.5.2 offline installer: https://www.microsoft.com/en-us/download/details.aspx?id=42642

***Features***
- MirrorFreezeCopy is a Windows Service, running in background, and requires Administrator account to install.
- MirrorFreezeCopy has three functions:
+ Mirror, meaning mirror copy from Source folder to Destination folder whenever Source folder has changed (deletion is not considered a change).
+ Freeze, meaning mirror copy from Destination folder to Source folder whenever Source folder has changed (deletion is not considered a change).
+ Copy, meaning copy only, from Source folder to Destination folder whenever Source folder has changed (deletion is not considered a change).
- The mirror copy makes an exact 1:1 copy of the two folders, the non-existed files and folders in Source folder will be deleted in Destination folder, so use Mirror and Freeze functions at your own risk.
- The service works with shared folder.

***Installation, Uninstallation***
  
  Install this Windows Service by using below script (Run as Administrator), with absolute path, where the MirrorFreezeCopy.WindowsService.exe file was saved:
	
	"<Absolute Path of Folder that contains MirrorFreezeCopy.WindowsService.exe>\MirrorFreezeCopy.WindowsService.exe" --install
	net start "MirrorFreezeCopy"
	pause
  Upon installing, Windows will ask for username and password of an account, administrator can use any local account having password (as this is Windows's default policy), with a ./ adding before username, like in below screenshot:
  https://1drv.ms/u/s!AoP1RrY3fZ23dyyjvPuhRxYefkQ
  
  Uninstall this Windows Service by using below script (Run as Administrator), with absolute path, where the MirrorFreezeCopy.WindowsService.exe file was saved:
	
	"<Absolute Path of Folder that contains MirrorFreezeCopy.WindowsService.exe>\MirrorFreezeCopy.WindowsService.exe" --uninstall
	powershell remove-eventlog "MirrorFreezeCopyLog"
	pause
	
	
***Configuration File and Log Files***

  After successfully installed, configuration file named MirrorFreezeCopy_Config.xml can be found at folder (note that Windows drive letter can be different):

	C:\ProgramData\MirrorFreezeCopy
	
  This configuration file can be modified by using any text editor, for example Windows's Notepad.
  
  Log files can be found at folder (note that Windows drive letter can be different):
  
	C:\ProgramData\MirrorFreezeCopy\Logs
	
  The log files can be viewed by using any text editor, for example Windows's Notepad.
	
  After changing configuration file, to take effects, users can restart Windows, Sign out and Sign in again, or restart MirrorFreezeCopy Windows Service by using below scripts (Run as Administrator):
	
	net stop "MirrorFreezeCopy"
	net start "MirrorFreezeCopy"
	pause

***Notes***

- MirrorFreezeCopy is designed for personal use only.
- It hasn't been tested under extreme conditions, for example, the Source folder is shared and changed by many users, or by other computer programs.
-	Under extreme conditions, it may show malfunctions, or decreasing in performance.
-	The mirror copy makes an exact 1:1 copy of the two folders, the non-existed files and folders in Source folder will be deleted in Destination folder, so use Mirror and Freeze functions at your own risk.
-	I didn't imagine the Freeze function, it is used to freeze the config folder of Youtube Downloader HD https://www.youtubedownloaderhd.com, which normally is C:\Users\Username\AppData\Roaming\Youtube Downloader HD
- The Interval node of configuration file is in seconds.
- In case the folder size, or number of files, from Windows GUI (Properties) doesn't tally with MirrorFreezeCopy's log, please use a 3rd-party program like WinDirStat https://windirstat.net, or RidNacs https://www.splashsoft.de/ridnacs-disk-space-usage-analyzer to compare with MirrorFreezeCopy's log, since these programs's statistic does tally with MirrorFreezeCopy's log.
- The startup project is MirrorFreezeCopy.WindowsService.
	
***Source Code***
- Source code of this program can be found at github: https://github.com/huytranhg/MirrorFreezeCopy
- Visual Studio 2017 Community were used to develop this program.

---Developed and documented by Huy Tran---
