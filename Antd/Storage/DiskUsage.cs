using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.Models;

namespace Antd.Storage {
    public class DiskUsage {

        private readonly MapToModel _mapper = new MapToModel();

        public List<DiskUsageModel> GetInfo() {
            var result = _mapper.FromCommand<DiskUsageModel>("df -HTP").Skip(1).ToList();
            return result;
        }
    }
}
