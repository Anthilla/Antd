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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace antdlib.Svcs.Dhcp {
    public class DhcpStatement {
        public static ServiceDataType SupposeDataType(string value) {
            if (value == "true" || value == "True" ||
                value == "false" || value == "False" ||
                value == "yes" || value == "Yes" ||
                value == "no" || value == "No") {
                return ServiceDataType.Boolean;
            }
            else if (value.Contains(";")) {
                return ServiceDataType.StringArray;
            }
            else {
                return ServiceDataType.String;
            }
        }

        public static KeyValuePair<string, string> SupposeBooleanVerbs(string value) {
            if (value == "true" || value == "false") {
                return new KeyValuePair<string, string>("true", "false");
            }
            else if (value == "True" || value == "False") {
                return new KeyValuePair<string, string>("True", "False");
            }
            else if (value == "yes" || value == "no") {
                return new KeyValuePair<string, string>("yes", "no");
            }
            else if (value == "Yes" || value == "No") {
                return new KeyValuePair<string, string>("Yes", "No");
            }
            else {
                return new KeyValuePair<string, string>("", "");
            }
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignAcl(string input) {
            var options = new List<DhcpConfig.OptionModel>() { };
            var str = $"[^#]acl[\\s]*[\"]*([a-zA-Z0-9.\\-_]*)[\"]*[\\s]*{{[\\s]*((?!(acl[\\s]*)|(include))[a-zA-Z0-9\\s;\"/_.}}{{-])*;";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var aclDataString = matches[i].Value.Trim();
                var aclDataStringSplit = aclDataString.Split(new String[] { "{" }, StringSplitOptions.RemoveEmptyEntries);
                var aclName = (aclDataStringSplit.Length > 0) ? aclDataStringSplit[0].Replace("acl", "").Replace("\"", "").Trim() : "";
                var data = (aclDataStringSplit.Length > 1) ? aclDataStringSplit[1].TrimEnd(';').TrimEnd('}').Trim() : "";
                var dataList = new List<DhcpConfig.LineModel>() { };
                if (data.Length > 0) {
                    var txt = data.Replace("acl {", "");
                    var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                    for (int x = 0; x < simpleDataMatches.Count; x++) {
                        var split = simpleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if (split.Length > 1 && !split[0].StartsWith("//")) {
                            var k = split[0];
                            var v = split[1].Replace(";", "").Replace("\"", "");
                            var ddd = new DhcpConfig.LineModel() {
                                Key = k,
                                Value = v,
                                Type = SupposeDataType(v),
                                BooleanVerbs = SupposeBooleanVerbs(v),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                    var multipleDataMatches = new Regex($"[a-zA-Z0-9\\s-\"/._*]*{{[a-zA-Z0-9\\s-\"/._*;]*}};").Matches(txt);
                    for (int x = 0; x < multipleDataMatches.Count; x++) {
                        var split = multipleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if (split.Length > 1) {
                            var k = split[0];
                            var v = split[1].Replace("};", "").Trim();
                            var ddd = new DhcpConfig.LineModel() {
                                Key = k,
                                Value = v,
                                Type = ServiceDataType.StringArray,
                                BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                }
                var option = new DhcpConfig.OptionModel() {
                    Name = aclName,
                    StringDefinition = data,
                    Data = dataList
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignControls(string input) {
            var controls = new List<DhcpConfig.OptionModel>() { };
            var controlsStr = $"[^#]*controls[\\s]*{{[\\s]*((?!con|key|sta|acl|opt|tru|zon|inc)[a-zA-Z0-9\\s;=\"/_.}}{{\\-*])*;";
            var controlsRegex = new Regex(controlsStr);
            var controlsMatches = controlsRegex.Matches(input);
            var data = (controlsMatches.Count > 0) ? controlsMatches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("controls {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = SupposeDataType(v),
                            BooleanVerbs = SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[a-zA-Z0-9\\s-\"/._*]*{{[a-zA-Z0-9\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = ServiceDataType.StringArray,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "controls",
                Data = dataList
            };
            controls.Add(option);
            return controls;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignInclude(string input) {
            var options = new List<DhcpConfig.OptionModel>() { };
            var str = $"[^#]*include[\\s]*\"([a-zA-Z0-9\\s./] *)\";";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var includeStr = matches[i].Value.Replace("{", "").Replace("\"", "").Replace("include", "").Trim();
                var includeName = "include";
                var option = new DhcpConfig.OptionModel() {
                    Name = includeName,
                    StringDefinition = includeStr,
                    //Data = data.ToList()
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignKey(string input) {
            var options = new List<DhcpConfig.OptionModel>() { };
            var str = $"[^#]acl[\\s]*[\"]*([a-zA-Z0-9.\\-_]*)[\"]*[\\s]*{{[\\s]*((?!(acl[\\s]*)|(include))[a-zA-Z0-9\\s;\"/_.}}{{-])*;";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var aclDataString = matches[i].Value.Trim();
                var aclDataStringSplit = aclDataString.Split(new String[] { "{" }, StringSplitOptions.RemoveEmptyEntries);
                var aclName = (aclDataStringSplit.Length > 0) ? aclDataStringSplit[0].Replace("acl", "").Replace("\"", "").Trim() : "";
                var data = (aclDataStringSplit.Length > 1) ? aclDataStringSplit[1].TrimEnd(';').TrimEnd('}').Trim() : "";
                var dataList = new List<DhcpConfig.LineModel>() { };
                if (data.Length > 0) {
                    var txt = data.Replace("acl {", "");
                    var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                    for (int x = 0; x < simpleDataMatches.Count; x++) {
                        var split = simpleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if (split.Length > 1 && !split[0].StartsWith("//")) {
                            var k = split[0];
                            var v = split[1].Replace(";", "").Replace("\"", "");
                            var ddd = new DhcpConfig.LineModel() {
                                Key = k,
                                Value = v,
                                Type = SupposeDataType(v),
                                BooleanVerbs = SupposeBooleanVerbs(v),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                    var multipleDataMatches = new Regex($"[a-zA-Z0-9\\s-\"/._*]*{{[a-zA-Z0-9\\s-\"/._*;]*}};").Matches(txt);
                    for (int x = 0; x < multipleDataMatches.Count; x++) {
                        var split = multipleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if (split.Length > 1) {
                            var k = split[0];
                            var v = split[1].Replace("};", "").Trim();
                            var ddd = new DhcpConfig.LineModel() {
                                Key = k,
                                Value = v,
                                Type = ServiceDataType.StringArray,
                                BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                }
                var option = new DhcpConfig.OptionModel() {
                    Name = aclName,
                    StringDefinition = data,
                    Data = dataList
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignLogging(string input) {
            var logging = new List<DhcpConfig.OptionModel>() { };
            //[^#]*logging[\s]*{[\s]*((?!con|key|sta|acl|opt|tru|zon|inc)[a-zA-Z0-9\s;="/_.}{-])*;
            var loggingStr = $"logging[\\s]*{{[\\s]*((?!con|key|sta|acl|opt|tru|zon|inc)[a-zA-Z0-9\\s;=\"/_.}}{{\\-*])*;";
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
                            Type = SupposeDataType(v),
                            BooleanVerbs = SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[a-zA-Z0-9\\s-\"/._*]*{{[a-zA-Z0-9\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = ServiceDataType.StringArray,
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

        public static IEnumerable<DhcpConfig.OptionModel> AssignLwres(string input) {
            var lwres = new List<DhcpConfig.OptionModel>() { };
            var lwresStr = $"lwres[\\s]*{{[\\s]*((?!con|key|logg|opt|mast|statistics-ch)[a-zA-Z0-9\\s;=\"/_.}}{{\\-*])*;";
            var lwresRegex = new Regex(lwresStr);
            var lwresMatches = lwresRegex.Matches(input);
            var data = (lwresMatches.Count > 0) ? lwresMatches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("lwres {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = SupposeDataType(v),
                            BooleanVerbs = SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[a-zA-Z0-9\\s-\"/._*]*{{[a-zA-Z0-9\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = ServiceDataType.StringArray,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "lwres",
                Data = dataList
            };
            lwres.Add(option);
            return lwres;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignMasters(string input) {
            var options = new List<DhcpConfig.OptionModel>() { };
            var str = $"[^#]masters[\\s]*[\"]*([a-zA-Z0-9.\\-_]*)[\"]*[\\s]*{{[\\s]*((?!(masters[\\s]*)|(include))[a-zA-Z0-9\\s;\"/_.}}{{-])*;";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var mastersDataString = matches[i].Value.Trim();
                var mastersDataStringSplit = mastersDataString.Split(new String[] { "{" }, StringSplitOptions.RemoveEmptyEntries);
                var mastersName = (mastersDataStringSplit.Length > 0) ? mastersDataStringSplit[0].Replace("masters", "").Replace("\"", "").Trim() : "";
                var data = (mastersDataStringSplit.Length > 1) ? mastersDataStringSplit[1].TrimEnd(';').TrimEnd('}').Trim() : "";
                var dataList = new List<DhcpConfig.LineModel>() { };
                if (data.Length > 0) {
                    var txt = data.Replace("masters {", "");
                    var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                    for (int x = 0; x < simpleDataMatches.Count; x++) {
                        var split = simpleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if (split.Length > 1 && !split[0].StartsWith("//")) {
                            var k = split[0];
                            var v = split[1].Replace(";", "").Replace("\"", "");
                            var ddd = new DhcpConfig.LineModel() {
                                Key = k,
                                Value = v,
                                Type = SupposeDataType(v),
                                BooleanVerbs = SupposeBooleanVerbs(v),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                    var multipleDataMatches = new Regex($"[a-zA-Z0-9\\s-\"/._*]*{{[a-zA-Z0-9\\s-\"/._*;]*}};").Matches(txt);
                    for (int x = 0; x < multipleDataMatches.Count; x++) {
                        var split = multipleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if (split.Length > 1) {
                            var k = split[0];
                            var v = split[1].Replace("};", "").Trim();
                            var ddd = new DhcpConfig.LineModel() {
                                Key = k,
                                Value = v,
                                Type = ServiceDataType.StringArray,
                                BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                }
                var option = new DhcpConfig.OptionModel() {
                    Name = mastersName,
                    StringDefinition = data,
                    Data = dataList
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignServer(string input) {
            var options = new List<DhcpConfig.OptionModel>() { };
            var str = $"[^#]server[\\s]*[\"]*([a-zA-Z0-9.\\-_]*)[\"]*[\\s]*{{[\\s]*((?!(server[\\s]*)|(include))[a-zA-Z0-9\\s;\"/_.}}{{-])*;";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var serverDataString = matches[i].Value.Trim();
                var serverDataStringSplit = serverDataString.Split(new String[] { "{" }, StringSplitOptions.RemoveEmptyEntries);
                var serverName = (serverDataStringSplit.Length > 0) ? serverDataStringSplit[0].Replace("server", "").Replace("\"", "").Trim() : "";
                var data = (serverDataStringSplit.Length > 1) ? serverDataStringSplit[1].TrimEnd(';').TrimEnd('}').Trim() : "";
                var dataList = new List<DhcpConfig.LineModel>() { };
                if (data.Length > 0) {
                    var txt = data.Replace("server {", "");
                    var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                    for (int x = 0; x < simpleDataMatches.Count; x++) {
                        var split = simpleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if (split.Length > 1 && !split[0].StartsWith("//")) {
                            var k = split[0];
                            var v = split[1].Replace(";", "").Replace("\"", "");
                            var ddd = new DhcpConfig.LineModel() {
                                Key = k,
                                Value = v,
                                Type = SupposeDataType(v),
                                BooleanVerbs = SupposeBooleanVerbs(v),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                    var multipleDataMatches = new Regex($"[a-zA-Z0-9\\s-\"/._*]*{{[a-zA-Z0-9\\s-\"/._*;]*}};").Matches(txt);
                    for (int x = 0; x < multipleDataMatches.Count; x++) {
                        var split = multipleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if (split.Length > 1) {
                            var k = split[0];
                            var v = split[1].Replace("};", "").Trim();
                            var ddd = new DhcpConfig.LineModel() {
                                Key = k,
                                Value = v,
                                Type = ServiceDataType.StringArray,
                                BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                }
                var option = new DhcpConfig.OptionModel() {
                    Name = serverName,
                    StringDefinition = data,
                    Data = dataList
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignOptions(string input) {
            var options = new List<DhcpConfig.OptionModel>() { };
            var optionsStr = $"options[\\s]*{{[\\s]*((?!con|key|lwr|logg|mast|statistics-ch)[a-zA-Z0-9\\s;=\"/_.}}{{\\-*])*;";
            var optionsRegex = new Regex(optionsStr);
            var optionsMatches = optionsRegex.Matches(input);
            var data = (optionsMatches.Count > 0) ? optionsMatches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("options {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = SupposeDataType(v),
                            BooleanVerbs = SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[a-zA-Z0-9\\s-\"/._*]*{{[a-zA-Z0-9\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = ServiceDataType.StringArray,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "options",
                Data = dataList
            };
            options.Add(option);
            return options;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignStatisticsChannels(string input) {
            var statisticsChannels = new List<DhcpConfig.OptionModel>() { };
            var statisticsChannelsStr = $"statistics-channels[\\s]*{{[\\s]*((?!con|key|logg|opt|mast|lwr)[a-zA-Z0-9\\s;=\"/_.}}{{\\-*])*;";
            var statisticsChannelsRegex = new Regex(statisticsChannelsStr);
            var statisticsChannelsMatches = statisticsChannelsRegex.Matches(input);
            var data = (statisticsChannelsMatches.Count > 0) ? statisticsChannelsMatches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("statistics-channels {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = SupposeDataType(v),
                            BooleanVerbs = SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[a-zA-Z0-9\\s-\"/._*]*{{[a-zA-Z0-9\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = ServiceDataType.StringArray,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "statisticsChannels",
                Data = dataList
            };
            statisticsChannels.Add(option);
            return statisticsChannels;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignTrustedKeys(string input) {
            var trustedKeys = new List<DhcpConfig.OptionModel>() { };
            var trustedKeysStr = $"trusted-keys[\\s]*{{[\\s]*((?!con|key|logg|opt|mast|lwr|statistics-ch)[a-zA-Z0-9\\s;=\"/_.}}{{\\-*])*;";
            var trustedKeysRegex = new Regex(trustedKeysStr);
            var trustedKeysMatches = trustedKeysRegex.Matches(input);
            var data = (trustedKeysMatches.Count > 0) ? trustedKeysMatches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("trusted-keys {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = SupposeDataType(v),
                            BooleanVerbs = SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[a-zA-Z0-9\\s-\"/._*]*{{[a-zA-Z0-9\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = ServiceDataType.StringArray,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "trustedKeys",
                Data = dataList
            };
            trustedKeys.Add(option);
            return trustedKeys;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignManagedKeys(string input) {
            var managedKeys = new List<DhcpConfig.OptionModel>() { };
            var managedKeysStr = $"managed-keys[\\s]*{{[\\s]*((?!con|key|logg|opt|mast|lwr|statistics-ch)[a-zA-Z0-9\\s;=\"/_.}}{{\\-*])*;";
            var managedKeysRegex = new Regex(managedKeysStr);
            var managedKeysMatches = managedKeysRegex.Matches(input);
            var data = (managedKeysMatches.Count > 0) ? managedKeysMatches[0].Value : "";
            var dataList = new List<DhcpConfig.LineModel>() { };
            if (data.Length > 0) {
                var txt = data.Replace("managed-keys {", "");
                var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                for (int i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = SupposeDataType(v),
                            BooleanVerbs = SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex($"[a-zA-Z0-9\\s-\"/._*]*{{[a-zA-Z0-9\\s-\"/._*;]*}};").Matches(txt);
                for (int i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if (split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Trim();
                        var ddd = new DhcpConfig.LineModel() {
                            Key = k,
                            Value = v,
                            Type = ServiceDataType.StringArray,
                            BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
            }
            var option = new DhcpConfig.OptionModel() {
                Name = "managedKeys",
                Data = dataList
            };
            managedKeys.Add(option);
            return managedKeys;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignView(string input) {
            var options = new List<DhcpConfig.OptionModel>() { };
            var str = $"[^#]view[\\s]*[\"]*([a-zA-Z0-9.\\-_]*)[\"]*[\\s]*{{[\\s]*((?!(view[\\s]*)|(include))[a-zA-Z0-9\\s;\"/_.}}{{-])*;";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var viewDataString = matches[i].Value.Trim();
                var viewDataStringSplit = viewDataString.Split(new String[] { "{" }, StringSplitOptions.RemoveEmptyEntries);
                var viewName = (viewDataStringSplit.Length > 0) ? viewDataStringSplit[0].Replace("view", "").Replace("\"", "").Trim() : "";
                var data = (viewDataStringSplit.Length > 1) ? viewDataStringSplit[1].TrimEnd(';').TrimEnd('}').Trim() : "";
                var dataList = new List<DhcpConfig.LineModel>() { };
                if (data.Length > 0) {
                    var txt = data.Replace("view {", "");
                    var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                    for (int x = 0; x < simpleDataMatches.Count; x++) {
                        var split = simpleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if (split.Length > 1 && !split[0].StartsWith("//")) {
                            var k = split[0];
                            var v = split[1].Replace(";", "").Replace("\"", "");
                            var ddd = new DhcpConfig.LineModel() {
                                Key = k,
                                Value = v,
                                Type = SupposeDataType(v),
                                BooleanVerbs = SupposeBooleanVerbs(v),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                    var multipleDataMatches = new Regex($"[a-zA-Z0-9\\s-\"/._*]*{{[a-zA-Z0-9\\s-\"/._*;]*}};").Matches(txt);
                    for (int x = 0; x < multipleDataMatches.Count; x++) {
                        var split = multipleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if (split.Length > 1) {
                            var k = split[0];
                            var v = split[1].Replace("};", "").Trim();
                            var ddd = new DhcpConfig.LineModel() {
                                Key = k,
                                Value = v,
                                Type = ServiceDataType.StringArray,
                                BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                }
                var option = new DhcpConfig.OptionModel() {
                    Name = viewName,
                    StringDefinition = data,
                    Data = dataList
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<DhcpConfig.OptionModel> AssignZone(string input) {
            var options = new List<DhcpConfig.OptionModel>() { };
            var str = $"[^#]zone[\\s]*[\"]*([a-zA-Z0-9.\\-_]*)[\"]*[\\s]*{{[\\s]*((?!(zone[\\s]*)|(include))[a-zA-Z0-9\\s;\"/_.}}{{-])*;";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var zoneDataString = matches[i].Value.Trim();
                var zoneDataStringSplit = zoneDataString.Split(new String[] { "{" }, StringSplitOptions.RemoveEmptyEntries);
                var zoneName = (zoneDataStringSplit.Length > 0) ? zoneDataStringSplit[0].Replace("zone", "").Replace("\"", "").Trim() : "";
                var data = (zoneDataStringSplit.Length > 1) ? zoneDataStringSplit[1].TrimEnd(';').TrimEnd('}').Trim() : "";
                var dataList = new List<DhcpConfig.LineModel>() { };
                if (data.Length > 0) {
                    var txt = data.Replace("zone {", "");
                    var simpleDataMatches = new Regex($"(?!([a-zA-Z0-9\\s-\"/._*;]*}})|(;))[a-zA-Z0-9\\s-/._\"*]*[;]").Matches(txt);
                    for (int x = 0;x < simpleDataMatches.Count; x++) {
                        var split = simpleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if (split.Length > 1 && !split[0].StartsWith("//")) {
                            var k = split[0];
                            var v = split[1].Replace(";", "").Replace("\"", "");
                            var ddd = new DhcpConfig.LineModel() {
                                Key = k,
                                Value = v,
                                Type = SupposeDataType(v),
                                BooleanVerbs = SupposeBooleanVerbs(v),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                    var multipleDataMatches = new Regex($"[a-zA-Z0-9\\s-\"/._*]*{{[a-zA-Z0-9\\s-\"/._*;]*}};").Matches(txt);
                    for (int x = 0; x < multipleDataMatches.Count; x++) {
                        var split = multipleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new String[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if (split.Length > 1) {
                            var k = split[0];
                            var v = split[1].Replace("};", "").Trim();
                            var ddd = new DhcpConfig.LineModel() {
                                Key = k,
                                Value = v,
                                Type = ServiceDataType.StringArray,
                                BooleanVerbs = new KeyValuePair<string, string>(";", ";"),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                }
                var option = new DhcpConfig.OptionModel() {
                    Name = zoneName,
                    StringDefinition = data,
                    Data = dataList
                };
                options.Add(option);
            }
            return options;
        }
    }
}
