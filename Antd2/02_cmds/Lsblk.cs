﻿using Antd2.models;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Antd2.cmds {

    /// <summary>
    /// https://linux.die.net/man/8/lsblk
    /// </summary>
    public class Lsblk {

        private const string lsblkCommand = "lsblk";

        private static readonly IDictionary<string, string> Substitutions = new Dictionary<string, string>() {
            { "maj:min",    "majmin" },
            { "min-io",     "minio" },
            { "opt-io",     "optio" },
            { "phy-sec",    "physec" },
            { "log-sec",    "logsec" },
            { "rq-size",    "rqsize" },
            { "disc-aln",   "discaln" },
            { "disc-gran",  "discgran" },
            { "disc-max",   "discmax" },
            { "disc-zero",  "disczero" },
            { "fsuse%",     "fsuseperc" }
        };

        public static List<LsblkBlockdevice> Get() {
            var commandResultLines = Bash.Execute($"{lsblkCommand} -banJO");
            var commandResult = string.Join("", commandResultLines);
            var js = new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore
            };

            foreach (var kvp in Substitutions) {
                commandResult = commandResult.Replace(kvp.Key, kvp.Value);
            }

            var disks = JsonConvert.DeserializeObject<LsblkModel>(commandResult, js);

            foreach (var disk in disks.Blockdevices) {
                disk.Name = "/dev/" + disk.Name;
                foreach (var partition in disk.Children) {
                    partition.Name = "/dev/" + partition.Name;
                    partition.IsVolume = true;
                    if (partition.Fstype == "zfs_member") {
                        partition.Mountpoint = "/Data/" + partition.Label;
                    }
                }
            }

            return disks.Blockdevices;
        }

    }
}
