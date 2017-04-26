using System;
using System.Collections.Generic;
using System.IO;
using antdlib.common;
using antdlib.models;
using anthilla.commands;
using Newtonsoft.Json;

namespace antdlib.config {
    public class Host2Configuration {
        public string FilePath { get; }
        public string FilePathBackup { get; }
        public Host2Model Host { get; private set; }

        private readonly CommandLauncher _commandLauncher = new CommandLauncher();

        public Host2Configuration() {
            FilePath = $"{Parameter.AntdCfg}/localhost.conf";
            FilePathBackup = $"{Parameter.AntdCfg}/localhost.conf.bck";
            Host = LoadModel();
        }

        private Host2Model LoadModel() {
            if(!File.Exists(FilePath)) {
                return new Host2Model();
            }
            try {
                return JsonConvert.DeserializeObject<Host2Model>(File.ReadAllText(FilePath));
            }
            catch(Exception) {
                return new Host2Model();
            }
        }

        public void Export(Host2Model model) {
            if(File.Exists(FilePath)) {
                File.Copy(FilePath, $"{FilePath}.bck", true);
            }
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(model, Formatting.Indented));
        }

        public void ApplySettings() {
            _commandLauncher.Launch("set-hostname", new Dictionary<string, string> {
                { "$host_name", Host.HostName }
            });
            _commandLauncher.Launch("set-chassis", new Dictionary<string, string> {
                { "$host_chassis", Host.HostChassis }
            });
            _commandLauncher.Launch("set-deployment", new Dictionary<string, string> {
                { "$host_deployment", Host.HostDeployment }
            });
            _commandLauncher.Launch("set-location", new Dictionary<string, string> {
                { "$host_location", Host.HostLocation }
            });
            File.WriteAllText("/etc/hostname", Host.HostName);
            _commandLauncher.Launch("set-timezone", new Dictionary<string, string> {
                { "$host_timezone", Host.Timezone }
            });
        }
    }
}
