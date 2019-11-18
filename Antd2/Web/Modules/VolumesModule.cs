using Antd2.cmds;
using Antd2.models;
using Antd2.Storage;
using Nancy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Antd2.Modules {
    public class VolumesModule : NancyModule {

        private static readonly IDictionary<string, WebDavServer> WebdavInstances = new Dictionary<string, WebDavServer>();
        private static readonly IDictionary<string, bool> WebdavStatus = new Dictionary<string, bool>();

        public class VolumeView {
            public List<PartedPartitionModel> Volumes { get; set; }
            public List<string> SyncableVolumes { get; set; }
        }

        public VolumesModule() : base("/volumes") {

            Get("/", x => ApiGet());

            Post("/mount", x => ApiPostMount());

            Post("/umount", x => ApiPostUmount());

            Post("/webdav/start", x => ApiPostWebdavStart());

            Post("/webdav/stop", x => ApiPostWebdavStop());

            Post("/sync", x => ApiPostSyncVolumes());

        }

        private dynamic ApiGet() {
            var diskList = Parted.GetDisks();
            var disks = diskList.Select(_ => Parted.Print(_)).ToArray();

            var disksLsblk = Lsblk.Get();
            var disksBlkid = Blkid.Get();
            var df = Df.Get();

            var volumes = new List<PartedPartitionModel>();

            foreach (var disk in disks) {
                foreach (var partition in disk.Partitions) {
                    var partitionBlk = disksBlkid.FirstOrDefault(_ => _.Partition == partition.Name);
                    if (!string.IsNullOrEmpty(partitionBlk.Uuid)) {
                        partition.IsVolume = true;
                        partition.Mountpoint = disksLsblk.FirstOrDefault(_ => "/dev/" + _.Name == partition.Name).Mountpoint;
                        partition.FsType = disksBlkid.FirstOrDefault(_ => _.Partition == partition.Name).Type;
                        partition.Label = disksBlkid.FirstOrDefault(_ => _.Partition == partition.Name).Label;

                        partition.Size = df.FirstOrDefault(_ => _.FS == partition.Name).Blocks;
                        partition.Used = df.FirstOrDefault(_ => _.FS == partition.Name).Used;

                        if (WebdavStatus.ContainsKey(partition.Mountpoint)) {
                            partition.WebdavRunning = WebdavStatus[partition.Mountpoint];
                        }

                        if (partition.FsType == "zfs_member") {
                            partition.Mountpoint = "/Data/" + partition.Label;
                        }

                        if (!string.IsNullOrEmpty(partition.FsType) &&
                            partition.Label != "EFI" &&
                            partition.Label != "System01" &&
                            partition.Label != "BootExt01" &&
                            partition.FsType != "linux_raid_member")
                            volumes.Add(partition);
                    }
                }
            }

            foreach (var a in volumes)
                a.SyncableVolumes = volumes.Where(_ => !string.IsNullOrEmpty(_.Mountpoint) && _.Name != a.Name).Select(_ => _.Mountpoint).ToList();

            if (WebdavStatus.Count == 0) {
                Console.WriteLine("no webdav active");
            }
            if (WebdavInstances.Count == 0) {
                Console.WriteLine("no webdav active");
            }
            foreach (var a in WebdavStatus) {
                Console.WriteLine($"[wd] active on {a.Key}");
            }

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(volumes.OrderBy(_ => _.Name).ToArray());
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiPostMount() {
            string partition = Request.Form.Partition;
            string label = Request.Form.Label;

            var blkid = Blkid.Get();
            var p = blkid.FirstOrDefault(_ => _.Partition == partition);
            if (string.IsNullOrEmpty(p.Uuid))
                return HttpStatusCode.InternalServerError;

            var destination = "/Data/" + p.Uuid;
            Directory.CreateDirectory(destination);
            Console.WriteLine($"mount LABEL={label} {destination}");
            Bash.Do($"mkdir -p {destination}");
            Bash.Do($"mount LABEL={label} {destination}");
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostUmount() {
            string mountpoint = Request.Form.Mountpoint;
            Mount.Umount(mountpoint);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostWebdavStart() {
            string ip = Request.Form.Ip;
            int port = Request.Form.Port;
            string root = Request.Form.Mountpoint;
            string user = Request.Form.User;
            string password = Request.Form.Password;

            System.Threading.Tasks.Task.Factory.StartNew(() => {
                var wd = new WebDavServer($"antd_{root.Replace("/", "_")}", ip, port, root, user, password);
                VolumesModule.WebdavStatus[root] = true;
                VolumesModule.WebdavInstances[root] = wd;
                wd.Start();
            });

            return HttpStatusCode.OK;
        }

        private dynamic ApiPostWebdavStop() {
            string root = Request.Form.Mountpoint;
            if (WebdavInstances.ContainsKey(root)) {
                if (WebdavInstances[root] != null) {
                    WebdavInstances[root].Stop();
                    WebdavInstances[root] = null;
                }
            }
            if (WebdavStatus.ContainsKey(root)) {
                WebdavStatus[root] = false;
            }
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostSyncVolumes() {
            string source = Request.Form.Source;
            string destination = Request.Form.Destination;
            if (!Directory.Exists(source))
                return HttpStatusCode.NotFound;
            if (!Directory.Exists(destination))
                return HttpStatusCode.NotFound;
            if (!source.EndsWith("/"))
                source += "/";
            if (!destination.EndsWith("/"))
                destination += "/";
            Rsync.SyncCustom("-aH --partial", source, destination);
            return HttpStatusCode.OK;
        }

    }
}