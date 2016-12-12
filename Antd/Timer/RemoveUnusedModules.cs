using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using antd.commands;
using antdlib.common.Models;
using antdlib.common;
using Antd.Host;

namespace Antd.Timer {
    public class RemoveUnusedModules {
        public System.Threading.Timer Timer { get; private set; }
        private static readonly MapToModel Mapper = new MapToModel();

        public void Start(TimeSpan alertTime) {
            var current = DateTime.Now;
            var timeToGo = alertTime - current.TimeOfDay;
            if(timeToGo < TimeSpan.Zero) {
                return;
            }
            Timer = new System.Threading.Timer(x => {
                Action();
            }, null, timeToGo, Timeout.InfiniteTimeSpan);
        }

        public void Stop() {
            Timer?.Dispose();
        }

        private static void Action() {
            var lsmod = Mapper.FromCommand<ModuleModel>("lsmod").ToList().Skip(1).ToList();
            if(!lsmod.Any())
                return;
            var launcher = new CommandLauncher();
            launcher.Launch("rmmod", new Dictionary<string, string> { { "$modules", lsmod.Select(_ => _.Name).JoinToString(" ") } });
            var hostConfiguration = new HostConfiguration();
            hostConfiguration.ApplyHostModprobes();
        }
    }
}
