//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace antdlib.Firewall {
    public class FirewallListModel {
        public string _Id { get; set; }
        public string Guid { get; set; }
        public bool IsEnabled { get; set; }
        public string IdTable { get; set; }
        public string IdType { get; set; }
        public string IdHook { get; set; }
        public string Rule { get; set; }
        public string Label { get; set; }
        public string TemplateWord => $"${IdTable}_{IdType}_{IdHook}_{Label}";
        public IEnumerable<string> Values { get; set; }
        public string ReplaceValues => string.Join(", ", Values);
        public string ReplaceTemplateValues => $"{{ {string.Join(", ", Values)} }}";
    }

    public class FirewallLists {
        public static IEnumerable<FirewallListModel> GetAll() => DeNSo.Session.New.Get<FirewallListModel>(_ => _.IsEnabled);

        public static IEnumerable<FirewallListModel> GetAllHidden() => DeNSo.Session.New.Get<FirewallListModel>();

        public static IEnumerable<FirewallListModel> GetForRule(string table, string type, string hook) {
            var l = GetAll().Where(_ => _.IdTable == table && _.IdType == type && _.IdHook == hook);
            return l;
        }

        public static IEnumerable<string> GetRuleSet(string table, string type, string hook) {
            var startMark = $"#start_{table}_{type}_{hook}";
            var endMark = $"#end_{table}_{type}_{hook}";
            var templateTextLines = File.ReadAllLines($"{Parameter.RepoConfig}/antd.firewall.template.conf").ToList();
            var startIndex = templateTextLines.FindIndex(_ => _.Contains(startMark));
            var endIndex = templateTextLines.FindIndex(_ => _.Contains(endMark));
            var relevantLines = templateTextLines.Skip(startIndex).Take(endIndex - startIndex);
            return relevantLines.Where(_ => !_.Contains("#"));
        }

        public static void AddList(string guid, string table, string type, string hook, string label) {
            var model = new FirewallListModel {
                _Id = Guid.NewGuid().ToString(),
                Guid = guid,
                IsEnabled = true,
                IdTable = table,
                IdType = type,
                IdHook = hook,
                Label = label,
                Values = new List<string>()
            };
            DeNSo.Session.New.Set(model);
        }

        public static void AddValueToList(string guid, IEnumerable<string> values) {
            if (!GetAllHidden().Any()) {
                return;
            }
            var list = GetAllHidden().FirstOrDefault(_ => _.Guid == guid);
            if (list == null)
                return;
            list.Values = values;
            DeNSo.Session.New.Set(list);
        }

        public static IEnumerable<string> GetLabels() => GetAllHidden().Select(_ => _.TemplateWord);

        public static void SetDefaultLists() {
            if (GetAllHidden().Any()) {
                return;
            }
            var list = new List<FirewallListModel> {
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "iifaddr",
                    Values = new List<string> {
                        "127.0.0.1"
                    }
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "iifaddr",
                    Values = new List<string> {
                        "127.0.0.1"
                    }
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "iifaddr",
                    Values = new List<string> {
                        "127.0.0.1"
                    }
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "eifaddr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "eifaddr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "eifaddr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "",
                    Label = "wanifaddr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "protoset",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "protoset",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "protoset",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "tcpportset",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "tcpportset",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "udpportset",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "udpportset",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "udpportset",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "pubsvcset",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "state",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "state",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "state",
                    Values = new List<string>()
                      },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "daddrstate",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "saddrnetaccept",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "saddrnetaccept",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "daddrnetaccept",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "saddripdrop",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "saddrnetaccept",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "saddrnetaccept",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "daddrnetaccept",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "iif",
                    Values = new List<string> {"lo"}
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "iif",
                    Values = new List<string> {"lo"}
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "oif",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "iif",
                    Values = new List<string> {"lo"}
                },
                 new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "oif",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "oifdrop",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "iifdaddraccept",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "iifaccept",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipoifaccept",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "iptcpiif1",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "iptcpoif1",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "iptcpdaddr1",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "iptcpport1",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "iptcpiif2",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "iptcpoif2",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "iptcpdaddr2",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "iptcpport2",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "pubsvcset",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "iif1",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "port1",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "dnat1",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "iif2",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "port2",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "dnat2",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "portredirect",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "portredirectto",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "net1",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "oif1",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "ip1",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "net2",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "oif2",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "ip2",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "oifmask",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "netmask",
                    Values = new List<string>()
                }
            };
            DeNSo.Session.New.SetAll(list);
        }
    }
}
