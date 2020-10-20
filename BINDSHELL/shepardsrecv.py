import socket, sys

host = sys.argv[1]
port = 6006

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
try:
    s.connect((host, port))
except:
    print('could not connect to ' + host + ' on ' + port)
    exit()
print("Connected on port: " + str(port))
nextcmd = 'whoami'
s.sendall((nextcmd + '\r\n').encode())

while True:
    data = s.recv(4096)
    while 'EOFX' not in data.decode():
        data += s.recv(4096)
    datarr = data.decode().split('\r\n')
    for line in datarr[:-3]:
        print(line)
    print("Current path: " + datarr[-3])
    nextcmd = input("%SBS% ")
    if nextcmd == 'quit':
        nextcmd += '\r\n'
        s.sendall(nextcmd.encode())
        s.close()
        break
    else:
        nextcmd += '\r\n'
        s.sendall(nextcmd.encode())
