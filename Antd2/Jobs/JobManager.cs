using anthilla.core;
using System;
using System.Linq;
using System.Threading;

namespace Antd2.Jobs {

    public class JobManager {

        public void ExecuteAllJobs() {
            var jobs = GetAllTypesImplementingInterface(typeof(Job));
            if(jobs.Length == 0) {
                return;
            }
            Job instanceJob = null;
            Thread thread = null;
            for(var i = 0; i < jobs.Length; i++) {
                if(IsRealClass(jobs[i])) {
                    instanceJob = (Job)Activator.CreateInstance(jobs[i]);
                    thread = new Thread(new ThreadStart(instanceJob.ExecuteJob));
                    thread.Start();
                    ConsoleLogger.Log($"[scheduler] {instanceJob.Name} job started");
                }
            }
        }

        public void ExecuteJob<T>() where T :  new() {
            Job instanceJob = null;
            Thread thread = null;
            var type = typeof(T);
            if(IsRealClass(type)) {
                instanceJob = (Job)Activator.CreateInstance(type);
                thread = new Thread(new ThreadStart(instanceJob.ExecuteJob));
                thread.Start();
                ConsoleLogger.Log($"[scheduler] {instanceJob.Name} job started");
            }
        }

        private Type[] GetAllTypesImplementingInterface(Type desiredType) {
            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => desiredType.IsAssignableFrom(type))
                .ToArray();
        }

        public static bool IsRealClass(Type testType) {
            return testType.IsAbstract == false
                && testType.IsGenericTypeDefinition == false
                && testType.IsInterface == false;
        }
    }
}