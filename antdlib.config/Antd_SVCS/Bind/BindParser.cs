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

namespace antdlib.config.Antd_SVCS.Bind {
    public class BindParser {

        public static IEnumerable<BindModel2.Acl> ParseAcl(List<string> lines) {
            var list = new List<BindModel2.Acl>();
            var regex = new Regex("acl ([a-zA-Z]+)[{ ]*([a-zA-Z0-9.;\\/]+)[ };]*");
            foreach(var line in lines) {
                var matchedGroups = regex.Match(line).Groups;
                var name = matchedGroups[1].Value;
                var interfaceList = matchedGroups[2].Value;
                var acl = new BindModel2.Acl {
                    Name = name,
                    InterfaceList = interfaceList.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList()
                };
                list.Add(acl);
            }
            return list;
        }

        public static IEnumerable<BindModel2.Control> ParseControl(List<string> lines) {
            var list = new List<BindModel2.Control>();
            var regex = new Regex("controls[ {]*([\\w]+) ([\\w\\d.]+) [\\w]+ ([\\d.]+) [\\w]+ [ {]*([\\w; ]+)[ }]*[\\w]+[ {]*\"([\\w]+)\"[}; ]*");
            foreach(var line in lines) {
                var matchedGroups = regex.Match(line).Groups;
                var networkName = matchedGroups[1].Value;
                var networkAddress = matchedGroups[2].Value;
                var port = matchedGroups[3].Value;
                var interfaceList = matchedGroups[4].Value;
                var key = matchedGroups[5].Value;
                var control = new BindModel2.Control {
                    NetworkName = networkName,
                    NetworkAddress = networkAddress,
                    Port = port,
                    InterfaceList = interfaceList.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries).ToList(),
                    Key = key
                };
                list.Add(control);
            }
            return list;
        }

        public static IEnumerable<BindModel2.Include> ParseInclude(List<string> lines) {
            var list = new List<BindModel2.Include>();
            var regex = new Regex("include \"([\\w\\d\\/.\\-_]+)\";");
            foreach(var line in lines) {
                var matchedGroups = regex.Match(line).Groups;
                var filePath = matchedGroups[1].Value;
                var control = new BindModel2.Include {
                    FilePath = filePath
                };
                list.Add(control);
            }
            return list;
        }

