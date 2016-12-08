using System;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.common.Helpers;
using antdlib.common.Tool;
using antdlib.Systemd;
using Newtonsoft.Json;
using IoDir = System.IO.Directory;


namespace Antd.Gluster {
    public class GlusterConfiguration {

        private readonly GlusterConfigurationModel _serviceModel;

        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/gluster.conf";
        private readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/gluster.conf.bck";
        private const string ServiceName = "glusterd.service";

        public GlusterConfiguration() {
            IoDir.CreateDirectory(Parameter.AntdCfgServices);
            if(!File.Exists(_cfgFile)) {
                _serviceModel = new GlusterConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<GlusterConfigurationModel>(text);
                    _serviceModel = obj;
                }
                catch(Exception) {
                    _serviceModel = new GlusterConfigurationModel();
                }
            }
        }

        public void Save(GlusterConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
            ConsoleLogger.Log("[sync] configuration saved");
        }


        public void Set() {
            Enable();
            Stop();
            Start();
            Launch();
        }

        public bool IsActive() {
            return _serviceModel != null && _serviceModel.IsActive;
        }

        public GlusterConfigurationModel Get() {
            return _serviceModel;
        }

        public void Enable() {
            Save(_serviceModel);
            ConsoleLogger.Log("[sync] enabled");
        }

        public void Disable() {
            _serviceModel.IsActive = false;
            Save(_serviceModel);
            ConsoleLogger.Log("[sync] disabled");
        }

        public void Stop() {
            Systemctl.Stop(ServiceName);
            ConsoleLogger.Log("[sync] stop");
        }

        public void Start() {
            if(Systemctl.IsEnabled(ServiceName) == false) {
                Systemctl.Enable(ServiceName);
            }
            if(Systemctl.IsActive(ServiceName) == false) {
                Systemctl.Restart(ServiceName);
            }
            ConsoleLogger.Log("[sync] start");
        }

        private readonly Bash _bash = new Bash();

        public void Launch() {
            var config = _serviceModel;
            foreach(var node in config.Nodes) {
                Console.WriteLine($"glusterfs: setup {node} node");
                Console.WriteLine(_bash.Execute($"gluster peer probe {node}"));
            }
            var numberOfNodes = _serviceModel.Nodes.Length;
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

        private void VolumeCreate(string volumeName, string numberOfNodes, string[] volumesList) {
            var volString = string.Join(" ", volumesList);
            Console.WriteLine($"gluster volume create {volumeName} replica {numberOfNodes} {volString} force");
            _bash.Execute($"gluster volume create {volumeName} replica {numberOfNodes} {volString} force", false);
        }

        private void VolumeStart(string volumeName) {
            _bash.Execute($"gluster volume start {volumeName}", false);
        }

        private void VolumeMount(string node, string volumeName, string mountPoint) {
            if(MountHelper.IsAlreadyMounted(mountPoint) == false) {
                _bash.Execute($"mount -t glusterfs {node}:/{volumeName} {mountPoint}", false);
            }
        }

        public void AddNode(string model) {
            var node = _serviceModel.Nodes;
            if(node.Any(_ => _ == model)) {
                return;
            }
            node.ToList().Add(model);
            _serviceModel.Nodes = node;
            Save(_serviceModel);
        }

        public void RemoveNode(string guid) {
            var volume = _serviceModel.Volumes;
            var model = volume.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            volume.ToList().Remove(model);
            _serviceModel.Volumes = volume;
            Save(_serviceModel);
        }

        public void AddVolume(GlusterVolume model) {
            var volume = _serviceModel.Volumes;
            if(volume.Any(_ => _.Name == model.Name)) {
                return;
            }
            volume.ToList().Add(model);
            _serviceModel.Volumes = volume;
            Save(_serviceModel);
        }

        public void RemoveVolume(string guid) {
            var volume = _serviceModel.Volumes;
            var model = volume.First(_ => _.Guid == guid);
            if(model == null) {
                return;
            }
            volume.ToList().Remove(model);
            _serviceModel.Volumes = volume;
            Save(_serviceModel);
        }
    }
}
