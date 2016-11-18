using System;
using System.Collections.Generic;
using System.IO;
using antdlib.common;
using antdlib.common.Helpers;
using antdlib.common.Tool;
using Newtonsoft.Json;

namespace Antd.Gluster {
    public class GlusterConfiguration {

        private static readonly string FilePath = $"{Parameter.RepoConfig}/gluster.conf";
        private const string ServiceName = "glusterd.service";

        public void Set() {
            Directory.CreateDirectory(Parameter.RepoConfig);
            if(!File.Exists(FilePath)) {
                var tempFlow = new GlusterSetup {
                    Name = ".gluster.conf",
                    Path = $"{Parameter.RepoConfig}/.gluster.conf",
                    IsConfigured = false,
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
                if(!File.Exists(tempFlow.Path)) {
                    File.WriteAllText(tempFlow.Path, JsonConvert.SerializeObject(tempFlow, Formatting.Indented));
                }
            }
        }

        public void Set(GlusterSetup setup) {
            Directory.CreateDirectory(Parameter.RepoConfig);
            if(!File.Exists(FilePath)) {
                return;
            }
            var text = JsonConvert.SerializeObject(setup, Formatting.Indented);
            File.WriteAllText(FilePath, text);
        }

        public bool IsConfigured => CheckIfIsConfigured();

        private static bool CheckIfIsConfigured() {
            if(!File.Exists(FilePath)) {
                return false;
            }
            try {
                var t = File.ReadAllText(FilePath);
                var m = JsonConvert.DeserializeObject<GlusterSetup>(t);
                if(m == null) {
                    return false;
                }
                return m.IsConfigured;
            }
            catch(Exception) {
                return false;
            }
        }

        private static readonly Bash Bash = new Bash();

        public void Start() {
            Console.WriteLine($"systemctl start {ServiceName}");
            Bash.Execute($"systemctl start {ServiceName}", false);
        }

        public void Launch() {
            if(!File.Exists(FilePath)) {
                return;
            }
            var text = File.ReadAllText(FilePath);
            var config = JsonConvert.DeserializeObject<GlusterSetup>(text);

            foreach(var node in config.Nodes) {
                Console.WriteLine($"glusterfs: setup {node} node");
                Console.WriteLine(Bash.Execute($"gluster peer probe {node}"));
            }

            var numberOfNodes = config.Nodes.Count.ToString();

            foreach(var volume in config.Volumes) {
                Console.WriteLine($"glusterfs: setup {volume.Name} volume");
                var volumePath = $"{volume.Brick}{volume.Name}";
                Directory.CreateDirectory(volumePath);

                //Terminal.Execute($"setfattr -x trusted.gfid {volumePath}");
                //Terminal.Execute($"setfattr -x trusted.glusterfs.volume-id {volumePath}");
                //Terminal.Execute($"rm -fR {volumePath}/.glusterfs");

                var volumesList = new List<string>();
                foreach(var node in config.Nodes) {
                    volumesList.Add($"{node}:{volumePath}");
                }

                VolumeCreate(volume.Name, numberOfNodes, volumesList.ToArray());
                VolumeStart(volume.Name);

                Directory.CreateDirectory(volume.MountPoint);
                foreach(var node in config.Nodes) {
                    VolumeMount(node, volume.Name, volume.MountPoint);
                }
            }
        }

        public void VolumeCreate(string volumeName, string numberOfNodes, string[] volumesList) {
            var volString = string.Join(" ", volumesList);
            Console.WriteLine($"gluster volume create {volumeName} replica {numberOfNodes} {volString} force");
            Bash.Execute($"gluster volume create {volumeName} replica {numberOfNodes} {volString} force", false);
        }

        public void VolumeStart(string volumeName) {
            Bash.Execute($"gluster volume start {volumeName}", false);
        }

        public void VolumeMount(string node, string volumeName, string mountPoint) {
            if(MountHelper.IsAlreadyMounted(mountPoint) == false) {
                Bash.Execute($"mount -t glusterfs {node}:/{volumeName} {mountPoint}", false);
            }
        }

        public GlusterSetup Get() {
            Directory.CreateDirectory(Parameter.RepoConfig);
            if(!File.Exists(FilePath)) {
                return new GlusterSetup();
            }
            var text = File.ReadAllText(FilePath);
            var setup = JsonConvert.DeserializeObject<GlusterSetup>(text);
            return setup;
        }
    }
}
