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
using System.IO;
using System.Linq;
using antdlib.common;
using antdlib.common.Tool;

namespace Antd.Log {
    public class Journalctl {

        private static readonly Bash Bash = new Bash();

        public static IEnumerable<string> GetAllLog() {
            var result = Bash.Execute("journalctl --no-pager --quiet").SplitBash();
            return result;
        }

        public static IEnumerable<string> GetAllLog(string filter) {
            var result = Bash.Execute($"journalctl --no-pager --quiet").SplitBash().Grep(filter);
            return result;
        }

        public static IEnumerable<string> GetAllLogSinceHour(string hours) {
            var cmd = $"journalctl --no-pager --quiet --since='{hours}h ago'";
            var result = Bash.Execute(cmd);
            return result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static IEnumerable<string> GetAntdLog() {
            var result = Bash.Execute($"journalctl --no-pager --quiet -u {Parameter.UnitAntdLauncher}");
            return result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static IEnumerable<string> GetLogContexts() {
            var result = Bash.Execute("journalctl --quiet | awk '{print $5}'| awk -F '[' '{print $1}'| sort -u");
            return result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(value => value.Replace(":", ""));
        }

        public static IEnumerable<string> GetLogContextsSinceHour(string hours) {
            var result = Bash.Execute($"journalctl --quiet --since='{hours}h ago' | awk '{{print $5}}'| awk -F '[' '{{print $1}}'| sort -u");
            return result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(value => value.Replace(":", ""));
        }

        public class Report {
            private static readonly string ReportDir = Parameter.AntdCfgReport;

            public static IEnumerable<string> Get() {
                Directory.CreateDirectory(ReportDir);
                return Directory.EnumerateFiles(ReportDir).Where(_ => _.EndsWith("antd-report.txt"));
            }

            public static void GenerateReport() {
                Directory.CreateDirectory(ReportDir);
                try {
                    var lines = new List<string> {
                        "+================================+",
                        $"|    Antd Report @ {DateTime.Now.ToString("yyyy-MM-dd")}    |",
                        "+================================+",
                        "",
                        Bash.Execute("uname -a"),
                        $"uptime:           {Bash.Execute("uptime | awk -F ',' '{print $1 $2}'").Trim()}",
                        $"processes:        {Bash.Execute("ps -aef | wc | awk -F ' ' '{ print $1 }'").Trim()}",
                        $"users logged:     {Bash.Execute("who | awk -F ' ' '{print $1}' |sort -u | wc |awk -F ' ' '{print $1}'").Trim()}",
                        $"sessions open:    {Bash.Execute("who | sort -u | wc |awk -F ' ' '{print $1}'").Trim()}",
                        $"load:             {Bash.Execute("uptime | awk -F ',' '{print $4 $5 $6}' | awk -F ':' '{print $2}'").Trim()}",
                        ""
                    };
                    lines.AddRange(GetSecurityReport());

                    File.WriteAllLines($"{ReportDir}/{Timestamp.Now}-antd-report.txt", lines);
                }
                catch (Exception ex) {
                    ConsoleLogger.Error($"unable to create the log report: {ex.Message}", ConsoleLogger.Method());
                }
            }

            private static IEnumerable<string> GetSecurityReport() {
                var lines = new List<string> {
                    "+================================+",
                    "|    Security Report                 |",
                    "+================================+",
                    "",
                };
                var sessionSummary = Bash.Execute("aureport -au --summary");
                var sessionSplitSummary = sessionSummary.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
                lines.AddRange(from infoRow in sessionSplitSummary.Skip(5) where infoRow.Length > 0 select infoRow.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries) into infoArr select $"{infoArr[0]} authentication requests from user {infoArr[1]}");
                lines.Add("");
                var session = Bash.Execute("aureport -au");
                var sessionSplit = session.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Skip(5).ToList();
                var ipList = new HashSet<string>();
                foreach (var infoRow in sessionSplit.Where(_ => _.Length > 0)) {
                    ipList.Add(infoRow.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[4].Trim());
                }
                //formato liena: 67. 11/20/2015 13:26:38 root 10.1.19.1 ssh /usr/sbin/sshd yes 753
                //lines.Add($"@ {infoArr[1]} {infoArr[2]} - someone tried to authenticate from {infoArr[4]} as {infoArr[3]}: result {infoArr[7]}");
                foreach (var ip in ipList) {
                    var ipLines = sessionSplit.Where(line => line.Contains(ip)).ToList();
                    var userList = new HashSet<string>();
                    foreach (var infoRow in ipLines.Where(_ => _.Length > 0)) {
                        userList.Add(infoRow.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[3].Trim());
                    }
                    var failureAttempts = ipLines.Where(_ => _.EndsWith("no")).ToList().Count;
                    var successAttempts = ipLines.Count - failureAttempts;
                    lines.Add($"Someone tried to authenticate from {ip} as {{{string.Join(", ", userList)}}}, {ipLines.Count} time(s), of which {failureAttempts} failed and {successAttempts} succeeded.");
                }
                return lines;
            }

            public static string ReadReport(string reportFile) {
                return !File.Exists(reportFile) ? "ERR" : Bash.Execute($"cat {reportFile}");
            }
        }
    }
}