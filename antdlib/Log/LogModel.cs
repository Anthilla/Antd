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
using antdlib.Common;

namespace antdlib.Log {
    public class LogModel {
        public enum EventLevel : byte {
            ApiRequest = 0,
            InvokedMethod = 1,
            Error = 99
        }
        public string _Id { get; } = Guid.NewGuid().ToString();
        public string LogGuid { get; } = Guid.NewGuid().ToString();
        public string AntdUid { get; } = AssemblyInfo.Guid;
        public string LogTimestamp { get; set; } = Timestamp.Now;
        public EventLevel Level { get; set; }
        public string Source { get; set; } = "";
        public string EventId { get; set; } = "";
        public string Activity { get; set; } = "";
        public string Keyword { get; set; } = "";
        public string User { get; set; } = "";
        public string OperativeCode { get; set; } = "";
        public string Reg { get; set; } = "";
        public string SessionId { get; set; } = "";
        public string RelationId { get; set; } = "";
        public string Message { get; set; } = "";
        public string Mode { get; set; } = "";
        public string File { get; set; } = "";
        public string Oldfile { get; set; } = "";
    }
}