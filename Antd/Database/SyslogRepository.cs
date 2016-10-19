using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class SyslogRepository {

        private const string ConfigGuid = "9152427C-E573-4564-9148-27010A58FC5A";
        private const string ViewName = "Syslog";

        public IEnumerable<SyslogSchema> GetAll() {
            var result = DatabaseRepository.Query<SyslogSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public SyslogSchema Get() {
            var result = DatabaseRepository.Query<SyslogSchema>(AntdApplication.Database, ViewName, schema => schema.Id == ConfigGuid || schema.Guid == ConfigGuid);
            return result.FirstOrDefault();
        }

        public bool Set(string rootPath, string port1, string port2, string port3) {
            var tryget = Get();
            if (tryget != null) {
                Delete();
            }
            var obj = new SyslogModel {
                RootPath = rootPath,
                PortNet1 = port1,
                PortNet2 = port2,
                PortNet3 = port3,
                Services = new Dictionary<string, string>()
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }


        public bool Set(string rootPath, string port1, string port2, string port3, Dictionary<string, string> services ) {
            var tryget = Get();
            if (tryget != null) {
                Delete();
            }
            var obj = new SyslogModel {
                RootPath = rootPath,
                PortNet1 = port1,
                PortNet2 = port2,
                PortNet3 = port3,
                Services = services
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Delete() {
            var result = DatabaseRepository.Delete<SyslogModel>(AntdApplication.Database, Guid.Parse(ConfigGuid));
            return result;
        }
    }
}
