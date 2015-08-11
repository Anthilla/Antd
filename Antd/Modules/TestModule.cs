///-------------------------------------------------------------------------------------
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

using Nancy;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Antd {
    public class TMP {
        public string _Id { get; set; }
        public string guid { get; set; }
        public List<int> list { get; set; }
        public string name { get; set; }
        public DateTime date { get; set; }
    }

    public class TestModule : NancyModule {

        public TestModule()
            : base("/test") {
            Get["Test page", "/"] = x => {
                return Response.AsText("Hello World!");
            };

            Get["/page"] = x => {
                return View["page-test"];
            };

            Post["/page"] = x => {
                var o = (string)this.Request.Form.Text;
                var arr = o.Split(new String[] { "/n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                foreach (var a in arr) {
                    Console.WriteLine(a);
                }
                return View["page-test"];
            };

            Get["/ssh"] = x => {
                antdlib.Ssh.Test.Start("10.1.3.194", "root", "root");
                return Response.AsText("gg");
            };

            Get["/e/1"] = x => {
                var model = new TMP();
                model._Id = Guid.NewGuid().ToString();
                model.date = DateTime.Now;
                model.guid = Guid.NewGuid().ToString();
                model.name = model.date.ToString().Substring(0, 5) + model.guid.Substring(5, 5);
                model.list = new List<int>() { };
                DeNSo.Session.New.Set(model);
                return Response.AsXml(model);
            };

            Get["/e/1/{guid}"] = x => {
                string guid = x.guid;
                var model = DeNSo.Session.New.Get<TMP>(m => m.guid == guid).First();
                return Response.AsXml(model);
            };

            Get["/e/2/{guid}"] = x => {
                string guid = x.guid;
                var model = DeNSo.Session.New.Get<TMP>(m => m.guid == guid).First();
                for (int i = 0; i < new Random().Next(1, 21); i++) {
                    model.list.Add(i);
                }
                DeNSo.Session.New.Set(model);
                return Response.AsXml(model);
            };

        }
    }
}