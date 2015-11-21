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
        public string IdChain { get; set; }
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
            => GetAll().Where(_=>_.IdTable == table && _.IdChain == chain && _.IdHook == hook);

        public static void SetDefaultLists() {
            if (!GetAllHidden().Any()) {
                return;
            }
            var list = new List<FirewallListModel> {
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "iifaddr",
                    ListType = "ipv4_addr",
                    Values = new List<string> {
                        "127.0.0.1"
                    }
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "eifaddr",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "wanifaddr",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "protoset",
                    ListType = "inet_proto",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "tcpportset",
                    ListType = "inet_service",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "udpportset",
                    ListType = "inet_service",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "pubsvcset",
                    ListType = "inet_service",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilinstate",
                    ListType = "ct_state",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfiloutstate",
                    ListType = "ct_state",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilfwstate",
                    ListType = "ct_state",
                    Values = new List<string>()
                      },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilinipdaddrstate",
                    ListType = "ct_state",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilinipdsaddrnetaccept",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilinipddaddrnetaccept",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilinipdsaddripdrop",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfiloutipdsaddrnetaccept",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfiloutipddaddrnetaccept",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilinipiif",
                    ListType = "if",
                    Values = new List<string> {"lo"}
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfiloutipiif",
                    ListType = "if",
                    Values = new List<string> {"lo"}
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfiloutipoif",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilfwipiif",
                    ListType = "if",
                    Values = new List<string> {"lo"}
                },
                 new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilfwipoif",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilfwipoifdrop",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilfwipiifdaddraccept",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilfwipiifaccept",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilfwipoifaccept",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilfwiptcpiif1",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilfwiptcpoif1",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilfwiptcpdaddr1",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilfwiptcpport1",
                    ListType = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilfwiptcpiif2",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilfwiptcpoif2",
                    ListType = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilfwiptcpdaddr2",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipfilfwiptcpport2",
                    ListType = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "pubsvcset",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "pubsvcset",
                    ListType = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipnatpreiif1",
                    ListType = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipnatpreport1",
                    ListType = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipnatprednat1",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipnatpreiif2",
                    ListType = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipnatpreport2",
                    ListType = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipnatprednat2",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipnatpreportredirect",
                    ListType = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipnatpreportredirectto",
                    ListType = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipnatpostnet1",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipnatpostoif1",
                    ListType = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipnatpostip1",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipnatpostnet2",
                    ListType = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipnatpostoif2",
                    ListType = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipnatpostip2",
                    ListType = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    Label = "ipnatpostoifmask",
                    ListType = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
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
