FTPSync
=======

Synchronises remote FTP folders and files locally.
Keeps track of files so that it wont re-download them even when removed locally.

Quick Start Guide
====

1. Download the latest release.
2. Unzip to a seperate directory (Eg: C:\Program Files\FTPSync)
3. Open and edit FTPSync.exe.config and set each of the appsettings to match your desired configuration (host, port, username, password, destination, source)
4. Run FTPSync.exe and it will sync the directory.
5. Set up a Task schedule to run it at a required interval. 

Important note: Do not let it start a second instance if the first is still running as two running ftpsync's in the same destination are not supported.

A .ftpsync hidden directory will be created at the destination which will contain a list of synced files and a log incase of debugging.