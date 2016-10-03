using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.common.Helpers;
using Antd.Configuration;
using Newtonsoft.Json;

namespace Antd.Gluster {
    public class GlusterConfiguration {

        private static readonly string FilePath = $"{Parameter.RepoConfig}/gluster.conf";
        private const string ServiceName = "glusterd.service";

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
            }
        }

        public static bool IsConfigured => File.Exists(FilePath);

        public static void Start() {
            Console.WriteLine($"systemctl start {ServiceName}");
            Bash.Execute($"systemctl start {ServiceName}");
        }

        public static void Launch() {
            if (!File.Exists(FilePath)) {
                return;
            }
            var text = File.ReadAllText(FilePath);
            var config = JsonConvert.DeserializeObject<GlusterSetup>(text);

            foreach (var node in config.Nodes) {
                Console.WriteLine($"glusterfs: setup {node} node");
                Console.WriteLine(Bash.Execute($"gluster peer probe {node}"));
            }

            var numberOfNodes = config.Nodes.Count.ToString();

            foreach (var volume in config.Volumes) {
                Console.WriteLine($"glusterfs: setup {volume.Name} volume");
                var volumePath = $"{volume.Brick}{volume.Name}";
                Directory.CreateDirectory(volumePath);

                //Terminal.Execute($"setfattr -x trusted.gfid {volumePath}");
                //Terminal.Execute($"setfattr -x trusted.glusterfs.volume-id {volumePath}");
                //Terminal.Execute($"rm -fR {volumePath}/.glusterfs");

                var volumesList = new List<string>();
                foreach (var node in config.Nodes) {
                    volumesList.Add($"{node}:{volumePath}");
                }

                VolumeCreate(volume.Name, numberOfNodes, volumesList.ToArray());
                VolumeStart(volume.Name);

                Directory.CreateDirectory(volume.MountPoint);
                foreach (var node in config.Nodes) {
                    VolumeMount(node, volume.Name, volume.MountPoint);
                }
            }
        }

        public static void VolumeCreate(string volumeName, string numberOfNodes, string[] volumesList) {
            var volString = string.Join(" ", volumesList);
            Console.WriteLine($"gluster volume create {volumeName} replica {numberOfNodes} {volString} force");
            Bash.Execute($"gluster volume create {volumeName} replica {numberOfNodes} {volString} force");
        }

        public static void VolumeStart(string volumeName) {
            Bash.Execute($"gluster volume start {volumeName}");
        }

        public static void VolumeMount(string node, string volumeName, string mountPoint) {
            if (Mounts.IsAlreadyMounted(mountPoint) == false) {
                Bash.Execute($"mount -t glusterfs {node}:/{volumeName} {mountPoint}");
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
