using Antd2.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Antd2.parsing {
    public class BindParser {

        private static string CaptureGroup(string sourceText, string pattern) {
            var regex = new Regex(pattern);
            var matchedGroups = regex.Match(sourceText).Groups;
            var capturedText = matchedGroups[1].Value;
            return capturedText;
        }

        //(options[\s]*{[\w\d\s;\-{}.*]+[\s]*};)
        public static BindModel ParseOptions(BindModel bindConfigurationModel, string text) {
            var regex = new Regex("(options[\\s]*{[\\w\\d\\s;\\-{}.*]+[\\s]*};)");
            var matchedGroups = regex.Match(text).Groups;
            var capturedText = matchedGroups[1].Value;
            bindConfigurationModel.Notify = CaptureGroup(capturedText, "[^\\-]notify[\\s]*([\\w]*);");
            bindConfigurationModel.MaxCacheSize = CaptureGroup(capturedText, "max-cache-size[\\s]*([\\w\\d]*);");
            bindConfigurationModel.MaxCacheTtl = CaptureGroup(capturedText, "max-cache-ttl[\\s]*([\\w\\d]*);");
            bindConfigurationModel.MaxNcacheTtl = CaptureGroup(capturedText, "max-ncache-ttl[\\s]*([\\w\\d]*);");
            bindConfigurationModel.Forwarders = CaptureGroup(capturedText, "forwarders[\\s]*{[\\s]*([\\w\\d.; ]*)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToArray();
            bindConfigurationModel.AllowNotify = CaptureGroup(capturedText, "allow-notify[\\s]*{[\\s]*([\\w\\d.; ]*)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToArray();
            bindConfigurationModel.AllowTransfer = CaptureGroup(capturedText, "allow-transfer[\\s]*{[\\s]*([\\w\\d.; ]*)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToArray();
            bindConfigurationModel.Recursion = CaptureGroup(capturedText, "recursion[\\s]*([\\w\\d]*);");
            bindConfigurationModel.TransferFormat = CaptureGroup(capturedText, "transfer-format[\\s]*([\\w\\d\\-]*);");
            bindConfigurationModel.QuerySourceAddress = CaptureGroup(capturedText, "query-source address[\\s]*([\\w\\d\\-*]*) port");
            bindConfigurationModel.QuerySourcePort = CaptureGroup(capturedText, "query-source address[\\s]*[\\w\\d\\-*]* port[\\s]*([\\w\\d\\-*]*);");
            bindConfigurationModel.Version = CaptureGroup(capturedText, "version[\\s]*([\\w\\d]*);");
            bindConfigurationModel.AllowQuery = CaptureGroup(capturedText, "allow-query[\\s]*{[\\s]*([\\w\\d.; ]*)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToArray();
            bindConfigurationModel.AllowRecursion = CaptureGroup(capturedText, "allow-recursion[\\s]*{[\\s]*([\\w\\d.; ]*)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToArray();
            bindConfigurationModel.IxfrFromDifferences = CaptureGroup(capturedText, "ixfr-from-differences[\\s]*([\\w\\d]*);");
            bindConfigurationModel.ListenOnV6 = CaptureGroup(capturedText, "listen-on-v6[\\s]*{[\\s]*([\\w\\d.; ]*)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToArray();
            bindConfigurationModel.ListenOnPort53 = CaptureGroup(capturedText, "listen-on port 53 [\\s]*{[\\s]*([\\w\\d.; ]*)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToArray();
            bindConfigurationModel.DnssecEnabled = CaptureGroup(capturedText, "dnssec-enable[\\s]*([\\w\\d]*);");
            bindConfigurationModel.DnssecValidation = CaptureGroup(capturedText, "dnssec-validation[\\s]*([\\w\\d]*);");
            bindConfigurationModel.DnssecLookaside = CaptureGroup(capturedText, "dnssec-lookaside[\\s]*([\\w\\d]*);");
            bindConfigurationModel.AuthNxdomain = CaptureGroup(capturedText, "auth-nxdomain[\\s]*([\\w\\d]*);");
            return bindConfigurationModel;
        }

        //(controls[\s]*{[\s]*[\w\s\d.]+allow[\s]*{[\s\w\d;]+}[\s]*keys {[\s]*["\w;]+[\s]*};)
        public static BindModel ParseControl(BindModel bindConfigurationModel, string text) {
            var regex = new Regex("(controls[\\s]*{[\\s]*[\\w\\s\\d.]+allow[\\s]*{[\\s\\w\\d;]+}[\\s]*keys {[\\s]*[\"\\w;]+[\\s]*};)");
            var matchedGroups = regex.Match(text).Groups;
            var capturedText = matchedGroups[1].Value;
            bindConfigurationModel.ControlIp = CaptureGroup(capturedText, "inet[\\s]+([\\d.]+)");
            bindConfigurationModel.ControlPort = CaptureGroup(capturedText, "port[\\s]+([\\d.]+)");
            bindConfigurationModel.ControlAllow = CaptureGroup(capturedText, "allow {[\\s]+([\\w\\d\\s;]+)}").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToArray();
            bindConfigurationModel.ControlKeys = CaptureGroup(capturedText, "keys {[\\s]+([\\w\\d\\s;\"]+)}").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim().Replace("\"", "")).Where(_ => !string.IsNullOrWhiteSpace(_)).ToArray();
            return bindConfigurationModel;
        }

        //(key "[\w]+" {[\s\d\w\-;"=]+[\s]+};)
        public static BindModel ParseKeySecret(BindModel bindConfigurationModel, string text) {
            var regex = new Regex("(key \"[\\w]+\" {[\\s\\d\\w\\-;\"=]+[\\s]+};)");
            var matchedGroups = regex.Match(text).Groups;
            var capturedText = matchedGroups[1].Value;
            bindConfigurationModel.KeyName = CaptureGroup(capturedText, "key \"([\\w]+)\"");
            bindConfigurationModel.KeySecret = CaptureGroup(capturedText, "secret \"([\\s\\S]*)\";");
            return bindConfigurationModel;
        }

        //(channel syslog {[\s]+[\s\d\w\-;]+)
        public static BindModel ParseLogging(BindModel bindConfigurationModel, string text) {
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
        public static BindModel ParseTrustedKey(BindModel bindConfigurationModel, string text) {
            var regex = new Regex("(trusted-keys {[\\s]+[\\s\\d\\w\\-;.\"\\/+]+};)");
            var matchedGroups = regex.Match(text).Groups;
            var capturedText = matchedGroups[1].Value;
            bindConfigurationModel.TrustedKeys = capturedText;
            return bindConfigurationModel;
        }

        //(acl [\w\d]+ {[\s]*[\w\d\/.;]+[\s]*};)
        public static List<BindAclModel> ParseAcl(string text) {
            var list = new List<BindAclModel>();
            var regex = new Regex("(acl [\\w\\d]+ {[\\s]*[\\w\\d\\/.;]+[\\s]*};)");
            var matches = regex.Matches(text);
            foreach (var match in matches) {
                var aclName = CaptureGroup(match.ToString(), "acl ([\\w\\d]+) {");
                var aclInterfaces = CaptureGroup(match.ToString(), "acl [\\w\\d]+ {([\\s]*[\\w\\d\\/.;]+)[\\s]*};").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToArray();
                var acl = new BindAclModel {
                    Name = aclName,
                    InterfaceList = aclInterfaces
                };
                list.Add(acl);
            }
            return list;
        }

        //(zone[\s]*"[\w\d\-.]+"[\s]*{[\s]*[\w ;"\/.]*};)
        public static List<BindZoneModel> ParseSimpleZones(string text) {
            var list = new List<BindZoneModel>();
            var regex = new Regex("(zone[\\s]*\"[\\w\\d\\-.]+\"[\\s]*{[\\s]*[\\w ;\"\\/.]*};)");
            var matches = regex.Matches(text);
            foreach (var match in matches) {
                var name = CaptureGroup(match.ToString(), "zone[\\s]+\"([\\s\\d\\w\\-.]+)\"");
                var type = CaptureGroup(match.ToString(), "type ([\\w]+)");
                var filePath = CaptureGroup(match.ToString(), "file \"([\\w\\d\\/.]+)\"");
                var zone = new BindZoneModel {
                    Name = name,
                    Type = type,
                    File = filePath,
                };
                list.Add(zone);
            }
            return list;
        }

        //(zone "[\s\w\d.\-]+" { type[\w\s]+;[\s]+file "[\w\d\/\-.]+";\s[\s]*serial-update[\-\w ]+;[\s]+allow-update { [\w ;]+};[\s]+allow-query { [\w ;]+};[\s]+allow-transfer { [\w ;]+};[\s]+)
        public static List<BindZoneModel> ParseComplexZones(string text) {
            var list = new List<BindZoneModel>();
            var regex = new Regex("(zone \"[\\s\\w\\d.\\-]+\" { type[\\w\\s]+;[\\s]+file \"[\\w\\d\\/\\-.]+\";\\s[\\s]*serial-update[\\-\\w ]+;[\\s]+allow-update { [\\w ;]+};[\\s]+allow-query { [\\w ;]+};[\\s]+allow-transfer { [\\w ;]+};[\\s]+)");
            var matches = regex.Matches(text);
            foreach (var match in matches) {
                var name = CaptureGroup(match.ToString(), "zone[\\s]+\"([\\s\\d\\w\\-.]+)\"");
                var type = CaptureGroup(match.ToString(), "type ([\\w]+)");
                var filePath = CaptureGroup(match.ToString(), "file \"([\\w\\d\\/.]+)\"");
                var serialUpdateMethod = CaptureGroup(match.ToString(), "serial-update-method[\\s]+([\\w]+)");
                var allowUpdate = CaptureGroup(match.ToString(), "allow-update[\\s]+{[\\s]+([\\w\\s]+);").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToArray();
                var allowQuery = CaptureGroup(match.ToString(), "allow-query[\\s]+{[\\s]+([\\w\\s]+);").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToArray();
                var allowTransfer = CaptureGroup(match.ToString(), "allow-transfer[\\s]+{[\\s]+([\\w\\s]+);").Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).Select(_ => _.Trim()).Where(_ => !string.IsNullOrWhiteSpace(_)).ToArray();
                var zone = new BindZoneModel {
                    Name = name,
                    Type = type,
                    File = filePath,
                    //SerialUpdateMethod = serialUpdateMethod,
                    //AllowUpdate = allowUpdate,
                    //AllowQuery = allowQuery,
                    //AllowTransfer = allowTransfer
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
            foreach (var match in matches) {
                var file = CaptureGroup(match.ToString(), "\"([\\d\\w\\/\\-.]+)\"");
                list.Add(file);
            }
            return list;
        }
    }
}
