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
        public string TemplateWord => $"${Label}";
        public string ListType { get; set; }
        public IEnumerable<string> Values { get; set; }
        public string ReplaceValue => $"{{ {string.Join(", ", Values)} }}";
    }

    public class FirewallLists {
        public static IEnumerable<FirewallListModel> GetAll() => DeNSo.Session.New.Get<FirewallListModel>(_ => _.IsEnabled);

        public static IEnumerable<FirewallListModel> GetAllHidden() => DeNSo.Session.New.Get<FirewallListModel>();

        public static IEnumerable<FirewallListModel> GetForRule(string table, string chain, string hook)
            => GetAll().Where(_ => _.IdTable == table && _.IdType == chain && _.IdHook == hook);

        public static void SetDefaultLists() {
            if (!GetAllHidden().Any()) {
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
                    Label = "$ipfiliniifaddr",
                    ListType = "ipv4_addr",
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
                    Label = "$ipfiloutiifaddr",
                    ListType = "ipv4_addr",
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
                    Label = "ipfilfwiifaddr",
                    ListType = "ipv4_addr",
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
                    Label = "ipfilineifaddr",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "ipfilouteifaddr",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfweifaddr",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "",
                    Label = "ipfilinwanifaddr",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "ipfilinprotoset",
                    ListType = "inet_proto",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "ipfiloutprotoset",
                    ListType = "inet_proto",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwprotoset",
                    ListType = "inet_proto",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "ipfilintcpportset",
                    ListType = "inet_service",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwtcpportset",
                    ListType = "inet_service",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "ipfilinudpportset",
                    ListType = "inet_service",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "ipfiloutudpportset",
                    ListType = "inet_service",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwudpportset",
                    ListType = "inet_service",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "ipfilinpubsvcset",
                    ListType = "inet_service",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "ipfilinstate",
                    ListType = "ct_state",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "ipfiloutstate",
                    ListType = "ct_state",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwstate",
                    ListType = "ct_state",
                    Values = new List<string>()
                      },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "ipfilindaddrstate",
                    ListType = "ct_state",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "ipfilinsaddrnetaccept",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "ipfiloutsaddrnetaccept",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "ipfiloutdaddrnetaccept",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "ipfilinsaddripdrop",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "ipfilinsaddrnetaccept",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "ipfiloutsaddrnetaccept",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "ipfiloutdaddrnetaccept",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "ipfiliniif",
                    ListType = "if",
                    Values = new List<string> {"lo"}
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "ipfiloutiif",
                    ListType = "if",
                    Values = new List<string> {"lo"}
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "output",
                    Label = "ipfiloutoif",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwiif",
                    ListType = "if",
                    Values = new List<string> {"lo"}
                },
                 new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwoif",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwoifdrop",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfiliniifdaddraccept",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwiifaccept",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwipoifaccept",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwiptcpiif1",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwiptcpoif1",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwiptcpdaddr1",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwiptcpport1",
                    ListType = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwiptcpiif2",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwiptcpoif2",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwiptcpdaddr2",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "forward",
                    Label = "ipfilfwiptcpport2",
                    ListType = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "filter",
                    IdHook = "input",
                    Label = "ipfilinpubsvcset",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "ipnatpreiif1",
                    ListType = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "ipnatpreport1",
                    ListType = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "ipnatprednat1",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "ipnatpreiif2",
                    ListType = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "ipnatpreport2",
                    ListType = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "ipnatprednat2",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "ipnatpreportredirect",
                    ListType = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "prerouting",
                    Label = "ipnatpreportredirectto",
                    ListType = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "ipnatpostnet1",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "ipnatpostoif1",
                    ListType = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "ipnatpostip1",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "ipnatpostnet2",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "ipnatpostoif2",
                    ListType = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "ipnatpostip2",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "ipnatpostoifmask",
                    ListType = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    IdTable = "ip",
                    IdType = "nat",
                    IdHook = "postrouting",
                    Label = "ipnatpostnetmask",
                    ListType = "net_addr",
                    Values = new List<string>()
                }
            };
            DeNSo.Session.New.SetAll(list);
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
    }
}
