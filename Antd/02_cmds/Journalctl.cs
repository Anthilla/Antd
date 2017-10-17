//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using anthilla.core;

//namespace Antd.cmds {
//    public class Journalctl {

//        public IEnumerable<string> GetAllLog() {
//            var result = Bash.Execute("journalctl --no-pager --quiet").Split();
//            return result;
//        }

//        public IEnumerable<string> GetAllLog(string filter) {
//            var result = Bash.Execute("journalctl --no-pager --quiet").Split().Grep(filter);
//            return result;
//        }

//        public IEnumerable<string> GetAllLogSinceHour(string hours) {
//            var cmd = $"journalctl --no-pager --quiet --since='{hours}h ago'";
//            var result = Bash.Execute(cmd);
//            return result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
//        }

//        public IEnumerable<string> GetAntdLog() {
//            var result = Bash.Execute($"journalctl --no-pager --quiet -u {Parameter.UnitAntdLauncher}");
//            return result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
//        }

//        public IEnumerable<string> GetLogContexts() {
//            var result = Bash.Execute("journalctl --quiet | awk '{print $5}'| awk -F '[' '{print $1}'| sort -u");
//            return result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(value => value.Replace(":", ""));
//        }

//        public IEnumerable<string> GetLogContextsSinceHour(string hours) {
//            var result = Bash.Execute($"journalctl --quiet --since='{hours}h ago' | awk '{{print $5}}'| awk -F '[' '{{print $1}}'| sort -u");
//            return result.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Select(value => value.Replace(":", ""));
//        }

//        public class Report {
//            private static readonly string ReportDir = Parameter.AntdCfgReport;

//            public IEnumerable<string> Get() {
//                DirectoryWithAcl.CreateDirectory(ReportDir, "755", "root", "wheel");
//                return Directory.EnumerateFiles(ReportDir).Where(_ => _.EndsWith("antd-report.txt"));
//            }

//            public void GenerateReport() {
//                DirectoryWithAcl.CreateDirectory(ReportDir, "755", "root", "wheel");
//                var lines = new List<string> {
//                    "+================================+",
//                    $"|    Antd Report @ {DateTime.Now:yyyy-MM-dd}    |",
//                    "+================================+",
//                    "",
//                    Bash.Execute("uname -a"),
//                    $"uptime:           {Bash.Execute("uptime | awk -F ',' '{print $1 $2}'").Trim()}",
//                    $"processes:        {Bash.Execute("ps -aef | wc | awk -F ' ' '{ print $1 }'").Trim()}",
//                    $"users logged:     {Bash.Execute("who | awk -F ' ' '{print $1}' |sort -u | wc |awk -F ' ' '{print $1}'").Trim()}",
//                    $"sessions open:    {Bash.Execute("who | sort -u | wc |awk -F ' ' '{print $1}'").Trim()}",
//                    $"load:             {Bash.Execute("uptime | awk -F ',' '{print $4 $5 $6}' | awk -F ':' '{print $2}'").Trim()}",
//                    ""
//                };
//                lines.AddRange(GetSecurityReport());

//                FileWithAcl.WriteAllLines($"{ReportDir}/{Timestamp.Now}-antd-report.txt", lines, "644", "root", "wheel");
//            }

//            private static IEnumerable<string> GetSecurityReport() {
//                var lines = new List<string> {
//                    "+================================+",
//                    "|    Security Report                 |",
//                    "+================================+",
//                    "",
//                };
//                var sessionSummary = Bash.Execute("aureport -au --summary");
//                var sessionSplitSummary = sessionSummary.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
//                lines.AddRange(from infoRow in sessionSplitSummary.Skip(5) where infoRow.Length > 0 select infoRow.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries) into infoArr select $"{infoArr[0]} authentication requests from user {infoArr[1]}");
//                lines.Add("");
//                var session = Bash.Execute("aureport -au");
//                var sessionSplit = session.Split(new[] { Environment.NewLine }, StringSplitOptions.None).Skip(5).ToList();
//                var ipList = new HashSet<string>();
//                foreach(var infoRow in sessionSplit.Where(_ => _.Length > 0)) {
//                    ipList.Add(infoRow.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[4].Trim());
//                }
//                //formato liena: 67. 11/20/2015 13:26:38 root 10.1.19.1 ssh /usr/sbin/sshd yes 753
//                //lines.Add($"@ {infoArr[1]} {infoArr[2]} - someone tried to authenticate from {infoArr[4]} as {infoArr[3]}: result {infoArr[7]}");
//                foreach(var ip in ipList) {
//                    var ipLines = sessionSplit.Where(line => line.Contains(ip)).ToList();
//                    var userList = new HashSet<string>();
//                    foreach(var infoRow in ipLines.Where(_ => _.Length > 0)) {
//                        userList.Add(infoRow.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[3].Trim());
//                    }
//                    var failureAttempts = ipLines.Where(_ => _.EndsWith("no")).ToList().Count;
//                    var successAttempts = ipLines.Count - failureAttempts;
//                    lines.Add($"Someone tried to authenticate from {ip} as {{{string.Join(", ", userList)}}}, {ipLines.Count} time(s), of which {failureAttempts} failed and {successAttempts} succeeded.");
//                }
//                return lines;
//            }

//            public string ReadReport(string reportFile) {
//                return !File.Exists(reportFile) ? "ERR" : Bash.Execute($"cat {reportFile}");
//            }
//        }
//    }
//}