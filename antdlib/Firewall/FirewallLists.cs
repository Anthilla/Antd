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
        public string TemplateWord { get; set; }
        public string Label { get; set; }
        public string Type { get; set; }
        public IEnumerable<string> Values { get; set; }
        public string ReplaceValue => $"{{ {string.Join(", ", Values)} }}";
    }

    public class FirewallLists {
        public static IEnumerable<FirewallListModel> GetAll() => DeNSo.Session.New.Get<FirewallListModel>(_ => _.IsEnabled);

        public static IEnumerable<FirewallListModel> GetAllHidden() => DeNSo.Session.New.Get<FirewallListModel>();

        public static void SetDefaultLists() {
            if (!GetAllHidden().Any()) {
                return;
            }
            var list = new List<FirewallListModel> {
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$iifaddr$",
                    Label = "iifaddr",
                    Type = "ipv4_addr",
                    Values = new List<string> {
                        "127.0.0.1"
                    }
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$eifaddr$",
                    Label = "eifaddr",
                    Type = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$wanifaddr$",
                    Label = "wanifaddr",
                    Type = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$protoset$",
                    Label = "protoset",
                    Type = "inet_proto",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$tcpportset$",
                    Label = "tcpportset",
                    Type = "inet_service",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$udpportset$",
                    Label = "udpportset",
                    Type = "inet_service",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$pubsvcset$",
                    Label = "pubsvcset",
                    Type = "inet_service",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilinstate$",
                    Label = "ipfilinstate",
                    Type = "ct_state",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfiloutstate$",
                    Label = "ipfiloutstate",
                    Type = "ct_state",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilfwstate$",
                    Label = "ipfilfwstate",
                    Type = "ct_state",
                    Values = new List<string>()
                      },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilinipdaddrstate$",
                    Label = "ipfilinipdaddrstate",
                    Type = "ct_state",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilinipdsaddrnetaccept$",
                    Label = "ipfilinipdsaddrnetaccept",
                    Type = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilinipddaddrnetaccept$",
                    Label = "ipfilinipddaddrnetaccept",
                    Type = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilinipdsaddripdrop$",
                    Label = "ipfilinipdsaddripdrop",
                    Type = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfiloutipdsaddrnetaccept$",
                    Label = "ipfiloutipdsaddrnetaccept",
                    Type = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfiloutipddaddrnetaccept$",
                    Label = "ipfiloutipddaddrnetaccept",
                    Type = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilinipiif$",
                    Label = "ipfilinipiif",
                    Type = "if",
                    Values = new List<string> {"lo"}
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfiloutipiif$",
                    Label = "ipfiloutipiif",
                    Type = "if",
                    Values = new List<string> {"lo"}
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfiloutipoif$",
                    Label = "ipfiloutipoif",
                    Type = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilfwipiif$",
                    Label = "ipfilfwipiif",
                    Type = "if",
                    Values = new List<string> {"lo"}
                },
                 new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilfwipoif$",
                    Label = "ipfilfwipoif",
                    Type = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilfwipoifdrop$",
                    Label = "ipfilfwipoifdrop",
                    Type = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilfwipiifdaddraccept$",
                    Label = "ipfilfwipiifdaddraccept",
                    Type = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilfwipiifaccept$",
                    Label = "ipfilfwipiifaccept",
                    Type = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilfwipoifaccept$",
                    Label = "ipfilfwipoifaccept",
                    Type = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilfwiptcpiif1$",
                    Label = "ipfilfwiptcpiif1",
                    Type = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilfwiptcpoif1$",
                    Label = "ipfilfwiptcpoif1",
                    Type = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilfwiptcpdaddr1$",
                    Label = "ipfilfwiptcpdaddr1",
                    Type = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilfwiptcpport1$",
                    Label = "ipfilfwiptcpport1",
                    Type = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilfwiptcpiif2$",
                    Label = "ipfilfwiptcpiif2",
                    Type = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilfwiptcpoif2$",
                    Label = "ipfilfwiptcpoif2",
                    Type = "if",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilfwiptcpdaddr2$",
                    Label = "ipfilfwiptcpdaddr2",
                    Type = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipfilfwiptcpport2$",
                    Label = "ipfilfwiptcpport2",
                    Type = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$pubsvcset$",
                    Label = "pubsvcset",
                    Type = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$pubsvcset$",
                    Label = "pubsvcset",
                    Type = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatpreiif1$",
                    Label = "ipnatpreiif1",
                    Type = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatpreport1$",
                    Label = "ipnatpreport1",
                    Type = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatprednat1$",
                    Label = "ipnatprednat1",
                    Type = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatpreiif2$",
                    Label = "ipnatpreiif2",
                    Type = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatpreport2$",
                    Label = "ipnatpreport2",
                    Type = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatprednat2$",
                    Label = "ipnatprednat2",
                    Type = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatpreportredirect$",
                    Label = "ipnatpreportredirect",
                    Type = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatpreportredirectto$",
                    Label = "ipnatpreportredirectto",
                    Type = "port",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatpostnet1$",
                    Label = "ipnatpostnet1",
                    Type = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatpostoif1$",
                    Label = "ipnatpostoif1",
                    Type = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatpostip1$",
                    Label = "ipnatpostip1",
                    Type = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatpostnet2$",
                    Label = "ipnatpostnet2",
                    Type = "net_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatpostoif2$",
                    Label = "ipnatpostoif2",
                    Type = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatpostip2$",
                    Label = "ipnatpostip2",
                    Type = "ipv4_addr",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatpostoifmask$",
                    Label = "ipnatpostoifmask",
                    Type = "if_name",
                    Values = new List<string>()
                },
                new FirewallListModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    IsEnabled = true,
                    TemplateWord = "$ipnatpostnetmask$",
                    Label = "ipnatpostnetmask",
                    Type = "net_addr",
                    Values = new List<string>()
                }
            };
            DeNSo.Session.New.SetAll(list);
        }

        public static void AddValueToList(string guid, string value) {
            if (!GetAllHidden().Any()) {
                return;
            }
            var list = GetAllHidden().FirstOrDefault(_ => _.Guid == guid);
            if (list == null)
                return;
            var els = list.Values.ToList();
            els.Add(value);
            list.Values = els;
            DeNSo.Session.New.Set(list);
        }

        public static void RemoveValueFromList(string guid, string value) {
            if (!GetAllHidden().Any()) {
                return;
            }
            var list = GetAllHidden().FirstOrDefault(_ => _.Guid == guid);
            if (list == null)
                return;
            var els = list.Values.ToList();
            els.Remove(value);
            list.Values = els;
            DeNSo.Session.New.Set(list);
        }


    }
}
