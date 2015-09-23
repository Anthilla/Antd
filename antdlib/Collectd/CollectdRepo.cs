
using Newtonsoft.Json;
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace antdlib.Collectd {
    public class CollectdRepo {
        public static List<CollectdMappedItem> MapCollectdData(string json) {
            var list = new List<CollectdMappedItem>() { };
            var collectdEntries = JsonConvert.DeserializeObject<List<CollectdItem>>(json);
            foreach (var entry in collectdEntries) {
                var m = MapItem(entry);
                list.Add(m);
            }
            return list;
        }

        public static List<CollectdItem> ImportCollectdData(string json) {
            var collectdEntries = JsonConvert.DeserializeObject<List<CollectdItem>>(json);
            var model = new CollectdDBModel() {
                _Id = Guid.NewGuid().ToString(),
                Guid = Guid.NewGuid().ToString(),
                Timestamp = Timestamp.Now
            };
            model.Data = collectdEntries;
            DeNSo.Session.New.Set(model);
            return collectdEntries;
        }

        public static List<CollectdDBModel> GetAllData() {
            return DeNSo.Session.New.Get<CollectdDBModel>(d => d != null).ToList();
        }

        public static CollectdDBModel GetLast() {
            return DeNSo.Session.New.Get<CollectdDBModel>(d => d != null).OrderByDescending(d => d.Timestamp).FirstOrDefault();
        }

        private static CollectdMappedItem MapItem(CollectdItem model) {
            var map = new CollectdMappedItem() {
                Host = model.host,
                Plugin = model.plugin,
                PluginInstance = model.plugin_instance,
                Type = model.type,
                TypeInstance = model.type_instance,
                Values = model.values
            };
            map.Date = Timestamp.ConvertUnixToDateTime(model.time);
            return map;
        }
    }
}
