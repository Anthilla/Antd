using Antd2.cmds;
using Antd2.models;
using Nancy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Antd2.Modules {
    public class VolumesModule : NancyModule {

        public class VolumeView {
            public List<PartedPartitionModel> Volumes { get; set; }
            public List<string> SyncableVolumes { get; set; }
        }

        public VolumesModule() : base("/volumes") {

            Get("/", x => ApiGet());

            Post("/mount", x => ApiPostMount());

            Post("/umount", x => ApiPostUmount());

            Post("/sync", x => ApiPostSyncVolumes());

        }

        private dynamic ApiGet() {
            var disks = Lsblk.Get();

            var volumes = new List<LsblkBlockdeviceChild>();
            foreach (var disk in disks) {
                foreach (var partition in disk.Children) {

                    if (string.IsNullOrEmpty(partition.Mountpoint))
                        partition.Mountpoint = "";

                    if (StartCommand.WebdavStatus.ContainsKey(partition.Mountpoint))
                        partition.WebdavRunning = StartCommand.WebdavStatus[partition.Mountpoint];


                    if (!string.IsNullOrEmpty(partition.Fstype) &&
                        partition.Label != "EFI" &&
                        partition.Label != "System01" &&
                        partition.Label != "BootExt01" &&
                        partition.Fstype != "swap" &&
                        partition.Fstype != "linux_raid_member" &&
                        partition.Fstype != "zfs_member"
                        )
                        volumes.Add(partition);
                }
            }

            foreach (var a in volumes) {
                a.SyncableVolumes = volumes.Where(_ => !string.IsNullOrEmpty(_.Mountpoint) && _.Name != a.Name).Select(_ => _.Mountpoint).ToList();
            }

            var df = Df.Get();
            var zfsParts = df.Where(_ => _.Type == "zfs").ToArray();
            Console.WriteLine($"zfs: {zfsParts.Length}");
            foreach (var d in zfsParts) {
                Console.WriteLine($"zfs: {d.FS}");
                var partition = new LsblkBlockdeviceChild();
                partition.Fstype = d.Type;
                partition.Label = d.FS;
                partition.FsUsePerc = d.Used;
                partition.Size = int.Parse(d.Avail);
                partition.Mountpoint = d.Mountpoint;
                //partition.Name = disksBlkid.FirstOrDefault(_ => _.Label.StartsWith(partition.Label)).Partition;

                if (StartCommand.WebdavStatus.ContainsKey(partition.Mountpoint)) {
                    partition.WebdavRunning = StartCommand.WebdavStatus[partition.Mountpoint];
                }
                volumes.Add(partition);
            }

            if (StartCommand.WebdavStatus.Count == 0) {
                Console.WriteLine("no webdav active");
            }
            if (StartCommand.WebdavInstances.Count == 0) {
                Console.WriteLine("no webdav active");
            }
            foreach (var a in StartCommand.WebdavStatus) {
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
            //string /*label*/ = Request.Form.Label;

            var blkid = Blkid.Get();
            var p = blkid.FirstOrDefault(_ => _.Partition == partition);
            if (string.IsNullOrEmpty(p.Uuid))
                return HttpStatusCode.InternalServerError;

            var destination = "/Data/" + p.Partuuid;
            Directory.CreateDirectory(destination);
            Console.WriteLine($"mount PARTUUID={p.Partuuid} {destination}");
            Bash.Do($"mkdir -p {destination}");
            Bash.Do($"mount PARTUUID={p.Partuuid} {destination}");
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostUmount() {
            string mountpoint = Request.Form.Mountpoint;
            Mount.Umount(mountpoint);
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