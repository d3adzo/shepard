SHEPARD /IN PROGRESS/

This is an /IN PROGRESS/ persistance tool using Windows Background Intelligent Transfer Service (BITS). 

Functionality: File Download, File Exfiltration, File Download + Persistent Execution

Usage: run shepard.exe with the following command line arguments
  
  -d remoteLocation, writePath: regular file download to a local path of your choice
  
  -e remoteLocation, localPath: regular file upload from a local path of your choice (only sends to IIS server, this is a limitation with BITS)
  
  -dr remoteLocation, writePath, [optionalFileArgs]:  file download to a path of your choice, and will attempt to maintain persitance. The downloaded file will
            attempt to run with optionalFileArgs and BITS will check back every 60 seconds to make sure the file is still on the compromised system.
  
Running this executable with no arguments or an incorrect amount of arguments will cause shepard to exit cleanly.
