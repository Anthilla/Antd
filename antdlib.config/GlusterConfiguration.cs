using antdlib.common;
using antdlib.common.Helpers;
using antdlib.models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using IoDir = System.IO.Directory;


namespace antdlib.config {
    public static class GlusterConfiguration {

        private static GlusterConfigurationModel ServiceModel => Load();

        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/gluster.conf";
        private static readonly string CfgFileBackup = $"{Parameter.AntdCfgServices}/gluster.conf.bck";
        private const string ServiceName = "glusterd.service";

        private static GlusterConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new GlusterConfigurationModel();
            }
            try {
                var text = File.ReadAllText(CfgFile);
                var obj = JsonConvert.DeserializeObject<GlusterConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new GlusterConfigurationModel();
            }
        }

        public static void Save(GlusterConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(CfgFile)) {
                File.Copy(CfgFile, CfgFileBackup, true);
            }
            File.WriteAllText(CfgFile, text);
            ConsoleLogger.Log("[sync] configuration saved");
        }


        public static void Set() {
            Enable();
            Stop();
            Start();
            Launch();
        }

        public static bool IsActive() {
            return ServiceModel != null && ServiceModel.IsActive;
        }

        public static GlusterConfigurationModel Get() {
            return ServiceModel;
        }

        public static void Enable() {
            Save(ServiceModel);
            ConsoleLogger.Log("[sync] enabled");
        }

        public static void Disable() {
            ServiceModel.IsActive = false;
            Save(ServiceModel);
            ConsoleLogger.Log("[sync] disabled");
        }

        public static void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[sync] stop");
        }

        public static void Start() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }
            ConsoleLogger.Log("[sync] start");
        }

        private static readonly Bash Bash = new Bash();

        public static void Launch() {
            var config = ServiceModel;
            foreach(var node in config.Nodes) {
                Console.WriteLine($"glusterfs: setup {node} node");
                Console.WriteLine(Bash.Execute($"gluster peer probe {node}"));
            }
            var numberOfNodes = ServiceModel.Nodes.Length;
            foreach(var volume in config.Volumes) {
                Console.WriteLine($"glusterfs: setup {volume.Name} volume");
                var volumePath = $"{volume.Brick}{volume.Name}";
                IoDir.CreateDirectory(volumePath);
                //Terminal.Execute($"setfattr -x trusted.gfid {volumePath}");
                //Terminal.Execute($"setfattr -x trusted.glusterfs.volume-id {volumePath}");
                //Terminal.Execute($"rm -fR {volumePath}/.glusterfs");
                VolumeCreate(volume.Name, numberOfNodes.ToString(), config.Nodes.Select(node => $"{node}:{volumePath}").ToArray());
                VolumeStart(volume.Name);
                IoDir.CreateDirectory(volume.MountPoint);
                foreach(var node in config.Nodes) {
                    VolumeMount(node, volume.Name, volume.MountPoint);
                }
            }
        }

        #region [    Private - Volumes Management    ]
        private static void VolumeCreate(string volumeName, string numberOfNodes, string[] volumesList) {
            var volString = string.Join(" ", volumesList);
            Console.WriteLine($"gluster volume create {volumeName} replica {numberOfNodes} {volString} force");
            Bash.Execute($"gluster volume create {volumeName} replica {numberOfNodes} {volString} force", false);
        }

        private static void VolumeStart(string volumeName) {
            Bash.Execute($"gluster volume start {volumeName}", false);
        }

        private static void VolumeMount(string node, string volumeName, string mountPoint) {
            if(MountHelper.IsAlreadyMounted(mountPoint) == false) {
                Bash.Execute($"mount -t glusterfs {node}:/{volumeName} {mountPoint}", false);
            }
        }
        #endregion

        public static void AddNode(string model) {
            var node = ServiceModel.Nodes;
            if(node.Any(_ => _ == model)) {
                return;
            }
            node.ToList().Add(model);
            ServiceModel.Nodes = node;
            Save(ServiceModel);
        }

        public static void RemoveNode(string guid) {
            var volume = ServiceModel.Volumes;
            var model = volume.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            volume.ToList().Remove(model);
            ServiceModel.Volumes = volume;
            Save(ServiceModel);
        }

        public static void AddVolume(GlusterVolume model) {
            var volume = ServiceModel.Volumes;
            if(volume.Any(_ => _.Name == model.Name)) {
                return;
            }
            volume.ToList().Add(model);
            ServiceModel.Volumes = volume;
            Save(ServiceModel);
        }

        public static void RemoveVolume(string guid) {
            var volume = ServiceModel.Volumes;
            var model = volume.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            volume.ToList().Remove(model);
            ServiceModel.Volumes = volume;
            Save(ServiceModel);
        }
    }
}
