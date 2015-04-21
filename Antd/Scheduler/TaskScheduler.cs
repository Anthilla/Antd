using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UbearCore.Scheduler {
    public class UbearJobs {
        public class Hello : IJob {
            public void Execute(IJobExecutionContext context) {
                Console.WriteLine("Greetings from HelloJob!");
            }
        }

        //public class Dumb : IJob {
        //    /// <summary>
        //    /// con dataMap si possono passare dei dati al Job
        //    /// </summary>
        //    /// <param name="context"></param>
        //    public void Execute(IJobExecutionContext context) {
        //        JobKey key = context.JobDetail.Key;
        //        JobDataMap dataMap = context.JobDetail.JobDataMap;
        //        string jobSays = dataMap.GetString("jobSays");
        //        float myFloatValue = dataMap.GetFloat("myFloatValue");
        //        Console.Error.WriteLine("Instance " + key + " of DumbJob says: " + jobSays + ", and val is: " + myFloatValue);
        //    }
        //}

        public class Command : IJob {
            public void Execute(IJobExecutionContext context) {
                JobKey key = context.JobDetail.Key;
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                string data0 = dataMap.GetString("data0");
                string data1 = dataMap.GetString("data1");
                Console.Error.WriteLine("Instance " + key + " of DumbJob says: \"" + data0 + "\" and \"" + data1 + "\"");
            }
        }
    }

    public class TaskScheduler {

        private static IScheduler __scheduler = StdSchedulerFactory.GetDefaultScheduler();

        public static void Start(bool _recoverTasks) {
            if (_recoverTasks == false) {
                __scheduler.Start();
            }
            else {
                __scheduler.Start();
                List<TaskModel> taskList = TaskRepository.GetAll();
                foreach (TaskModel task in taskList) {
                    LauchJob(
                        DefineJob<UbearJobs.Command>(
                            task.Guid,
                            new string[] { 
                                task.Data0,
                                task.Data1
                            }
                        ),
                        DefineTrigger(
                            task.Guid,
                            task.Interval
                            )
                        );
                }
            }
        }

        public static void Stop() {
            __scheduler.Shutdown();
        }

        public static void LauchJob(IJobDetail job, ITrigger trigger) {
            __scheduler.ScheduleJob(job, trigger);
        }

        public static IJobDetail DefineJob<T>(string _identity, string[] _jobData) where T : IJob {
            //todo
            //customize .UsingJobData()
            IJobDetail job = JobBuilder.Create<T>()
                .WithIdentity(_identity, Guid.NewGuid().ToString()) // name "_identity", group "Guid.NewGuid().ToString()"
                .UsingJobData("data0", _jobData[0])
                .UsingJobData("data1", _jobData[1])
                .Build();
            return job;
        }

        public static ITrigger DefineTrigger(string _identity, int _interval) {
            //todo
            //customize triggers' timers
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(_identity, Guid.NewGuid().ToString()) // name "_identity", group "Guid.NewGuid().ToString()"
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(_interval)
                    .RepeatForever())
                .Build();
            return trigger;
        }

        //public static void Test1() {
        //    dynamic[] data = new dynamic[] { 
        //        "key",
        //        "value"
        //    };
        //    LauchJob(
        //        DefineJob<UbearJobs.Hello>(new Tuple<string, string>("job1", "group1"), data),
        //        DefineTrigger(new Tuple<string, string>("trigger1", "group1"))
        //        );
        //}

        //public static void Test2() {
        //    string[] data = new string[] { 
        //        "primo valore",
        //        "secondo valore"
        //    };
        //    LauchJob(
        //        DefineJob<UbearJobs.Command>(new Tuple<string, string>("job2", "group2"), data),
        //        DefineTrigger(new Tuple<string, string>("trigger2", "group2"))
        //        );
        //}
    }
}
