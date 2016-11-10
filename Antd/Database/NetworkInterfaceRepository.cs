using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class NetworkInterfaceRepository {
        private const string ViewName = "NetworkInterface";

        public IEnumerable<NetworkInterfaceSchema> GetAll() {
            var result = DatabaseRepository.Query<NetworkInterfaceSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public NetworkInterfaceSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<NetworkInterfaceSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }

        public bool Create(IDictionary<string, string> dict) {
            var name = dict["Name"];
            var type = dict["Type"];
            var obj = new NetworkInterfaceModel {
                Name = name,
                Type = (NetworkInterfaceType)Enum.Parse(typeof(NetworkInterfaceType), type)
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var name = dict["Name"];
            var type = dict["Type"];
            var objUpdate = new NetworkInterfaceModel {
                Id = id.ToGuid(),
                Name = name.IsNullOrEmpty() ? null : name,
                Type = (NetworkInterfaceType)Enum.Parse(typeof(NetworkInterfaceType), type)
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }

        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<NetworkInterfaceModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }

        private void FlushDb() {
            foreach(var obj in GetAll()) {
                Delete(obj.Id);
            }
        }

        private readonly Bash _bash = new Bash();

        public void Import() {
            FlushDb();
            if(!Parameter.IsUnix)
                return;
            var dirs = Directory.GetDirectories("/sys/class/net");
            var physical = from dir in dirs
                           let f = _bash.Execute($"file {dir}")
                           where !f.Contains("virtual") && !f.Contains("fake")
                           select Path.GetFileName(dir);
            foreach(var iface in physical) {
                Create(new Dictionary<string, string> {
                    {"Name", iface},
                    {"Type", NetworkInterfaceType.Physical.ToString()}
                });
            }
            var virtualIf = (from dir in dirs
                             let f = _bash.Execute($"file {dir}")
                             where f.Contains("virtual") || f.Contains("fake")
                             select Path.GetFileName(dir)).Where(_ => !_.Contains("bond"));
            foreach(var iface in virtualIf) {
                Create(new Dictionary<string, string> {
                    {"Name", iface},
                    {"Type", NetworkInterfaceType.Virtual.ToString()}
                });
            }
            var bondIf = (from dir in dirs
                          let f = _bash.Execute($"file {dir}")
                          where f.Contains("virtual") || f.Contains("fake")
                          select Path.GetFileName(dir)).Where(_ => _.Contains("bond"));
            foreach(var iface in bondIf) {
                Create(new Dictionary<string, string> {
                    {"Name", iface},
                    {"Type", NetworkInterfaceType.Bond.ToString()}
                });
            }
            var bridgeIf = _bash.Execute("brctl show").Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Skip(1).ToList();
            var brList = bridgeIf.Select(bbrr => bbrr.Replace("\t", " ").Replace("/t", " ").Replace("  ", " ").Split(' ')[0]).Select(brAttr => brAttr.Trim()).ToList();
            foreach(var iface in brList) {
                Create(new Dictionary<string, string> {
                    {"Name", iface},
                    {"Type", NetworkInterfaceType.Bridge.ToString()}
                });
            }
        }
    }
}
