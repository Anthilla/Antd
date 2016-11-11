using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Antd.Database;

namespace Antd.Log {
    public class SyslogNg {

        public class Model {
            public DateTime DateTime { get; set; }
            public string Date { get; set; }
            public string Host { get; set; }
            public string LogText { get; set; }
        }
        private static readonly SyslogRepository SyslogRepository = new SyslogRepository();

        public List<Model> GetAll() {
            var list = new List<Model>();
            var config = SyslogRepository.Get();
            if(config == null) {
                return list;
            }
            if(string.IsNullOrEmpty(config.RootPath)) {
                return list;
            }
            var dir = config.RootPath;
            var files = Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories);
            foreach(var file in files) {
                var path = file.Replace(dir, "");
                var dd = path.Contains("/") ? '/' : path.Contains("\\") ? '\\' : ' ';
                if(string.IsNullOrEmpty(dd.ToString())) { continue; }
                var pathel = path.Split(dd);
                if(pathel.Length < 3) { continue; }
                try {
                    var date = DateTime.ParseExact($"{pathel[1]}{pathel[2]}{pathel[3]}", "yyMMMdd", CultureInfo.InvariantCulture);
                    var log = File.ReadAllText(file);
                    var l = new Model {
                        DateTime = date,
                        Date = date.ToString("yyyyMMdd"),
                        Host = pathel[0],
                        LogText = log
                    };
                    list.Add(l);
                }
                catch(Exception) { }
            }
            return list;
        }
    }
}
