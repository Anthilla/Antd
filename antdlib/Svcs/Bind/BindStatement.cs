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
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace antdlib.Svcs.Bind {
    public class BindStatement {

        public static IEnumerable<BindConfig.OptionModel> AssignAcl(string filePath, string input) {
            var options = new List<BindConfig.OptionModel>() { };
            var str = $"[^#]acl[\\s]*([a-zA-Z0-9.\\-_]*)[\\s]*{{";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var aclName = matches[i].Value.Replace("{", "").Replace("\"", "").Replace("acl", "").Trim();
                var aclStr = $"[^#]acl[\\s]*iif[\\s]*{{[\\s]*((?!acl[\\s]*)[a-zA-Z0-9\\s;\"/_.}}{{-])*;";
                var aclRegex = new Regex(aclStr);
                var aclMatches = aclRegex.Matches(input);
                var data = "";
                if (aclMatches.Count > 0) {
                    data = aclMatches[0].Value;
                }
                var option = new BindConfig.OptionModel() {
                    FilePath = filePath,
                    Name = aclName,
                    StringDefinition = data,
                    //Data = data.ToList()
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignControls(string filePath, string input) {
            var options = new List<BindConfig.OptionModel>() { };
            var str = $"[^#]controls[\\s]*{{";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var controlsName = "controls";
                var controlsStr = $"[^#]controls[\\s]*{{[\\s]*((?![ghjqczxvm]+)[a-zA-Z0-9\\s;\"/_.}}{{-])*;";
                var controlsRegex = new Regex(controlsStr);
                var controlsMatches = controlsRegex.Matches(input);
                var data = "";
                if (controlsMatches.Count > 0) {
                    data = controlsMatches[0].Value;
                }
                var option = new BindConfig.OptionModel() {
                    FilePath = filePath,
                    Name = controlsName,
                    StringDefinition = data,
                    //Data = data.ToList()
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignInclude(string filePath, string input) {
            var options = new List<BindConfig.OptionModel>() { };
            var str = $"[^#]include[\\s]*\"([a-zA-Z0-9\\s./] *)\";";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var includeStr = matches[i].Value.Replace("{", "").Replace("\"", "").Replace("include", "").Trim();
                var includeName = "include";
                var option = new BindConfig.OptionModel() {
                    FilePath = filePath,
                    Name = includeName,
                    StringDefinition = includeStr,
                    //Data = data.ToList()
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignKey(string filePath, string input) {
            var options = new List<BindConfig.OptionModel>() { };
            var str = $"[^#]key[\\s]*[\"]*([a-zA-Z0-9.\\-_]*)[\"]*[\\s]*{{";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var keyName = matches[i].Value.Replace("{", "").Replace("\"", "").Replace("key", "").Trim();
                var keyStr = $"[^#]key[\\s]*\"{keyName}\"[\\s]*{{[\\s]*((?!con|log|sta|acl|opt|tru|zon|inc)[a-zA-Z0-9\\s;=\"/_.}}{{-])*;";
                var keyRegex = new Regex(keyStr);
                var keyMatches = keyRegex.Matches(input);
                var data = "";
                if (keyMatches.Count > 0) {
                    data = keyMatches[0].Value;
                }
                var option = new BindConfig.OptionModel() {
                    FilePath = filePath,
                    Name = keyName,
                    StringDefinition = data,
                    //Data = data.ToList()
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignLogging(string filePath, string input) {
            var options = new List<BindConfig.OptionModel>() { };
            var str = $"[^#]logging[\\s]*{{[\\s]*((?!con|key|sta|acl|opt|tru|zon|inc)[a-zA-Z0-9\\s;=\"/_.}}{{-])*;";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var loggingName = "logging";
                var loggingStr = $"[^#]logging[\\s]*{{[\\s]*((?!con|key|sta|acl|opt|tru|zon|inc)[a-zA-Z0-9\\s;=\"/_.}}{{-])*;";
                var loggingRegex = new Regex(loggingStr);
                var loggingMatches = loggingRegex.Matches(input);
                var data = "";
                if (loggingMatches.Count > 0) {
                    data = loggingMatches[0].Value;
                }
                var option = new BindConfig.OptionModel() {
                    FilePath = filePath,
                    Name = loggingName,
                    StringDefinition = data,
                    //Data = data.ToList()
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignLwres(string filePath, string input) {
            var options = new List<BindConfig.OptionModel>() { };
            var str = $"[^#]lwres[\\s]*{{[\\s]*((?!con|key|sta|acl|opt|tru|zon|inc)[a-zA-Z0-9\\s;=\"/_.}}{{-])*;";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var lwresName = "lwres";
                var lwresStr = $"[^#]lwres[\\s]*{{[\\s]*((?!con|log|key|sta|acl|opt|tru|zon|inc)[a-zA-Z0-9\\s;=\"/_.}}{{-])*;";
                var lwresRegex = new Regex(lwresStr);
                var lwresMatches = lwresRegex.Matches(input);
                var data = "";
                if (lwresMatches.Count > 0) {
                    data = lwresMatches[0].Value;
                }
                var option = new BindConfig.OptionModel() {
                    FilePath = filePath,
                    Name = lwresName,
                    StringDefinition = data,
                    //Data = data.ToList()
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignMasters(string filePath, string input) {
            var options = new List<BindConfig.OptionModel>() { };
            var str = $"[^#]masters[\\s]*([a-zA-Z0-9]*)[\\s]*([a-zA-Z0-9.:/]*)[\\s]*{{";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var mastersName = matches[i].Value.Replace("{", "").Replace("masters", "").Trim();
                var mastersStr = $"[^#]masters[\\s]*\"{Regex.Escape(mastersName)}\"[\\s]*{{[\\s]*((?!con|logg|key|sta|acl|opt|tru|zon|inc)[a-zA-Z0-9\\s;=\"/_.}}{{-])*;";
                var mastersRegex = new Regex(mastersStr);
                var mastersMatches = mastersRegex.Matches(input);
                var data = "";
                if (mastersMatches.Count > 0) {
                    data = mastersMatches[0].Value;
                }
                var option = new BindConfig.OptionModel() {
                    FilePath = filePath,
                    Name = mastersName,
                    StringDefinition = data,
                    //Data = data.ToList()
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignServer(string filePath, string input) {
            var options = new List<BindConfig.OptionModel>() { };
            var str = $"[^#]server[\\s]*([a-zA-Z0-9.\\-\"_]*[\\s]*{{";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var serverName = matches[i].Value.Replace("{", "").Replace("\"", "").Replace("server", "").Trim();
                var serverStr = $"[^#]server[\\s]*\"{serverName}\"[\\s]*{{[\\s]*((?!con|logg|ser|sta|acl|opt|tru|zon|inc|key)[a-zA-Z0-9\\s;\"/_.}}{{-])*;";
                var serverRegex = new Regex(serverStr);
                var serverMatches = serverRegex.Matches(input);
                var data = "";
                if (serverMatches.Count > 0) {
                    data = serverMatches[0].Value;
                }
                var option = new BindConfig.OptionModel() {
                    FilePath = filePath,
                    Name = serverName,
                    StringDefinition = data,
                    //Data = data.ToList()
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignStatisticsChannels(string filePath, string input) {
            var options = new List<BindConfig.OptionModel>() { };
            var str = $"[^#]statistics-channels[\\s]*{{[\\s]*((?!con|key|logg|acl|opt|tru|zon|inc)[a-zA-Z0-9\\s;=\"/_.}}{{-])*;";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var statisticsChannelsName = "statistics-channels";
                var statisticsChannelsStr = $"[^#]statistics-channels[\\s]*{{[\\s]*((?!con|key|logg|acl|opt|tru|zon|inc)[a-zA-Z0-9\\s;=\"/_.}}{{-])*;";
                var statisticsChannelsRegex = new Regex(statisticsChannelsStr);
                var statisticsChannelsMatches = statisticsChannelsRegex.Matches(input);
                var data = "";
                if (statisticsChannelsMatches.Count > 0) {
                    data = statisticsChannelsMatches[0].Value;
                }
                var option = new BindConfig.OptionModel() {
                    FilePath = filePath,
                    Name = statisticsChannelsName,
                    StringDefinition = data,
                    //Data = data.ToList()
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignTrustedKeys(string filePath, string input) {
            var options = new List<BindConfig.OptionModel>() { };
            var str = $"[^#]trusted-keys[\\s]*{{[\\s]*((?!con|key|sta|acl|opt|logg|zon|inc)[a-zA-Z0-9\\s;=\"/_.}}{{-])*;";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var trustedKeysName = "trusted-keys";
                var trustedKeysStr = $"[^#]trusted-keys[\\s]*{{[\\s]*((?!con|key|sta|acl|opt|logg|zon|inc)[a-zA-Z0-9\\s;=\"/_.}}{{-])*;";
                var trustedKeysRegex = new Regex(trustedKeysStr);
                var trustedKeysMatches = trustedKeysRegex.Matches(input);
                var data = "";
                if (trustedKeysMatches.Count > 0) {
                    data = trustedKeysMatches[0].Value;
                }
                var option = new BindConfig.OptionModel() {
                    FilePath = filePath,
                    Name = trustedKeysName,
                    StringDefinition = data,
                    //Data = data.ToList()
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignManagedKeys(string filePath, string input) {
            var options = new List<BindConfig.OptionModel>() { };
            var str = $"[^#]managed-keys[\\s]*{{[\\s]*((?!con|key|sta|acl|opt|logg|zon|inc)[a-zA-Z0-9\\s;=\"/_.}}{{-])*;";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var managedKeysName = "managed-keys";
                var managedKeysStr = $"[^#]managed-keys[\\s]*{{[\\s]*((?!con|key|sta|acl|opt|logg|zon|inc)[a-zA-Z0-9\\s;=\"/_.}}{{-])*;";
                var managedKeysRegex = new Regex(managedKeysStr);
                var managedKeysMatches = managedKeysRegex.Matches(input);
                var data = "";
                if (managedKeysMatches.Count > 0) {
                    data = managedKeysMatches[0].Value;
                }
                var option = new BindConfig.OptionModel() {
                    FilePath = filePath,
                    Name = managedKeysName,
                    StringDefinition = data,
                    //Data = data.ToList()
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignView(string filePath, string input) {
            var options = new List<BindConfig.OptionModel>() { };
            var str = $"[^#]view[\\s]*([a-zA-Z0-9.\\-\"_]*[\\s]*{{";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var viewName = matches[i].Value.Replace("{", "").Replace("\"", "").Replace("view", "").Trim();
                var viewStr = $"[^#]view[\\s]*\"{viewName}\"[\\s]*{{[\\s]*((?!con|logg|ser|sta|acl|opt|tru|zon|inc|key)[a-zA-Z0-9\\s;\"/_.}}{{-])*;";
                var viewRegex = new Regex(viewStr);
                var viewMatches = viewRegex.Matches(input);
                var data = "";
                if (viewMatches.Count > 0) {
                    data = viewMatches[0].Value;
                }
                var option = new BindConfig.OptionModel() {
                    FilePath = filePath,
                    Name = viewName,
                    StringDefinition = data,
                    //Data = data.ToList()
                };
                options.Add(option);
            }
            return options;
        }

        public static IEnumerable<BindConfig.OptionModel> AssignZone(string filePath, string input) {
            var options = new List<BindConfig.OptionModel>() { };
            var str = $"[^#]zone[\\s]*[{"\""}]*([a-zA-Z0-9.\\-_]*)[{"\""}]*[\\s]*{{";
            var regex = new Regex(str);
            var matches = regex.Matches(input);
            for (int i = 0; i < matches.Count; i++) {
                var zoneName = matches[i].Value.Replace("{", "").Replace("\"", "").Replace("zone", "").Trim();
                var zoneStr = $"[^#]zone[\\s]*\"{zoneName}\"[\\s]*{{[\\s]*((?!zone[\\s]*\")[a-zA-Z0-9\\s;\"/_.}}{{-])*;";
                var zoneRegex = new Regex(zoneStr);
                var zoneMatches = zoneRegex.Matches(input);
                var data = "";
                if (zoneMatches.Count > 0) {
                    data = zoneMatches[0].Value;
                }
                var option = new BindConfig.OptionModel() {
                    FilePath = filePath,
                    Name = zoneName,
                    StringDefinition = data,
                    //Data = data.ToList()
                };
                options.Add(option);
            }
            return options;
        }
    }
}
