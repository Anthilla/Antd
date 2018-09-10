using anthilla.scheduler;
using Antd.cmds;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MQTTnet.Core;
using anthilla.core.Serialization;
using System;
using Antd.models;
using anthilla.core;

namespace Antd {
    public class SendInfoToCloudJob : Job {

        #region [    Core Parameter    ]
        private bool _isRepeatable = true;

        public override bool IsRepeatable {
            get {
                return _isRepeatable;
            }
            set {
                value = _isRepeatable;
            }
        }

        private int _repetitionIntervalTime = 15000;

        public override int RepetitionIntervalTime {
            get {
                return _repetitionIntervalTime;
            }

            set {
                value = _repetitionIntervalTime;
            }
        }

        public override string Name {
            get {
                return GetType().Name;
            }

            set {
                value = GetType().Name;
            }
        }
        #endregion

        public override void DoJob() {
            DoJobAsync().GetAwaiter().GetResult();
        }

        static async Task DoJobAsync() {
            if(Application.MQTTCLIENT.IsConnected == false) {
                ConsoleLogger.Log($"[mqtt] mqtt server is unreachable");
                return;
            }
            var servicesNames = new List<string>();
            if(Directory.Exists(Const.AntdUnits)) {
                var antdUnits = Directory.EnumerateFiles(Const.AntdUnits).ToArray();
                for(var i = 0; i < antdUnits.Length; i++) {
                    servicesNames.Add(Path.GetFileName(antdUnits[i]));
                }
            }
            if(Directory.Exists(Const.ApplicativeUnits)) {
                var applicativeUnits = Directory.EnumerateFiles(Const.ApplicativeUnits).ToArray();
                for(var i = 0; i < applicativeUnits.Length; i++) {
                    servicesNames.Add(Path.GetFileName(applicativeUnits[i]));
                }
            }
            var arrServicesNames = servicesNames.ToArray();
            var services = new ServiceStatus[arrServicesNames.Length];
            for(var i = 0; i < arrServicesNames.Length; i++) {
                services[i] = new ServiceStatus {
                    Name = arrServicesNames[i],
                    IsActive = cmds.Systemctl.IsActive(arrServicesNames[i])
                };
            }

            var du = DiskUsage.Get();
            var dus = new DiskUsageStatus[du.Length];
            for(var i = 0; i < du.Length; i++) {
                dus[i] = new DiskUsageStatus {
                    Device = du[i].MountedOn,
                    Used = ParseInt(du[i].UsePercentage.Replace("%", ""))
                };
            }

            var free = Free.Get();
            var frees = new FreeStatus[free.Length];
            for(var i = 0; i < free.Length; i++) {
                frees[i] = new FreeStatus() {
                    Name = free[i].Name,
                    Total = ParseInt(free[i].Total),
                    Used = ParseInt(free[i].Used),
                    Free = ParseInt(free[i].Free),
                    Shared = ParseInt(free[i].Shared),
                    BuffCache = ParseInt(free[i].BuffCache),
                    Available = ParseInt(free[i].Available)
                };
            }

            if(frees.Length == 0 && du.Length == 0) {
                ConsoleLogger.Log($"[mqtt] mqtt server is unreachable");
                return;
            }

            var status = new CurrentMachineStatus() {
                PublicIp = PublicIp.Get(),
                MachineId = Application.MACHINE_ID,
                Date = new DateStatus {
                    Date1 = DateTime.Now,
                    Date2 = Date.Get()
                },
                Free = frees,
                DiskUsage = dus,
                Services = services
            };
            var converted = MSG.Serialize(status);
            var message = new MqttApplicationMessageBuilder()
             .WithTopic("/status")
             .WithPayload(converted)
             .WithExactlyOnceQoS()
             .WithRetainFlag()
             .Build();
            await Application.MQTTCLIENT.PublishAsync(message);
            //WriteStatus(status);
        }

        private static int ParseInt(string value) {
            try {
                return int.Parse(value);
            }
            catch(Exception) {
                return 0;
            }
        }

        //private static void WriteStatus(Status status) {
        //    Console.WriteLine($"+=======================================+");
        //    Console.WriteLine($"uid\t\t{status.MachineId.MachineUid}");
        //    Console.WriteLine($"sn\t\t{status.MachineId.SerialNumber}");
        //    Console.WriteLine($"pn\t\t{status.MachineId.PartNumber}");
        //    Console.WriteLine($"ip\t\t{status.PublicIp}");
        //    for(var i = 0; i < status.Free.Length; i++) {
        //        Console.WriteLine($"{status.Free[i].Name}\t\t{status.Free[i].Total}\t{status.Free[i].Free}\t{status.Free[i].Used}\t{status.Free[i].Shared}\t{status.Free[i].BuffCache}\t{status.Free[i].Available}");
        //    }
        //    Console.WriteLine();
        //    for(var i = 0; i < status.DiskUsage.Length; i++) {
        //        Console.WriteLine($"{status.DiskUsage[i].Device}\t\t\t\t\t{status.DiskUsage[i].Used}");
        //    }
        //    Console.WriteLine($"date\t\t{status.Date.Date1}\t\t{status.Date.Date2}");
        //    for(var i = 0; i < status.Services.Length; i++) {
        //        Console.WriteLine($"{status.Services[i].Name}\t\t\t\t\t{status.Services[i].IsActive}");
        //    }
        //    Console.WriteLine($"+=======================================+");
        //    Console.WriteLine();
        //}
    }
}
