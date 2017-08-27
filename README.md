FTPSync
=======

This script performs a one way synchronization between a remote FTP folder and a local folder.
It's purpose is to keep track of files that are downloaded so that it wont re-download them even when they are removed locally.

Quick Start Guide
====

1. Download the powershell script and place it in a folder with write access (For the log an persistance file).
2. Run the command with the supplied parameters.

If you want to run it from a batch file, or a scheduled windows task you can make a .bat file and put the following:
Powershell.exe -executionpolicy remotesigned -File ftpsync.ps1 -DestinationDirectory "C:\Downloads" -uri "ftp://ftp.myserver.com" -Dir "/downloads"

By default a log.txt file will be created on each process.
If onlyDownloadNew parameter is used a .prev.txt file will be created to keep track of which files have been downloaded.

History
====
This used to be a stand alone exectuable however it made more sense to re-write it as a powershell script.