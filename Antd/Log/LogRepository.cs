using System;
using System.Linq;

namespace Antd.Log {
    public class LogRepo {
        public LogModel[] GetAll() {
            var list = DeNSo.Session.New.Get<LogModel>().ToList();
            return (from l in list
                   where l != null
                   orderby l.time descending
                   select l).ToArray();
        }

        public static void Create(string _time, string _mode, string _file) {
            var l = new LogModel() {
                _Id = Guid.NewGuid().ToString(),
                time = _time,
                mode = _mode,
                file = _file
            };
            DeNSo.Session.New.Set(l);
        }

        public static void Create(string _time, string _mode, string _file, string _oldfile) {
            var l = new LogModel() {
                _Id = Guid.NewGuid().ToString(),
                time = _time,
                mode = _mode,
                file = _file,
                oldfile = _oldfile
            };
            DeNSo.Session.New.Set(l);
        }
    }
}
