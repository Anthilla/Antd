using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using anthilla.core;

namespace Antd.cmds {
    public class Journalctl {

        private const string journalctlFileLocation = "/usr/bin/journalctl";
        private const string journalctlOptions = "--no-pager --quiet";

        public static string[] GetLog() {
            return CommonProcess.Execute(journalctlFileLocation, journalctlOptions).ToArray();
        }

        public static string[] GetUnitLog(string unitName) {
            var args = CommonString.Append(journalctlOptions, " -u ", unitName);
            return CommonProcess.Execute(journalctlFileLocation, args).ToArray();
        }

        public static string[] GetAntdLog() {
            return GetUnitLog("app-antd-03-launcher");
        }

        public static string[] GetAntdUiLog() {
            return GetUnitLog("app-antdui-03-launcher");
        }

        public static string[] GetLastHours(int hours) {
            var args = CommonString.Append(journalctlOptions, " --since='", hours.ToString(), "h ago'");
            return CommonProcess.Execute(journalctlFileLocation, args).ToArray();
        }

        public class Report {
            private static readonly string ReportDir = "/cfg/antd/reports";

            public IEnumerable<string> Get() {
                Directory.CreateDirectory(ReportDir);
                return Directory.EnumerateFiles(ReportDir).Where(_ => _.EndsWith("antd-report.txt"));
            }

            public void GenerateReport() {
                Directory.CreateDirectory(ReportDir);
                var lines = new List<string> {
                    "+================================+",
                    $"|    Antd Report @ {DateTime.Now:yyyy-MM-dd}    |",
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
                foreach(var infoRow in sessionSplit.Where(_ => _.Length > 0)) {
                    ipList.Add(infoRow.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[4].Trim());
                }
                //formato liena: 67. 11/20/2015 13:26:38 root 10.1.19.1 ssh /usr/sbin/sshd yes 753
                //lines.Add($"@ {infoArr[1]} {infoArr[2]} - someone tried to authenticate from {infoArr[4]} as {infoArr[3]}: result {infoArr[7]}");
                foreach(var ip in ipList) {
                    var ipLines = sessionSplit.Where(line => line.Contains(ip)).ToList();
                    var userList = new HashSet<string>();
                    foreach(var infoRow in ipLines.Where(_ => _.Length > 0)) {
                        userList.Add(infoRow.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)[3].Trim());
                    }
                    var failureAttempts = ipLines.Where(_ => _.EndsWith("no")).ToList().Count;
                    var successAttempts = ipLines.Count - failureAttempts;
                    lines.Add($"Someone tried to authenticate from {ip} as {{{string.Join(", ", userList)}}}, {ipLines.Count} time(s), of which {failureAttempts} failed and {successAttempts} succeeded.");
                }
                return lines;
            }

            public string ReadReport(string reportFile) {
                return !File.Exists(reportFile) ? "ERR" : Bash.Execute($"cat {reportFile}");
            }
        }
    }
}