using Antd2.cmds;
using Nancy;
using System;
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

            Get("/check/fs/{device*}", x => ApiGetCheckFs(x));

        }

        private dynamic ApiGet() {
            var disks = Lsblk.Get();
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

        private dynamic ApiGetCheckFs(dynamic x) {
            string partition = x.device;
            var fsck = E2fsck.CheckPartition(partition);
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(fsck);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }
    }
}