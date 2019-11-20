using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Antd2.Jobs {

    public class Cron : ICron, IDisposable {

        public readonly IDictionary<string, CronJob> RunningJobs = new Dictionary<string, CronJob>();
     
        public void Add(string name, string command, int timeSpan) {
            if (RunningJobs.ContainsKey(name)) {
                Console.WriteLine($"[cron] a job with name {name} is already running");
                return;
            }
            var cronJob = new CronJob(name, command, timeSpan);
            var thread = new Thread(new ThreadStart(cronJob.ExecuteJob));
            thread.Name = name;
            thread.Start();
            RunningJobs[thread.Name] = cronJob;
            Console.WriteLine($"[cron] new {name} job started");
        }

        public void Pause(string jobThreadName) {
            if (RunningJobs.ContainsKey(jobThreadName)) {
                try {
                    Console.WriteLine($"[cron] pausing job {jobThreadName}");
                    RunningJobs[jobThreadName].Interrupted = true;
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
            else {
                Console.WriteLine($"[cron] no running job with name {jobThreadName}");
            }
        }

        public void Resume(string jobThreadName) {
            if (RunningJobs.ContainsKey(jobThreadName)) {
                try {
                    Console.WriteLine($"[cron] resuming job {jobThreadName}");
                    RunningJobs[jobThreadName].Interrupted = false;
                }
                catch (Exception ex) {
                    Console.WriteLine(ex.ToString());
                }
            }
            else {
                Console.WriteLine($"[cron] no running job with name {jobThreadName}");
            }
        }

        #region Do not modify!

        /// <summary>
        /// The volatile keyword ensures that the instantiation is complete
        /// before it can be accessed further helping with thread safety.
        /// </summary>
        private static volatile Cron _instance = null;
        private static readonly object _syncLock = new object();
        private readonly bool _disposed = false;

        /// <summary>
        /// Pattern 'double check locking'
        /// </summary>
        public static Cron Jobs {
            get {
                if (_instance != null)
                    return _instance;
                lock (_syncLock) {
                    if (_instance == null) {
                        _instance = new Cron();
                    }
                    return _instance;
                }
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private protected virtual void Dispose(bool disposing) {
            if (_disposed)
                return;
            if (disposing) {
                lock (_syncLock) {
                    _instance = null;
                }
            }
        }
        #endregion
    }

    public interface ICron {
        void Dispose();
    }
}