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
using System.Reflection;
using antdlib.Ssh;
using antdlib;
using Newtonsoft.Json;
using antdlib.Collectd;

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

            Before += y => {
                return null;
            };

            After += y => {
            };

            Get["Test page", "/"] = x => {
                return Response.AsText("Hello World!");
            };

            Get["/page"] = x => {
                return View["page-test"];
            };

            Post["/page"] = x => {
                var o = (string)Request.Form.Text;
                var arr = o.Split(new String[] { "/n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                foreach (var a in arr) {
                    Console.WriteLine(a);
                }
                return View["page-test"];
            };

            Post["/post-collectd"] = x => {
                var collectdBody = Request.Body.ReadAsString();
                var listCollectdLog = CollectdRepo.ImportCollectdData(collectdBody);
                Console.WriteLine($"{listCollectdLog.Count} collectd entries");
                return Response.AsJson(true);
            };
        }
    }
}