
using System;
using System.Threading.Tasks;
using BITS = BITSReference1_5;

namespace shepard
{
    class Program
    {
        
        private void setup()
        {
            Console.WriteLine("test message");
        }

        public static void Main(string[] args)
        {
            if (args.Length == 0) { return; }

            var mgr = new BITS.BackgroundCopyManager1_5();
            BITS.GUID jobGuid;
            BITS.IBackgroundCopyJob job;
            mgr.CreateJob("Quick download", BITS.BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out jobGuid, out job);
            job.AddFile("https://aka.ms/WinServ16/StndPDF", @"C:\Users\enzod\Desktop\Server2016.pdf");
            //
            //string inputaddr = "23.52.161.19";
            //System.Net.IPHostEntry hostEntry = System.Net.Dns.GetHostEntry(inputaddr);
            // System.Console.WriteLine(hostEntry.HostName);
            //job.AddFile(hostEntry.HostName, @"C:\Users\enzod\Desktop\Server2016.pdf");
            job.Resume();
            bool jobIsFinal = false;
            while (!jobIsFinal)
            {
                BITS.BG_JOB_STATE state;
                job.GetState(out state);
                switch (state)
                {
                    case BITS.BG_JOB_STATE.BG_JOB_STATE_ERROR:
                    case BITS.BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED:
                        job.Complete();
                        break;

                    case BITS.BG_JOB_STATE.BG_JOB_STATE_CANCELLED:
                    case BITS.BG_JOB_STATE.BG_JOB_STATE_ACKNOWLEDGED:
                        jobIsFinal = true;
                        break;
                    default:
                        Task.Delay(500); // delay a little bit
                        break;
                }
            }
            Console.ReadLine(); //pause execution of cmd window, enter to close
            // Job is complete
        }

        private void downloadFile()
        {
            Console.WriteLine("test message");
        }

        private void exfiltrateFile()
        {
            Console.WriteLine("test message");
        }
        //exfiltrate method
        //download method
        //init setup method called at the start of main
        //somewhere in main, consisent loop for input of command

    }
}
// Job is complete
