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
using System.Net;
using System.Net.Sockets;
using System.ServiceModel.Syndication;
using Nancy;

namespace Antd.Modules {

    public class FeedModule : CoreModule {
        public FeedModule() {
            Get["/feed"] = x => {
                var feed = new SyndicationFeed { Title = new TextSyndicationContent("Directory Watcher Feed") };
                feed.Authors.Add(new SyndicationPerson("info@anthilla.com"));
                feed.Categories.Add(new SyndicationCategory("Directory Watcher Feed"));
                feed.Description = new TextSyndicationContent("Directory Watcher Feed");
                var item1 = new SyndicationItem(
                    "Item One",
                    "This is the content for item one",
                    new Uri("http://localhost/Content/One"),
                    "ItemOneID",
                    DateTime.Now);
                feed.Items = new List<SyndicationItem> { item1 };
                return new RssResponse(feed);
            };

            Get["/feed2"] = x => {
                var feed = new SyndicationFeed { Title = new TextSyndicationContent("AnthillaSP Ticket") };
                feed.Authors.Add(new SyndicationPerson("info@anthilla.com"));
                feed.Categories.Add(new SyndicationCategory("AnthillaSP Ticket"));
                feed.Description = new TextSyndicationContent("AnthillaSP Ticket");

                var directories = new List<string>();
                var list = new List<SyndicationItem>();

                foreach (var dir in directories) {
                    var f = new SyndicationItem(
                    dir,
                    dir,
                    new Uri("http://localhost/feed"),
                    Guid.NewGuid().ToString(),
                    DateTime.Now);
                    list.Add(f);
                }

                feed.Items = list;
                return new RssResponse(feed);
            };

            Get["/hello"] = x => {
                var interNetwork = new List<string>();
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList) {
                    if (ip.AddressFamily == AddressFamily.InterNetwork) {
                        interNetwork.Add(ip.ToString());
                    }
                }
                return Response.AsJson(interNetwork);
            };
        }
    }
}