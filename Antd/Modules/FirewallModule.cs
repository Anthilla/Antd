﻿///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using antdlib.Firewall;
using Nancy;
using System.Collections.Generic;
//using Nancy.Security;
using System.Dynamic;
using System.Linq;

namespace Antd {
    public class FirewallModule : NancyModule {
        public FirewallModule()
            : base("/firewall") {
            //this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.NFTCommands = NFTables.GetNFTCommandsBundle().ToArray();
                return View["_page-firewall", vmod];
            };

            Post["/addrule"] = x => {
                var command = (string)Request.Form.Command;
                var rule = (string)Request.Form.Rule;
                NFTables.AddNFTRule(command, rule);
                return Response.AsRedirect("/firewall");
            };

            Post["/stoprule"] = x => {
                var guid = (string)Request.Form.Guid;
                NFTables.DeleteNFTRule(guid);
                return Response.AsRedirect("/firewall");
            };

            Get["/getrule/{table}/{chain}/{hook}"] = x => {
                var table = x.table;
                var chain = x.chain;
                var hook = x.hook;
                List<string> data = NFTables.GetRulesFromCommand(table, chain, hook);
                return Response.AsJson(data);
            };

            Post["/export"] = x => {
                NFTables.Export.WriteFile();
                return Response.AsJson(true);
            };
        }
    }
}