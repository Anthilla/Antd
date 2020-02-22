using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Antd2.models {
    public class LsblkBlockdeviceChild {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("kname")]
        public string Kname { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("majmin")]
        public string MajMin { get; set; }

        [JsonProperty("fsavail")]
        public string Fsavail { get; set; }

        [JsonProperty("fssize")]
        public string Fssize { get; set; }

        [JsonProperty("fstype")]
        public string Fstype { get; set; }

        [JsonProperty("fsused")]
        public string Fsused { get; set; }

        [JsonProperty("fsuseperc")]
        public string FsUsePerc { get; set; }

        [JsonProperty("mountpoint")]
        public string Mountpoint { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("ptuuid")]
        public string Ptuuid { get; set; }

        [JsonProperty("pttype")]
        public string Pttype { get; set; }

        [JsonProperty("parttype")]
        public string Parttype { get; set; }

        [JsonProperty("partlabel")]
        public string Partlabel { get; set; }

        [JsonProperty("partuuid")]
        public string Partuuid { get; set; }

        [JsonProperty("partflags")]
        public string Partflags { get; set; }

        [JsonProperty("ra")]
        public int Ra { get; set; }

        [JsonProperty("ro")]
        public bool Ro { get; set; }

        [JsonProperty("rm")]
        public bool Rm { get; set; }

        [JsonProperty("hotplug")]
        public bool Hotplug { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("serial")]
        public string Serial { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("alignment")]
        public int Alignment { get; set; }

        [JsonProperty("minio")]
        public int MinIo { get; set; }

        [JsonProperty("optio")]
        public int OptIo { get; set; }

        [JsonProperty("physec")]
        public int PhySec { get; set; }

        [JsonProperty("logsec")]
        public int LogSec { get; set; }

        [JsonProperty("rota")]
        public bool Rota { get; set; }

        [JsonProperty("sched")]
        public string Sched { get; set; }

        [JsonProperty("rqsize")]
        public long RqSize { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("discaln")]
        public int DiscAln { get; set; }

        [JsonProperty("discgran")]
        public int DiscGran { get; set; }

        [JsonProperty("discmax")]
        public int DiscMax { get; set; }

        [JsonProperty("disczero")]
        public bool DiscZero { get; set; }

        [JsonProperty("wsame")]
        public int Wsame { get; set; }

        [JsonProperty("wwn")]
        public string Wwn { get; set; }

        [JsonProperty("rand")]
        public bool Rand { get; set; }

        [JsonProperty("pkname")]
        public string Pkname { get; set; }

        [JsonProperty("hctl")]
        public string Hctl { get; set; }

        [JsonProperty("tran")]
        public string Tran { get; set; }

        [JsonProperty("subsystems")]
        public string Subsystems { get; set; }

        [JsonProperty("rev")]
        public string Rev { get; set; }

        [JsonProperty("vendor")]
        public string Vendor { get; set; }

        [JsonProperty("zoned")]
        public string Zoned { get; set; }

        public bool WebdavRunning { get; set; }
        public bool IsVolume { get; set; }
        public System.Collections.Generic.List<string> SyncableVolumes { get; set; }

    }

    public class LsblkBlockdevice {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("kname")]
        public string Kname { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("majmin")]
        public string MajMin { get; set; }

        [JsonProperty("fsavail")]
        public string Fsavail { get; set; }

        [JsonProperty("fssize")]
        public int Fssize { get; set; }

        [JsonProperty("fstype")]
        public string Fstype { get; set; }

        [JsonProperty("fsused")]
        public int Fsused { get; set; }

        [JsonProperty("fsuseperc")]
        public int FsUsePerc { get; set; }

        [JsonProperty("mountpoint")]
        public string Mountpoint { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("uuid")]
        public string Uuid { get; set; }

        [JsonProperty("ptuuid")]
        public string Ptuuid { get; set; }

        [JsonProperty("pttype")]
        public string Pttype { get; set; }

        [JsonProperty("parttype")]
        public string Parttype { get; set; }

        [JsonProperty("partlabel")]
        public string Partlabel { get; set; }

        [JsonProperty("partuuid")]
        public string Partuuid { get; set; }

        [JsonProperty("partflags")]
        public string Partflags { get; set; }

        [JsonProperty("ra")]
        public int Ra { get; set; }

        [JsonProperty("ro")]
        public bool Ro { get; set; }

        [JsonProperty("rm")]
        public bool Rm { get; set; }

        [JsonProperty("hotplug")]
        public bool Hotplug { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("serial")]
        public string Serial { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("owner")]
        public string Owner { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("alignment")]
        public int Alignment { get; set; }

        [JsonProperty("minio")]
        public int MinIo { get; set; }

        [JsonProperty("optio")]
        public int OptIo { get; set; }

        [JsonProperty("physec")]
        public int PhySec { get; set; }

        [JsonProperty("logsec")]
        public int LogSec { get; set; }

        [JsonProperty("rota")]
        public bool Rota { get; set; }

        [JsonProperty("sched")]
        public string Sched { get; set; }

        [JsonProperty("rqsize")]
        public long RqSize { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("discaln")]
        public int DiscAln { get; set; }

        [JsonProperty("discgran")]
        public int DiscGran { get; set; }

        [JsonProperty("discmax")]
        public int DiscMax { get; set; }

        [JsonProperty("disczero")]
        public bool DiscZero { get; set; }

        [JsonProperty("wsame")]
        public int Wsame { get; set; }

        [JsonProperty("wwn")]
        public string Wwn { get; set; }

        [JsonProperty("rand")]
        public bool Rand { get; set; }

        [JsonProperty("pkname")]
        public string Pkname { get; set; }

        [JsonProperty("hctl")]
        public string Hctl { get; set; }

        [JsonProperty("tran")]
        public string Tran { get; set; }

        [JsonProperty("subsystems")]
        public string Subsystems { get; set; }

        [JsonProperty("rev")]
        public string Rev { get; set; }

        [JsonProperty("vendor")]
        public string Vendor { get; set; }

        [JsonProperty("zoned")]
        public string Zoned { get; set; }

        [JsonProperty("children")]
        public List<LsblkBlockdeviceChild> Children { get; set; }
    }

    public class LsblkModel {

        [JsonProperty("blockdevices")]
        public List<LsblkBlockdevice> Blockdevices { get; set; }

    }
}
