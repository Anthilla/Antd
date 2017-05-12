using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace Antd.Storage {
    public class Disks {
        public List<DiskModel> GetList() {
            var str = Bash.Execute("lsblk -JO");
            var clean = str?.Replace("-", "_").Replace("maj:min", "maj_min");
            var ret = JsonConvert.DeserializeObject<LsblkJsonModel.Lsblk>(clean);
            var result = ret.blockdevices.Select(_ => new DiskModel(_)).ToList();
            return result;
        }
    }
}
