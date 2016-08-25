using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.Systemd;
using Antd.Configuration;
using Newtonsoft.Json;

namespace Antd.Gluster {
    public class GlusterConfiguration {

        private static readonly string FilePath = $"{Parameter.RepoConfig}/gluster.conf";
        private static readonly string ServiceName = "glusterd.service";

        public static void Set() {
            Directory.CreateDirectory(Parameter.RepoConfig);
            if (!File.Exists(FilePath)) {
                ConsoleLogger.Log("gluster configuration file does not exist");
                var tempFlow = new GlusterSetup {
                    Name = ".gluster.conf",
                    Path = $"{Parameter.RepoConfig}/.gluster.conf",
                    Nodes = new List<string> { "server01", "server02" },
                    Volumes = new List<GfsVolume> {
                        new GfsVolume {
                            Name = "glusterVolume01",
                            Brick = "/Path/To/Brick00/",
                            MountPoint = "/Local/Mount/Point01"
                        },
                        new GfsVolume {
                            Name = "glusterVolume02",
                            Brick = "/Path/To/Brick00/",
                            MountPoint = "/Local/Mount/Point02"
                        }
                    }
                };
                if (!File.Exists(tempFlow.Path)) {
                    File.WriteAllText(tempFlow.Path, JsonConvert.SerializeObject(tempFlow, Formatting.Indented));
                    ConsoleLogger.Log("a gluster configuration file template has been created");
                }
                return;
            }
            //var text = File.ReadAllText(FilePath);
            //var flow = JsonConvert.DeserializeObject<GlusterSetup>(text);
        }

        public static void Start() {
            if (Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Start(ServiceName);
            }
        }

        public static List<Control> Get() {
            Directory.CreateDirectory(Parameter.RepoConfig);
            if (!File.Exists(FilePath)) {
                return new List<Control>();
            }
            var text = File.ReadAllText(FilePath);
            var flow = JsonConvert.DeserializeObject<ConfigurationFlow>(text);
            var controls = flow?.Controls?.OrderBy(_ => _.Index).ToList();
            return controls;
        }
    }
}
