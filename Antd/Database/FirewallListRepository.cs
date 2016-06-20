using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using antdlib.views;
using antdlib.views.Repo;
namespace Antd.Database {
    public class FirewallListRepository {
        private const string ViewName = "FirewallList";
        public IEnumerable<FirewallListSchema> GetAll() {
            var result = DatabaseRepository.Query<FirewallListSchema>(AntdApplication.Database, ViewName).ToList();
            result.AddRange(Defaults());
            return result;
        }
        public FirewallListSchema GetByGuid(string guid) {
            var result = DatabaseRepository.Query<FirewallListSchema>(AntdApplication.Database, ViewName, schema => schema.Id == guid || schema.Guid == guid);
            return result.FirstOrDefault();
        }
        public bool Create(IDictionary<string, string> dict) {
            var tableId = dict["TableId"];
            var typeId = dict["TypeId"];
            var hookId = dict["HookId"];
            var rule = dict["Rule"];
            var label = dict["Label"];
            var values = dict["Values"];
            var obj = new FirewallListModel {
                TableId = tableId,
                TypeId = typeId,
                HookId = hookId,
                Rule = rule,
                Label = label,
                Values = values.SplitToList(),
                IsEnabled = true
            };
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }
        public bool Edit(IDictionary<string, string> dict) {
            var id = dict["Id"];
            var tableId = dict["TableId"];
            var typeId = dict["TypeId"];
            var hookId = dict["HookId"];
            var rule = dict["Rule"];
            var label = dict["Label"];
            var values = dict["Values"];
            var isEnabled = dict["IsEnabled"];
            var objUpdate = new FirewallListModel {
                Id = id.ToGuid(),
                TableId = tableId.IsNullOrEmpty() ? null : tableId,
                TypeId = typeId.IsNullOrEmpty() ? null : typeId,
                HookId = hookId.IsNullOrEmpty() ? null : hookId,
                Rule = rule.IsNullOrEmpty() ? null : rule,
                Label = label.IsNullOrEmpty() ? null : label,
                Values = values.IsNullOrEmpty() ? new List<string>() : values.SplitToList(),
                IsEnabled = isEnabled.IsNullOrEmpty() ? null : isEnabled.ToBoolean()
            };
            var result = DatabaseRepository.Edit(AntdApplication.Database, objUpdate, true);
            return result;
        }
        public bool Delete(string guid) {
            var result = DatabaseRepository.Delete<FirewallListModel>(AntdApplication.Database, Guid.Parse(guid));
            return result;
        }
        public IEnumerable<string> GetLabels() {
            var result = GetAll().Select(_ => _.TemplateWord);
            return result;
        }
        public IEnumerable<FirewallListSchema> GetForRule(string table, string type, string hook) {
            var result = GetAll().Where(_ => _.TableId == table && _.TypeId == type && _.HookId == hook);
            return result;
        }
        #region Defaults()
        private static List<FirewallListSchema> Defaults() {
            var list = new List<FirewallListSchema> {
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "input",
                    Label = "iifaddr",
                    Values = "127.0.0.1"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "output",
                    Label = "iifaddr",
                    Values = "127.0.0.1"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "iifaddr",
                    Values = "127.0.0.1"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "input",
                    Label = "eifaddr",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "output",
                    Label = "eifaddr",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "eifaddr",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "",
                    Label = "wanifaddr",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "input",
                    Label = "protoset",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "output",
                    Label = "protoset",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "protoset",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "input",
                    Label = "tcpportset",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "tcpportset",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "input",
                    Label = "udpportset",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "output",
                    Label = "udpportset",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "udpportset",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "input",
                    Label = "pubsvcset",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "input",
                    Label = "state",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "output",
                    Label = "state",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "state",
                    Values = ""
                      },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "input",
                    Label = "daddrstate",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "input",
                    Label = "saddrnetaccept",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "output",
                    Label = "saddrnetaccept",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "output",
                    Label = "daddrnetaccept",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "input",
                    Label = "saddripdrop",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "input",
                    Label = "saddrnetaccept",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "output",
                    Label = "saddrnetaccept",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "output",
                    Label = "daddrnetaccept",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "input",
                    Label = "iif",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "output",
                    Label = "iif",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "output",
                    Label = "oif",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "iif",
                    Values = "lo"
                },
                 new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "oif",
                    Values = ""
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "oifdrop",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "iifdaddraccept",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "iifaccept",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "ipoifaccept",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "iptcpiif1",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "iptcpoif1",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "iptcpdaddr1",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "iptcpport1",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "iptcpiif2",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "iptcpoif2",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "iptcpdaddr2",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "forward",
                    Label = "iptcpport2",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "filter",
                    HookId = "input",
                    Label = "pubsvcset",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "prerouting",
                    Label = "iif1",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "prerouting",
                    Label = "port1",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "prerouting",
                    Label = "dnat1",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "prerouting",
                    Label = "iif2",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "prerouting",
                    Label = "port2",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "prerouting",
                    Label = "dnat2",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "prerouting",
                    Label = "portredirect",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "prerouting",
                    Label = "portredirectto",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "postrouting",
                    Label = "net1",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "postrouting",
                    Label = "oif1",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "postrouting",
                    Label = "ip1",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "postrouting",
                    Label = "net2",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "postrouting",
                    Label = "oif2",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "postrouting",
                    Label = "ip2",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "postrouting",
                    Label = "oifmask",
                    Values = "lo"
                },
                new FirewallListSchema {
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TableId = "ip",
                    TypeId = "nat",
                    HookId = "postrouting",
                    Label = "netmask",
                    Values = "lo"
                }
            };
            return list;
        }
        #endregion
    }
}
