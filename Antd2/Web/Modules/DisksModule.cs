using Antd2.cmds;
using Nancy;
using System.Linq;
using System.Text;

namespace Antd2.Modules {
    public class DisksModule : NancyModule {

        public DisksModule() : base("/disks") {

            Get("/", x => ApiGet());

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

            var myJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(disks);
            var jsonBytes = Encoding.UTF8.GetBytes(myJsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }
    }
}