using System.Threading;

namespace Antd2.Jobs {

    public abstract class Job {

        public void ExecuteJob() {
            if(IsRepeatable) {
                while(true) {
                    DoJob();
                    Thread.Sleep(RepetitionIntervalTime);
                }
            }
            else {
                DoJob();
            }
        }

        public virtual object GetParameters() {
            return null;
        }

        public abstract bool IsRepeatable { get; set; }

        public abstract int RepetitionIntervalTime { get; set; }

        public abstract string Name { get; set; }

        public abstract void DoJob();
    }
}