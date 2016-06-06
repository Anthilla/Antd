
using System;
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
using System.Collections.Generic;

namespace antdlib.Collectd {
    public class CollectdItem {
        public long[] values { get; set; }
        public string[] dstypes { get; set; }
        public string[] dsnames { get; set; }
        public double time { get; set; }
        public double interval { get; set; }
        public string host { get; set; }
        public string plugin { get; set; }
        public string plugin_instance { get; set; }
        public string type { get; set; }
        public string type_instance { get; set; }
    }

    public class CollectdMappedItem {
        public string Host { get; set; }
        public DateTime Date { get; set; }
        public string Plugin { get; set; }
        public string PluginInstance { get; set; }
        public string Type { get; set; }
        public string TypeInstance { get; set; }
        public long[] Values { get; set; }
    }

    public class CollectdDBModel {
        public string _Id { get; set; }
        public string Guid { get; set; }
        public string Timestamp { get; set; }
        public List<CollectdItem> Data { get; set; } = new List<CollectdItem>();
    }
}