        //(zone[\s]*"[\w\d\-.]+"[\s]*{[\s]*[\w ;]*)
        public static IEnumerable<BindConfig.OptionModel> ParseZone(List<string> lines) {
            var list = new List<BindConfig.OptionModel>();
            var regex = new Regex("[^#]zone[\\s]*[\"]*([\\w\\d.\\-_]*)[\"]*[\\s]*{[\\s]*((?!(zone[\\s]*)|(include))[\\w\\d\\s;\"/_.}{-])*;");

            return list;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignKey(string input) {
            var options = new List<BindConfig.OptionModel>();
            var regex = new Regex("[^#]acl[\\s]*[\"]*([\\w\\d.\\-_]*)[\"]*[\\s]*{[\\s]*((?!(acl[\\s]*)|(include))[\\w\\d\\s;\"/_.}{-])*;");
            var matches = regex.Matches(input);
            for(var i = 0; i < matches.Count; i++) {
                var aclDataString = matches[i].Value.Trim();
                var aclDataStringSplit = aclDataString.Split(new[] { "{" }, StringSplitOptions.RemoveEmptyEntries);
                var aclName = aclDataStringSplit.Length > 0 ? aclDataStringSplit[0].Replace("acl", "").Replace("\"", "").Trim() : "";
                var data = aclDataStringSplit.Length > 1 ? aclDataStringSplit[1].TrimEnd(';').TrimEnd('}').Trim() : "";
                var dataList = new List<BindConfig.LineModel>();
                if(data.Length > 0) {
                    var txt = data.Replace("acl {", "");
                    var simpleDataMatches = new Regex("(?!([\\w\\d\\s-\"/._*;]*})|(;))[\\w\\d\\s-/._\"*]*[;]").Matches(txt);
                    for(var x = 0; x < simpleDataMatches.Count; x++) {
                        var split = simpleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if(split.Length > 1 && !split[0].StartsWith("//")) {
                            var k = split[0];
                            var v = split[1].Replace(";", "").Replace("\"", "");
                            var ddd = new BindConfig.LineModel {
                                Key = k,
                                Value = v,
                                Type = Helper.ServiceData.SupposeDataType(v),
                                BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                    var multipleDataMatches = new Regex("[\\w\\d\\s-\"/._*]*{[\\w\\d\\s-\"/._*;]*};").Matches(txt);
                    for(var y = 0; y < multipleDataMatches.Count; y++) {
                        var split = multipleDataMatches[y].Value.Replace("\t", " ").Trim().Split(new[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if(split.Length > 1) {
                            var k = split[0];
                            var v = split[1].Replace("};", "").Replace("\t", " ").Replace("\n", " ").Trim();
                            var splitValue = v.Split(new[] { ";" }, StringSplitOptions.None).ToArray();
                            var splitValue2 = splitValue[0].Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                            var type = splitValue2.Length > 1 ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                            var ddd = new BindConfig.LineModel {
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
                var option = new BindConfig.OptionModel {
                    Name = aclName,
                    StringDefinition = data,
                    Data = dataList
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignLogging(string input) {
            var logging = new List<BindConfig.OptionModel>();
            var loggingRegex = new Regex("logging[\\s]*{[\\s]*((?!con|key|sta|acl|opt|tru|zon|inc)[\\w\\d\\s;=\"/_.}{\\-*])*;");
            var loggingMatches = loggingRegex.Matches(input);
            var data = loggingMatches.Count > 0 ? loggingMatches[0].Value : "";
            var dataList = new List<BindConfig.LineModel>();
            if(data.Length > 0) {
                var txt = data.Replace("logging {", "");
                var simpleDataMatches = new Regex("(?!([\\w\\d\\s-\"/._*;]*})|(;))[\\w\\d\\s-/._\"*]*[;]").Matches(txt);
                for(var i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if(split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new BindConfig.LineModel {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex("[\\w\\d\\s-\"/._*]*{[\\w\\d\\s-\"/._*;]*};").Matches(txt);
                for(var i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if(split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Replace("\t", " ").Replace("\n", " ").Trim();
                        var splitValue = v.Split(new[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        var type = splitValue2.Length > 1 ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new BindConfig.LineModel {
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
            var option = new BindConfig.OptionModel {
                Name = "logging",
                Data = dataList
            };
            logging.Add(option);
            return logging;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignLwres(string input) {
            var lwres = new List<BindConfig.OptionModel>();
            var lwresRegex = new Regex("lwres[\\s]*{[\\s]*((?!con|key|logg|opt|mast|statistics-ch)[\\w\\d\\s;=\"/_.}{\\-*])*;");
            var lwresMatches = lwresRegex.Matches(input);
            var data = lwresMatches.Count > 0 ? lwresMatches[0].Value : "";
            var dataList = new List<BindConfig.LineModel>();
            if(data.Length > 0) {
                var txt = data.Replace("lwres {", "");
                var simpleDataMatches = new Regex("(?!([\\w\\d\\s-\"/._*;]*})|(;))[\\w\\d\\s-/._\"*]*[;]").Matches(txt);
                for(var i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if(split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new BindConfig.LineModel {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex("[\\w\\d\\s-\"/._*]*{[\\w\\d\\s-\"/._*;]*};").Matches(txt);
                for(var i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if(split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Replace("\t", " ").Replace("\n", " ").Trim();
                        var splitValue = v.Split(new[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        var type = splitValue2.Length > 1 ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new BindConfig.LineModel {
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
            var option = new BindConfig.OptionModel {
                Name = "lwres",
                Data = dataList
            };
            lwres.Add(option);
            return lwres;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignMasters(string input) {
            var options = new List<BindConfig.OptionModel>();
            var regex = new Regex("[^#]masters[\\s]*[\"]*([\\w\\d.\\-_]*)[\"]*[\\s]*{[\\s]*((?!(masters[\\s]*)|(include))[\\w\\d\\s;\"/_.}{-])*;");
            var matches = regex.Matches(input);
            for(var i = 0; i < matches.Count; i++) {
                var mastersDataString = matches[i].Value.Trim();
                var mastersDataStringSplit = mastersDataString.Split(new[] { "{" }, StringSplitOptions.RemoveEmptyEntries);
                var mastersName = mastersDataStringSplit.Length > 0 ? mastersDataStringSplit[0].Replace("masters", "").Replace("\"", "").Trim() : "";
                var data = mastersDataStringSplit.Length > 1 ? mastersDataStringSplit[1].TrimEnd(';').TrimEnd('}').Trim() : "";
                var dataList = new List<BindConfig.LineModel>();
                if(data.Length > 0) {
                    var txt = data.Replace("masters {", "");
                    var simpleDataMatches = new Regex("(?!([\\w\\d\\s-\"/._*;]*})|(;))[\\w\\d\\s-/._\"*]*[;]").Matches(txt);
                    for(var x = 0; x < simpleDataMatches.Count; x++) {
                        var split = simpleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if(split.Length > 1 && !split[0].StartsWith("//")) {
                            var k = split[0];
                            var v = split[1].Replace(";", "").Replace("\"", "");
                            var ddd = new BindConfig.LineModel {
                                Key = k,
                                Value = v,
                                Type = Helper.ServiceData.SupposeDataType(v),
                                BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                    var multipleDataMatches = new Regex("[\\w\\d\\s-\"/._*]*{[\\w\\d\\s-\"/._*;]*};").Matches(txt);
                    for(var y = 0; y < multipleDataMatches.Count; y++) {
                        var split = multipleDataMatches[y].Value.Replace("\t", " ").Trim().Split(new[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if(split.Length > 1) {
                            var k = split[0];
                            var v = split[1].Replace("};", "").Replace("\t", " ").Replace("\n", " ").Trim();
                            var splitValue = v.Split(new[] { ";" }, StringSplitOptions.None).ToArray();
                            var splitValue2 = splitValue[0].Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                            var type = splitValue2.Length > 1 ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                            var ddd = new BindConfig.LineModel {
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
                var option = new BindConfig.OptionModel {
                    Name = mastersName,
                    StringDefinition = data,
                    Data = dataList
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignServer(string input) {
            var options = new List<BindConfig.OptionModel>();
            var regex = new Regex("[^#]server[\\s]*[\"]*([\\w\\d.\\-_]*)[\"]*[\\s]*{[\\s]*((?!(server[\\s]*)|(include))[\\w\\d\\s;\"/_.}{-])*;");
            var matches = regex.Matches(input);
            for(var i = 0; i < matches.Count; i++) {
                var serverDataString = matches[i].Value.Trim();
                var serverDataStringSplit = serverDataString.Split(new[] { "{" }, StringSplitOptions.RemoveEmptyEntries);
                var serverName = serverDataStringSplit.Length > 0 ? serverDataStringSplit[0].Replace("server", "").Replace("\"", "").Trim() : "";
                var data = serverDataStringSplit.Length > 1 ? serverDataStringSplit[1].TrimEnd(';').TrimEnd('}').Trim() : "";
                var dataList = new List<BindConfig.LineModel>();
                if(data.Length > 0) {
                    var txt = data.Replace("server {", "");
                    var simpleDataMatches = new Regex("(?!([\\w\\d\\s-\"/._*;]*})|(;))[\\w\\d\\s-/._\"*]*[;]").Matches(txt);
                    for(var x = 0; x < simpleDataMatches.Count; x++) {
                        var split = simpleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if(split.Length > 1 && !split[0].StartsWith("//")) {
                            var k = split[0];
                            var v = split[1].Replace(";", "").Replace("\"", "");
                            var ddd = new BindConfig.LineModel {
                                Key = k,
                                Value = v,
                                Type = Helper.ServiceData.SupposeDataType(v),
                                BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                    var multipleDataMatches = new Regex("[\\w\\d\\s-\"/._*]*{[\\w\\d\\s-\"/._*;]*};").Matches(txt);
                    for(var y = 0; y < multipleDataMatches.Count; y++) {
                        var split = multipleDataMatches[y].Value.Replace("\t", " ").Trim().Split(new[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if(split.Length > 1) {
                            var k = split[0];
                            var v = split[1].Replace("};", "").Replace("\t", " ").Replace("\n", " ").Trim();
                            var splitValue = v.Split(new[] { ";" }, StringSplitOptions.None).ToArray();
                            var splitValue2 = splitValue[0].Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                            var type = splitValue2.Length > 1 ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                            var ddd = new BindConfig.LineModel {
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
                var option = new BindConfig.OptionModel {
                    Name = serverName,
                    StringDefinition = data,
                    Data = dataList
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignOptions(string input) {
            var options = new List<BindConfig.OptionModel>();
            var optionsRegex = new Regex("options[\\s]*{[\\s]*((?!con|key|lwr|logg|mast|statistics-ch)[\\w\\d\\s;=\"/_.}{\\-*])*;");
            var optionsMatches = optionsRegex.Matches(input);
            var data = optionsMatches.Count > 0 ? optionsMatches[0].Value : "";
            var dataList = new List<BindConfig.LineModel>();
            if(data.Length > 0) {
                var txt = data.Replace("options {", "");
                var simpleDataMatches = new Regex("(?!([\\w\\d\\s-\"/._*;]*})|(;))[\\w\\d\\s-/._\"*]*[;]").Matches(txt);
                for(var i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if(split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new BindConfig.LineModel {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex("[\\w\\d\\s-\"/._*]*{[\\w\\d\\s-\"/._*;]*};").Matches(txt);
                for(var i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if(split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Replace("\t", " ").Replace("\n", " ").Trim();
                        var splitValue = v.Split(new[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        var type = splitValue2.Length > 1 ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new BindConfig.LineModel {
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
            var option = new BindConfig.OptionModel {
                Name = "options",
                Data = dataList
            };
            options.Add(option);
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignStatisticsChannels(string input) {
            var statisticsChannels = new List<BindConfig.OptionModel>();
            var statisticsChannelsRegex = new Regex("[^#]statistics-channels[\\s]*{[\\s]*((?!con|key|sta|acl|opt|tru|zon|logg|inc)[\\w\\d\\s;=\"/_.}{-])*;");
            var statisticsChannelsMatches = statisticsChannelsRegex.Matches(input);
            var data = statisticsChannelsMatches.Count > 0 ? statisticsChannelsMatches[0].Value : "";
            var dataList = new List<BindConfig.LineModel>();
            if(data.Length > 0) {
                var txt = data.Replace("statistics-channels {", "");
                var simpleDataMatches = new Regex("(?!([\\w\\d\\s-\"/._*;]*})|(;))[\\w\\d\\s-/._\"*]*[;]").Matches(txt);
                for(var i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if(split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new BindConfig.LineModel {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex("[\\w\\d\\s-\"/._*]*{[\\w\\d\\s-\"/._*;]*};").Matches(txt);
                for(var i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if(split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Replace("\t", " ").Replace("\n", " ").Trim();
                        var splitValue = v.Split(new[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        var type = splitValue2.Length > 1 ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new BindConfig.LineModel {
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
            var option = new BindConfig.OptionModel {
                Name = "statisticsChannels",
                Data = dataList
            };
            statisticsChannels.Add(option);
            return statisticsChannels;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignTrustedKeys(string input) {
            var trustedKeys = new List<BindConfig.OptionModel>();
            var trustedKeysRegex = new Regex("trusted-keys[\\s]*{[\\s]*((?!con|key|logg|opt|mast|lwr|statistics-ch)[\\w\\d\\s;=\"/_.}{\\-*])*;");
            var trustedKeysMatches = trustedKeysRegex.Matches(input);
            var data = trustedKeysMatches.Count > 0 ? trustedKeysMatches[0].Value : "";
            var dataList = new List<BindConfig.LineModel>();
            if(data.Length > 0) {
                var txt = data.Replace("trusted-keys {", "");
                var simpleDataMatches = new Regex("(?!([\\w\\d\\s-\"/._*;]*})|(;))[\\w\\d\\s-/._\"*]*[;]").Matches(txt);
                for(var i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if(split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new BindConfig.LineModel {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex("[\\w\\d\\s-\"/._*]*{[\\w\\d\\s-\"/._*;]*};").Matches(txt);
                for(var i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if(split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Replace("\t", " ").Replace("\n", " ").Trim();
                        var splitValue = v.Split(new[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        var type = splitValue2.Length > 1 ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new BindConfig.LineModel {
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
            var option = new BindConfig.OptionModel {
                Name = "trustedKeys",
                Data = dataList
            };
            trustedKeys.Add(option);
            return trustedKeys;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignManagedKeys(string input) {
            var managedKeys = new List<BindConfig.OptionModel>();
            var managedKeysRegex = new Regex("managed-keys[\\s]*{[\\s]*((?!con|key|logg|opt|mast|lwr|statistics-ch)[\\w\\d\\s;=\"/_.}{\\-*])*;");
            var managedKeysMatches = managedKeysRegex.Matches(input);
            var data = managedKeysMatches.Count > 0 ? managedKeysMatches[0].Value : "";
            var dataList = new List<BindConfig.LineModel>();
            if(data.Length > 0) {
                var txt = data.Replace("managed-keys {", "");
                var simpleDataMatches = new Regex("(?!([\\w\\d\\s-\"/._*;]*})|(;))[\\w\\d\\s-/._\"*]*[;]").Matches(txt);
                for(var i = 0; i < simpleDataMatches.Count; i++) {
                    var split = simpleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if(split.Length > 1 && !split[0].StartsWith("//")) {
                        var k = split[0];
                        var v = split[1].Replace(";", "").Replace("\"", "");
                        var ddd = new BindConfig.LineModel {
                            Key = k,
                            Value = v,
                            Type = Helper.ServiceData.SupposeDataType(v),
                            BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                            FilePath = ""
                        };
                        dataList.Add(ddd);
                    }
                }
                var multipleDataMatches = new Regex("[\\w\\d\\s-\"/._*]*{[\\w\\d\\s-\"/._*;]*};").Matches(txt);
                for(var i = 0; i < multipleDataMatches.Count; i++) {
                    var split = multipleDataMatches[i].Value.Replace("\t", " ").Trim().Split(new[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                    if(split.Length > 1) {
                        var k = split[0];
                        var v = split[1].Replace("};", "").Replace("\t", " ").Replace("\n", " ").Trim();
                        var splitValue = v.Split(new[] { ";" }, StringSplitOptions.None).ToArray();
                        var splitValue2 = splitValue[0].Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        var type = splitValue2.Length > 1 ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                        var ddd = new BindConfig.LineModel {
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
            var option = new BindConfig.OptionModel {
                Name = "managedKeys",
                Data = dataList
            };
            managedKeys.Add(option);
            return managedKeys;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignView(string input) {
            var options = new List<BindConfig.OptionModel>();
            var regex = new Regex("[^#]view[\\s]*[\"]*([\\w\\d.\\-_]*)[\"]*[\\s]*{[\\s]*((?!(view[\\s]*)|(include))[\\w\\d\\s;\"/_.}{-])*;");
            var matches = regex.Matches(input);
            for(var i = 0; i < matches.Count; i++) {
                var viewDataString = matches[i].Value.Trim();
                var viewDataStringSplit = viewDataString.Split(new[] { "{" }, StringSplitOptions.RemoveEmptyEntries);
                var viewName = viewDataStringSplit.Length > 0 ? viewDataStringSplit[0].Replace("view", "").Replace("\"", "").Trim() : "";
                var data = viewDataStringSplit.Length > 1 ? viewDataStringSplit[1].TrimEnd(';').TrimEnd('}').Trim() : "";
                var dataList = new List<BindConfig.LineModel>();
                if(data.Length > 0) {
                    var txt = data.Replace("view {", "");
                    var simpleDataMatches = new Regex("(?!([\\w\\d\\s-\"/._*;]*})|(;))[\\w\\d\\s-/._\"*]*[;]").Matches(txt);
                    for(var x = 0; x < simpleDataMatches.Count; x++) {
                        var split = simpleDataMatches[x].Value.Replace("\t", " ").Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if(split.Length > 1 && !split[0].StartsWith("//")) {
                            var k = split[0];
                            var v = split[1].Replace(";", "").Replace("\"", "");
                            var ddd = new BindConfig.LineModel {
                                Key = k,
                                Value = v,
                                Type = Helper.ServiceData.SupposeDataType(v),
                                BooleanVerbs = Helper.ServiceData.SupposeBooleanVerbs(v),
                                FilePath = ""
                            };
                            dataList.Add(ddd);
                        }
                    }
                    var multipleDataMatches = new Regex("[\\w\\d\\s-\"/._*]*{[\\w\\d\\s-\"/._*;]*};").Matches(txt);
                    for(var y = 0; y < multipleDataMatches.Count; y++) {
                        var split = multipleDataMatches[y].Value.Replace("\t", " ").Trim().Split(new[] { " {" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                        if(split.Length > 1) {
                            var k = split[0];
                            var v = split[1].Replace("};", "").Replace("\t", " ").Replace("\n", " ").Trim();
                            var splitValue = v.Split(new[] { ";" }, StringSplitOptions.None).ToArray();
                            var splitValue2 = splitValue[0].Trim().Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                            var type = splitValue2.Length > 1 ? ServiceDataType.DataArray : ServiceDataType.StringArray;
                            var ddd = new BindConfig.LineModel {
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
                var option = new BindConfig.OptionModel {
                    Name = viewName,
                    StringDefinition = data,
                    Data = dataList
                };
                options.Add(option);
            }
            return options;
        }
    }
}
