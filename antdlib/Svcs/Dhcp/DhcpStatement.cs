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

using antdlib.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace antdlib.Svcs.Dhcp {
    public class DhcpStatement {
        public static IEnumerable<DhcpConfig.LineModel> AssignGlobal(string input) {
            var global = new List<DhcpConfig.LineModel>() { };
            var globalStr = $"(?!range|prefix|zone|host|algor|secret)(^[^#{{}}\\s].*?;)";
            var globalRegex = new Regex(globalStr, RegexOptions.Multiline);
            var globalMatches = globalRegex.Matches(input);
            for (int i = 0; i < globalMatches.Count; i++) {
                var split = globalMatches[i].Value.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                if (split.Length > 1) {
                    var k = split[0].Trim();
                    var v = split[1].Trim().Replace(";", "").Trim();
                    var ddd = new DhcpConfig.LineModel() {
                        Key = k,
                        Value = v,
                        Type = Helper.ServiceData.SupposeDataType(v),
                        BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                        FilePath = ""
                    };
                    global.Add(ddd);
                }
            }
            return global;
        }

        public static IEnumerable<DhcpConfig.LineModel> AssignInclude(string input) {
            var include = new List<DhcpConfig.LineModel>() { };
            var includeStr = $"include[\\s\\w\\d\\-{{.:\"=/]*;";
            var includeRegex = new Regex(includeStr);
            var includeMatches = includeRegex.Matches(input);
            for (int i = 0; i < includeMatches.Count; i++) {
                var v = includeMatches[i].Value.Replace("\t", " ").Replace("include ", "").Trim();
                var ddd = new DhcpConfig.LineModel() {
                    Key = "include",
                    Value = v,
                    Type = Helper.ServiceData.SupposeDataType(v),
                    BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                    FilePath = ""
                };
                include.Add(ddd);
            }
            return include;
        }

        public static IEnumerable<DhcpConfig.LineModel> AssignPrefix6(string input) {
            var prefix6 = new List<DhcpConfig.LineModel>() { };
            var prefix6Str = $"prefix6[\\s\\w\\d\\-{{.:\"=/]*;";
            var prefix6Regex = new Regex(prefix6Str);
            var prefix6Matches = prefix6Regex.Matches(input);
            for (int i = 0; i < prefix6Matches.Count; i++) {
                var v = prefix6Matches[i].Value.Replace("\t", " ").Replace("prefix6 ", "").Trim();
                var ddd = new DhcpConfig.LineModel() {
                    Key = "prefix6",
                    Value = v,
                    Type = Helper.ServiceData.SupposeDataType(v),
                    BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                    FilePath = ""
                };
                prefix6.Add(ddd);
            }
            return prefix6;
        }

        public static IEnumerable<DhcpConfig.LineModel> AssignRange6(string input) {
            var range6 = new List<DhcpConfig.LineModel>() { };
            var range6Str = $"range6[\\s\\w\\d\\-{{.:\"=/]*;";
            var range6Regex = new Regex(range6Str);
            var range6Matches = range6Regex.Matches(input);
            for (int i = 0; i < range6Matches.Count; i++) {
                var v = range6Matches[i].Value.Replace("\t", " ").Replace("range6 ", "").Trim();
                var ddd = new DhcpConfig.LineModel() {
                    Key = "range6",
                    Value = v,
                    Type = Helper.ServiceData.SupposeDataType(v),
                    BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                    FilePath = ""
                };
                range6.Add(ddd);
            }
            return range6;
        }

        public static IEnumerable<DhcpConfig.LineModel> AssignRange(string input) {
            var range = new List<DhcpConfig.LineModel>() { };
            var rangeStr = $"range[\\s\\w\\d\\-{{.:\"=/]*;";
            var rangeRegex = new Regex(rangeStr);
            var rangeMatches = rangeRegex.Matches(input);
            for (int i = 0; i < rangeMatches.Count; i++) {
                var v = rangeMatches[i].Value.Replace("\t", " ").Replace("range ", "").Trim();
                var ddd = new DhcpConfig.LineModel() {
                    Key = "range",
                    Value = v,
                    Type = Helper.ServiceData.SupposeDataType(v),
                    BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                    FilePath = ""
                };
                range.Add(ddd);
            }
            return range;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignKey(string input) {
            var key = new List<DhcpConfig.OptionModel>() { };
            var keyStr = $"key[\\s\\S]*}};";
            var keyRegex = new Regex(keyStr);
            var keyMatches = keyRegex.Matches(input);
            var data = (keyMatches.Count > 0) ? keyMatches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("key {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[\\w\\d\\s-\"/._*]*{{[\\w\\d\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var splitValue = v.Split(new String[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Split(new String[] { " " }, StringSplitOptions.None).ToArray();
                        var type = (splitValue2.Length > 0) ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = type,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "key",
                Data = dataList
            };
            key.Add(option);
            return key;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignSubnet(string input) {
            var subnet = new List<DhcpConfig.OptionModel>() { };
            var subnetStr = $"subnet[\\s\\w\\d.]*mask[\\s\\w\\d.]*{{[\\s\\w\\d.;{{-]*";
            var subnetRegex = new Regex(subnetStr);
            var subnetMatches = subnetRegex.Matches(input);
            var data = (subnetMatches.Count > 0) ? subnetMatches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("subnet {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[\\w\\d\\s-\"/._*]*{{[\\w\\d\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var splitValue = v.Split(new String[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Split(new String[] { " " }, StringSplitOptions.None).ToArray();
                        var type = (splitValue2.Length > 0) ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = type,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "subnet",
                Data = dataList
            };
            subnet.Add(option);
            return subnet;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignHost(string input) {
            var host = new List<DhcpConfig.OptionModel>() { };
            var hostStr = $"[^\\S]host[\\s\\w\\d{{.:;\\-]*";
            var hostRegex = new Regex(hostStr);
            var hostMatches = hostRegex.Matches(input);
            var data = (hostMatches.Count > 0) ? hostMatches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("host {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[\\w\\d\\s-\"/._*]*{{[\\w\\d\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var splitValue = v.Split(new String[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Split(new String[] { " " }, StringSplitOptions.None).ToArray();
                        var type = (splitValue2.Length > 0) ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = type,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "host",
                Data = dataList
            };
            host.Add(option);
            return host;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignSubnet6(string input) {
            var subnet6 = new List<DhcpConfig.OptionModel>() { };
            var subnet6Str = $"[^\\S][^\\S]subnet6[\\s\\w\\d\\-{{.:;\"=/]*";
            var subnet6Regex = new Regex(subnet6Str);
            var subnet6Matches = subnet6Regex.Matches(input);
            var data = (subnet6Matches.Count > 0) ? subnet6Matches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("subnet6 {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[\\w\\d\\s-\"/._*]*{{[\\w\\d\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var splitValue = v.Split(new String[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Split(new String[] { " " }, StringSplitOptions.None).ToArray();
                        var type = (splitValue2.Length > 0) ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = type,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "subnet6",
                Data = dataList
            };
            subnet6.Add(option);
            return subnet6;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignFailover(string input) {
            var failover = new List<DhcpConfig.OptionModel>() { };
            var failoverStr = $"[^\\S]failover[\\s\\w\\d\\-{{.:;\"]*";
            var failoverRegex = new Regex(failoverStr);
            var failoverMatches = failoverRegex.Matches(input);
            var data = (failoverMatches.Count > 0) ? failoverMatches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("failover {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[\\w\\d\\s-\"/._*]*{{[\\w\\d\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var splitValue = v.Split(new String[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Split(new String[] { " " }, StringSplitOptions.None).ToArray();
                        var type = (splitValue2.Length > 0) ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = type,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "failover",
                Data = dataList
            };
            failover.Add(option);
            return failover;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignLogging(string input) {
            var logging = new List<DhcpConfig.OptionModel>() { };
            var loggingStr = $"logging[\\s\\w\\d\\-{{.:\"=/;}}]*";
            var loggingRegex = new Regex(loggingStr);
            var loggingMatches = loggingRegex.Matches(input);
            var data = (loggingMatches.Count > 0) ? loggingMatches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("logging {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[\\w\\d\\s-\"/._*]*{{[\\w\\d\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var splitValue = v.Split(new String[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Split(new String[] { " " }, StringSplitOptions.None).ToArray();
                        var type = (splitValue2.Length > 0) ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = type,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "logging",
                Data = dataList
            };
            logging.Add(option);
            return logging;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignSharedNetwork(string input) {
            var sharedNetwork = new List<DhcpConfig.OptionModel>() { };
            var sharedNetworkStr = $"[^\\S][^\\S]shared-network[\\s\\w\\d\\-{{.:;\"=/]*";
            var sharedNetworkRegex = new Regex(sharedNetworkStr);
            var sharedNetworkMatches = sharedNetworkRegex.Matches(input);
            var data = (sharedNetworkMatches.Count > 0) ? sharedNetworkMatches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("shared-network {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[\\w\\d\\s-\"/._*]*{{[\\w\\d\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var splitValue = v.Split(new String[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Split(new String[] { " " }, StringSplitOptions.None).ToArray();
                        var type = (splitValue2.Length > 0) ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = type,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "shared-network",
                Data = dataList
            };
            sharedNetwork.Add(option);
            return sharedNetwork;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignGroup(string input) {
            var group = new List<DhcpConfig.OptionModel>() { };
            var groupStr = $"[^\\S][^\\S]group[\\s\\w\\d\\-{{.:;\"=/]*";
            var groupRegex = new Regex(groupStr);
            var groupMatches = groupRegex.Matches(input);
            var data = (groupMatches.Count > 0) ? groupMatches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("group {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[\\w\\d\\s-\"/._*]*{{[\\w\\d\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var splitValue = v.Split(new String[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Split(new String[] { " " }, StringSplitOptions.None).ToArray();
                        var type = (splitValue2.Length > 0) ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = type,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "group",
                Data = dataList
            };
            group.Add(option);
            return group;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignClass(string input) {
            var @class = new List<DhcpConfig.OptionModel>() { };
            var classStr = $"[^\\S]class[\\s\\w\\d\\-{{.:;\"=]*";
            var classRegex = new Regex(classStr);
            var classMatches = classRegex.Matches(input);
            var data = (classMatches.Count > 0) ? classMatches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("class {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[\\w\\d\\s-\"/._*]*{{[\\w\\d\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var splitValue = v.Split(new String[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Split(new String[] { " " }, StringSplitOptions.None).ToArray();
                        var type = (splitValue2.Length > 0) ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = type,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "class",
                Data = dataList
            };
            @class.Add(option);
            return @class;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignSubclass(string input) {
            var subclass = new List<DhcpConfig.OptionModel>() { };
            var subclassStr = $"[^\\S]subclass[\\s\\w\\d\\-{{.:;\"=/]*";
            var subclassRegex = new Regex(subclassStr);
            var subclassMatches = subclassRegex.Matches(input);
            var data = (subclassMatches.Count > 0) ? subclassMatches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("subclass {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[\\w\\d\\s-\"/._*]*{{[\\w\\d\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var splitValue = v.Split(new String[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Split(new String[] { " " }, StringSplitOptions.None).ToArray();
                        var type = (splitValue2.Length > 0) ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = type,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "subclass",
                Data = dataList
            };
            subclass.Add(option);
            return subclass;
        }
    }
}
