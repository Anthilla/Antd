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
using antdlib.common;
using Newtonsoft.Json;

namespace antdlib.Info {
    public class Proc {

        private static readonly Bash Bash = new Bash();

        private static List<ProcModel> GetAllAllProc() {
            return MapProcJson(JsonConvert.SerializeObject(Bash.Execute("ps -aef").ConvertCommandToModel().output));
        }

        public static List<ProcModel> AllAll => GetAllAllProc();

        private static List<ProcModel> GetAllProc() {
            var list = MapProcJson(JsonConvert.SerializeObject(Bash.Execute("ps -aef").ConvertCommandToModel().output));
            var procs = new List<ProcModel>();
            procs.AddRange(list.Where(p => !p.Cmd.Contains('[')));
            return procs;
        }

        public static List<ProcModel> All => GetAllProc();

        public static List<ProcModel> MapProcJson(string procJson) {
            return (from rowJson in new string[] { } where !string.IsNullOrEmpty(rowJson) let cellDivider = new[] { " " } select rowJson.Split(cellDivider, StringSplitOptions.None).ToArray() into procJsonCell select MapProc(procJsonCell)).ToList();
        }

        public static ProcModel MapProc(string[] procJsonCell) {
            var proc = new ProcModel {
                Uid = procJsonCell[0],
                Pid = procJsonCell[1],
                Ppid = procJsonCell[2],
                C = procJsonCell[3],
                Stime = procJsonCell[4],
                Tty = procJsonCell[5]
            };
            if(procJsonCell.Length > 6) {
                proc.Time = procJsonCell[6];
            }
            if(procJsonCell.Length > 8) {
                proc.Cmd = procJsonCell[7] + " " + procJsonCell[8];
            }
            else if(procJsonCell.Length > 7) {
                proc.Cmd = procJsonCell[7];
            }
            return proc;
        }

        public static string GetPid(string service) {
            var procs = All;
            var proc = (from p in procs
                        where p.Cmd.Contains(service)
                        select p).FirstOrDefault();
            return proc?.Pid;
        }
    }

    public class ProcModel {
        public string Uid { get; set; }
        public string Pid { get; set; }
        public string Ppid { get; set; }
        public string C { get; set; }
        public string Stime { get; set; }
        public string Tty { get; set; }
        public string Time { get; set; }
        public string Cmd { get; set; }
    }
}