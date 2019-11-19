using Antd2.cmds;
using System;
using System.Threading;

namespace Antd2.Jobs {

    public class CronJob {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="command"></param>
        /// <param name="timeSpan">milliseconds</param>
        public CronJob(string name, string command, int timeSpan) {
            Name = name;
            Command = command;
            RepetitionIntervalTime = timeSpan;
        }

        public string Name { get; private set; }
        public string Command { get; private set; }
        public int RepetitionIntervalTime { get; private set; }
        public bool Interrupted { get; set; }

        public void ExecuteJob() {
            while (true) {
                DoJob();
                Thread.Sleep(RepetitionIntervalTime);
            }
        }

        private void DoJob() {
            if (!Interrupted) {
                Console.WriteLine($"[cron] exec {Command}");
                Bash.Do(Command);
            }
        }
    }
}