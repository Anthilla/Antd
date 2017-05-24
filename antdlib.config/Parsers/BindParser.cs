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
using System.Text.RegularExpressions;
using antdlib.models;

namespace antdlib.config.Parsers {
    public class BindParser {

        private static string CaptureGroup(string sourceText, string pattern) {
            var regex = new Regex(pattern);
            var matchedGroups = regex.Match(sourceText).Groups;
            var capturedText = matchedGroups[1].Value;
            return capturedText;
        }

        //(options[\s]*{[\w\d\s;\-{}.*]+[\s]*};)
        public static BindConfigurationModel ParseOptions(BindConfigurationModel bindConfigurationModel, string text) {
            var regex = new Regex("(options[\\s]*{[\\w\\d\\s;\\-{}.*]+[\\s]*};)");
            var matchedGroups = regex.Match(text).Groups;
            var capturedText = matchedGroups[1].Value;
            bindConfigurationModel.Notify = CaptureGroup(capturedText, "[^\\-]notify[\\s]*([\\w]*);");
            bindConfigurationModel.MaxCacheSize = CaptureGroup(capturedText, "max-cache-size[\\s]*([\\w\\d]*);");
            bindConfigurationModel.MaxCacheTtl = CaptureGroup(capturedText, "max-cache-ttl[\\s]*([\\w\\d]*);");
            bindConfigurationModel.MaxNcacheTtl = CaptureGroup(capturedText, "max-ncache-ttl[\\s]*([\\w\\d]*);");
            bindConfigurationModel.Forwarders = CaptureGroup(capturedText, "forwarders[\\s]*{[\\s]*([\\w\\d.; ]*)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();
            bindConfigurationModel.AllowNotify = CaptureGroup(capturedText, "allow-notify[\\s]*{[\\s]*([\\w\\d.; ]*)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();
            bindConfigurationModel.AllowTransfer = CaptureGroup(capturedText, "allow-transfer[\\s]*{[\\s]*([\\w\\d.; ]*)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();
            bindConfigurationModel.Recursion = CaptureGroup(capturedText, "recursion[\\s]*([\\w\\d]*);");
            bindConfigurationModel.TransferFormat = CaptureGroup(capturedText, "transfer-format[\\s]*([\\w\\d\\-]*);");
            bindConfigurationModel.QuerySourceAddress = CaptureGroup(capturedText, "query-source address[\\s]*([\\w\\d\\-*]*) port");
            bindConfigurationModel.QuerySourcePort = CaptureGroup(capturedText, "query-source address[\\s]*[\\w\\d\\-*]* port[\\s]*([\\w\\d\\-*]*);");
            bindConfigurationModel.Version = CaptureGroup(capturedText, "version[\\s]*([\\w\\d]*);");
            bindConfigurationModel.AllowQuery = CaptureGroup(capturedText, "allow-query[\\s]*{[\\s]*([\\w\\d.; ]*)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();
            bindConfigurationModel.AllowRecursion = CaptureGroup(capturedText, "allow-recursion[\\s]*{[\\s]*([\\w\\d.; ]*)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();
            bindConfigurationModel.IxfrFromDifferences = CaptureGroup(capturedText, "ixfr-from-differences[\\s]*([\\w\\d]*);");
            bindConfigurationModel.ListenOnV6 = CaptureGroup(capturedText, "listen-on-v6[\\s]*{[\\s]*([\\w\\d.; ]*)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();
            bindConfigurationModel.ListenOnPort53 = CaptureGroup(capturedText, "listen-on port 53 [\\s]*{[\\s]*([\\w\\d.; ]*)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();
            bindConfigurationModel.DnssecEnabled = CaptureGroup(capturedText, "dnssec-enable[\\s]*([\\w\\d]*);");
            bindConfigurationModel.DnssecValidation = CaptureGroup(capturedText, "dnssec-validation[\\s]*([\\w\\d]*);");
            bindConfigurationModel.DnssecLookaside = CaptureGroup(capturedText, "dnssec-lookaside[\\s]*([\\w\\d]*);");
            bindConfigurationModel.AuthNxdomain = CaptureGroup(capturedText, "auth-nxdomain[\\s]*([\\w\\d]*);");
            return bindConfigurationModel;
        }

        //(controls[\s]*{[\s]*[\w\s\d.]+allow[\s]*{[\s\w\d;]+}[\s]*keys {[\s]*["\w;]+[\s]*};)
        public static BindConfigurationModel ParseControl(BindConfigurationModel bindConfigurationModel, string text) {
            var regex = new Regex("(controls[\\s]*{[\\s]*[\\w\\s\\d.]+allow[\\s]*{[\\s\\w\\d;]+}[\\s]*keys {[\\s]*[\"\\w;]+[\\s]*};)");
            var matchedGroups = regex.Match(text).Groups;
            var capturedText = matchedGroups[1].Value;
            bindConfigurationModel.ControlIp = CaptureGroup(capturedText, "inet[\\s]+([\\d.]+)");
            bindConfigurationModel.ControlPort = CaptureGroup(capturedText, "port[\\s]+([\\d.]+)");
            bindConfigurationModel.ControlAllow = CaptureGroup(capturedText, "allow {[\\s]+([\\w\\d\\s;]+)}").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();
            bindConfigurationModel.ControlKeys = CaptureGroup(capturedText, "keys {[\\s]+([\\w\\d\\s;\"]+)}").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim().Replace("\"", "")).Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();
            return bindConfigurationModel;
        }

        //(key "[\w]+" {[\s\d\w\-;"=]+[\s]+};)
        public static BindConfigurationModel ParseKeySecret(BindConfigurationModel bindConfigurationModel, string text) {
            var regex = new Regex("(key \"[\\w]+\" {[\\s\\d\\w\\-;\"=]+[\\s]+};)");
            var matchedGroups = regex.Match(text).Groups;
            var capturedText = matchedGroups[1].Value;
            bindConfigurationModel.KeyName = CaptureGroup(capturedText, "key \"([\\w]+)\"");
            bindConfigurationModel.KeySecret = CaptureGroup(capturedText, "secret \"([\\s\\S]*)\";");
            return bindConfigurationModel;
        }

        //(channel syslog {[\s]+[\s\d\w\-;]+)
        public static BindConfigurationModel ParseLogging(BindConfigurationModel bindConfigurationModel, string text) {
            var regex = new Regex("(channel syslog {[\\s]+[\\s\\d\\w\\-;]+)");
            var matchedGroups = regex.Match(text).Groups;
            var capturedText = matchedGroups[1].Value;
            bindConfigurationModel.SyslogSeverity = CaptureGroup(capturedText, "[^-]severity[\\s]*([\\w]+);");
            bindConfigurationModel.SyslogPrintCategory = CaptureGroup(capturedText, "print-category[\\s]*([\\w]+);");
            bindConfigurationModel.SyslogPrintSeverity = CaptureGroup(capturedText, "print-severity[\\s]*([\\w]+);");
            bindConfigurationModel.SyslogPrintTime = CaptureGroup(capturedText, "print-time[\\s]*([\\w]+);");
            return bindConfigurationModel;
        }

        //(trusted-keys {[\s]+[\s\d\w\-;."\/+]+};)
        public static BindConfigurationModel ParseTrustedKey(BindConfigurationModel bindConfigurationModel, string text) {
            var regex = new Regex("(trusted-keys {[\\s]+[\\s\\d\\w\\-;.\"\\/+]+};)");
            var matchedGroups = regex.Match(text).Groups;
            var capturedText = matchedGroups[1].Value;
            bindConfigurationModel.TrustedKeys = capturedText;
            return bindConfigurationModel;
        }

        //(acl [\w\d]+ {[\s]*[\w\d\/.;]+[\s]*};)
        public static List<BindConfigurationAcl> ParseAcl(string text) {
            var list = new List<BindConfigurationAcl>();
            var regex = new Regex("(acl [\\w\\d]+ {[\\s]*[\\w\\d\\/.;]+[\\s]*};)");
            var matches = regex.Matches(text);
            foreach(var match in matches) {
                var aclName = CaptureGroup(match.ToString(), "acl ([\\w\\d]+) {");
                var aclInterfaces = CaptureGroup(match.ToString(), "acl [\\w\\d]+ {([\\s]*[\\w\\d\\/.;]+)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();
                var acl = new BindConfigurationAcl {
                    Name = aclName,
                    InterfaceList = aclInterfaces
                };
                list.Add(acl);
            }
            return list;
        }

        //(zone[\s]*"[\w\d\-.]+"[\s]*{[\s]*[\w ;"\/.]*};)
        public static List<BindConfigurationZoneModel> ParseSimpleZones(string text) {
            var list = new List<BindConfigurationZoneModel>();
            var regex = new Regex("(zone[\\s]*\"[\\w\\d\\-.]+\"[\\s]*{[\\s]*[\\w ;\"\\/.]*};)");
            var matches = regex.Matches(text);
            foreach(var match in matches) {
                var name = CaptureGroup(match.ToString(), "zone[\\s]+\"([\\s\\d\\w\\-.]+)\"");
                var type = CaptureGroup(match.ToString(), "type ([\\w]+)");
                var filePath = CaptureGroup(match.ToString(), "file \"([\\w\\d\\/.]+)\"");
                var zone = new BindConfigurationZoneModel {
                    Name = name,
                    Type = type,
                    File = filePath,
                };
                list.Add(zone);
            }
            return list;
        }

        //(zone "[\s\w\d.\-]+" { type[\w\s]+;[\s]+file "[\w\d\/\-.]+";\s[\s]*serial-update[\-\w ]+;[\s]+allow-update { [\w ;]+};[\s]+allow-query { [\w ;]+};[\s]+allow-transfer { [\w ;]+};[\s]+)
        public static List<BindConfigurationZoneModel> ParseComplexZones(string text) {
            var list = new List<BindConfigurationZoneModel>();
            var regex = new Regex("(zone \"[\\s\\w\\d.\\-]+\" { type[\\w\\s]+;[\\s]+file \"[\\w\\d\\/\\-.]+\";\\s[\\s]*serial-update[\\-\\w ]+;[\\s]+allow-update { [\\w ;]+};[\\s]+allow-query { [\\w ;]+};[\\s]+allow-transfer { [\\w ;]+};[\\s]+)");
            var matches = regex.Matches(text);
            foreach(var match in matches) {
                var name = CaptureGroup(match.ToString(), "zone[\\s]+\"([\\s\\d\\w\\-.]+)\"");
                var type = CaptureGroup(match.ToString(), "type ([\\w]+)");
                var filePath = CaptureGroup(match.ToString(), "file \"([\\w\\d\\/.]+)\"");
                var serialUpdateMethod = CaptureGroup(match.ToString(), "serial-update-method[\\s]+([\\w]+)");
                var allowUpdate = CaptureGroup(match.ToString(), "allow-update[\\s]+{[\\s]+([\\w\\s]+);").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();
                var allowQuery = CaptureGroup(match.ToString(), "allow-query[\\s]+{[\\s]+([\\w\\s]+);").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();
                var allowTransfer = CaptureGroup(match.ToString(), "allow-transfer[\\s]+{[\\s]+([\\w\\s]+);").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToList();
                var zone = new BindConfigurationZoneModel {
                    Name = name,
                    Type = type,
                    File = filePath,
                    SerialUpdateMethod = serialUpdateMethod,
                    AllowUpdate = allowUpdate,
                    AllowQuery = allowQuery,
                    AllowTransfer = allowTransfer
                };
                list.Add(zone);
            }
            return list;
        }

        //(include "[\d\w\/\-.]+";)
        public static List<string> ParseInclude(string text) {
            var list = new List<string>();
            var regex = new Regex("(include \"[\\d\\w\\/\\-.]+\";)");
            var matches = regex.Matches(text);
            foreach(var match in matches) {
                var file = CaptureGroup(match.ToString(), "\"([\\d\\w\\/\\-.]+)\"");
                list.Add(file);
            }
            return list;
        }
    }
}
