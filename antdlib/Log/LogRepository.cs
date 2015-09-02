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

using System.Linq;

namespace antdlib.Log {

    public class Logger {

        public static LogModel[] GetAll() {
            var list = DeNSo.Session.New.Get<LogModel>().ToList();
            return (from l in list
                    where l != null
                    orderby l.LogTimestamp descending
                    select l).ToArray();
        }

        public static LogModel[] GetAllMethods() {
            var list = DeNSo.Session.New.Get<LogModel>(l => l.Level == LogModel.EventLevel.InvokedMethod).ToList();
            return (from l in list
                    where l != null
                    orderby l.LogTimestamp descending
                    select l).ToArray();
        }

        public static void TraceEvent(LogModel.EventLevel level, string source, string eventId, string activity, string keyword, string user,
            string opCode, string reg, string sessionId, string relationId, string message) {
            var l = new LogModel() {
                Level = level,
                Source = source,
                EventID = eventId,
                Activity = activity,
                Keyword = keyword,
                User = user,
                OperativeCode = opCode,
                Reg = reg,
                SessionID = sessionId,
                RelationID = relationId,
                Message = message
            };
            DeNSo.Session.New.Set(l);
        }

        public static void TraceMethod(string keyword, string source) {
            var l = new LogModel() {
                Level = LogModel.EventLevel.InvokedMethod,
                Source = source,
                Keyword = keyword
            };
            DeNSo.Session.New.Set(l);
        }

        public static void TraceFileChange(string mode, string file) {
            var l = new LogModel() {
                Mode = mode,
                File = file
            };
            DeNSo.Session.New.Set(l);
        }

        public static void TraceFileChange(string mode, string file, string oldfile) {
            var l = new LogModel() {
                Mode = mode,
                File = file,
                Oldfile = oldfile
            };
            DeNSo.Session.New.Set(l);
        }
    }
}