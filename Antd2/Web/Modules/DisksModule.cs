using Antd2.cmds;
using Nancy;
using System.Linq;
using System.Text;

namespace Antd2.Modules {
    public class DisksModule : NancyModule {

        public DisksModule() : base("/disks") {

            Get("/", x => ApiGet());

            Post("/create/partition/table", x => ApiPostCreatePartitionTable());

            Post("/create/partition", x => ApiPostCreatePartition());

            Post("/create/fs/ext4", x => ApiPostCreateFsExt4());

            Post("/create/fs/zfs", x => ApiPostCreateFsZfs());

        }

        private dynamic ApiGet() {
            var diskList = Parted.GetDisks();
            var disks = diskList.Select(_ => Parted.Print(_)).ToArray();

            var disksLsblk = Lsblk.Get();
            var disksBlkid = Blkid.Get();

            foreach (var disk in disks) {
                foreach (var partition in disk.Partitions) {
                    var partitionBlk = disksBlkid.FirstOrDefault(_ => _.Partition == partition.Name);
                    if (!string.IsNullOrEmpty(partitionBlk.Uuid)) {
                        partition.IsVolume = true;
                        partition.Mountpoint = disksLsblk.FirstOrDefault(_ => _.Name == partition.Name).Mountpoint;
                        partition.FsType = disksBlkid.FirstOrDefault(_ => _.Partition == partition.Name).Type;
                        partition.Label = disksBlkid.FirstOrDefault(_ => _.Partition == partition.Name).Label;
                    }
                }
            }

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(disks);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiPostCreatePartitionTable() {
            string device = Request.Form.Device;
            string label = Request.Form.Label;
            Parted.SetDiskLabel(device, label);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostCreatePartition() {
            string device = Request.Form.Device;
            string partType = Request.Form.PartType;
            string partName = Request.Form.PartName;
            string fsType = Request.Form.FsType;
            string start = Request.Form.Start;
            string end = Request.Form.End;
            Parted.SetPartition(device, partType, partName, fsType, start, end);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostCreateFsExt4() {
            string device = Request.Form.Device;
            string label = Request.Form.Label;
            Mkfs.Ext4.AddLabel(device, label);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostCreateFsZfs() {
            string device = Request.Form.Device;
            string pool = Request.Form.Pool;
            string label = Request.Form.Label;
            Zfs.CreateFs(device, pool, label);
            return HttpStatusCode.OK;
        }
        
    }
}