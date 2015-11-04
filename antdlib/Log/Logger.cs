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
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace antdlib.Log {

    public enum EventLevel : byte {
        Info = 0,
        Warn = 1,
        Error = 2,
        ApiRequest = 98,
        InvokedMethod = 99
    }

    public class LogModel {
        [Key]
        public string _Id { get; } = Guid.NewGuid().ToString();
        public string LogGuid { get; } = Guid.NewGuid().ToString();
        public DateTime DateTime { get; set; }
        public EventLevel Level { get; set; }
        public string User { get; set; }
        public string Session { get; set; }
        public string AnthillaId { get; set; }
        public string EventId { get; set; }
        public string EventName { get; set; }
        public string Message { get; set; }
    }

    public class Logger {

        public static IEnumerable<LogModel> GetAll() {
            return DeNSo.Session.New.Get<LogModel>().OrderByDescending(_ => _.DateTime);
        }

        public LogModel GetById(string id) {
            return DeNSo.Session.New.Get<LogModel>(c => c.LogGuid == id).FirstOrDefault();
        }

        public static void Trace(string user, string eventName, EventLevel level, string message) {
            try {
                var logItem = new LogModel {
                    DateTime = DateTime.Now,
                    User = user,
                    Session = "",
                    AnthillaId = "",
                    Level = level,
                    EventId = Guid.NewGuid().ToString(),
                    EventName = eventName,
                    Message = message
                };
                DeNSo.Session.New.Set(logItem);
            }
            catch (Exception) { }
        }
    }
}