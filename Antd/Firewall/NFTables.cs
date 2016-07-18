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

namespace Antd.Firewall {
    public class NfTables {
        //public static IEnumerable<ConfigManagement.CommandsBundle> GetNftCommandsBundle() {
        //    return ConfigManagement.GetCommandsBundle().Where(_ => _.Command.StartsWith("nft"));
        //}

        //public static void AddNftRule(string prefix, string rule) {
        //    ConfigManagement.AddCommandsBundle($"{prefix} {rule}");
        //    if (rule.Length > 0) {
        //        Terminal.Terminal.Execute($"{prefix} {rule}");
        //    }
        //}

        //public static void DeleteNftRule(string guid) {
        //    var command = ConfigManagement.GetCommandsBundle().Where(_ => _.Guid == guid).Select(_ => _.Command).FirstOrDefault();
        //    if (command == null || command.Length <= 0)
        //        return;
        //    var chain = command.Split(' ')[4];
        //    var hook = command.Split(' ')[5];
        //    var checkTables = Terminal.Terminal.Execute($"nft list table {chain} -a | grep \"{command}\"");
        //    if (checkTables.Length > 0) {
        //        var handle = checkTables.Split(' ').Last();
        //        Terminal.Terminal.Execute($"nft delete rule {chain} {hook} handle {handle}");
        //    }
        //    ConfigManagement.DeleteCommandsBundle(guid);
        //}

        //public class Export {
        //    private static string GetLastFileName() {
        //        var configName = Directory.EnumerateFiles(Parameter.RepoConfig, "*firewall.export", SearchOption.TopDirectoryOnly).Select(Path.GetFileName).ToArray();
        //        if (!configName.Any())
        //            return "0_antd_firewall.export";
        //        var lastFile = configName.Last();
        //        var num = Convert.ToInt32(lastFile.Split('_')[0]);
        //        return $"{num + 1}_antd_firewall.export";
        //    }

        //    public static void ExportTemplate() {
        //        var savedtemplate = $"{Parameter.Resources}/antd.firewall.template.conf";
        //        var storedtemplate = $"{Parameter.RepoConfig}/antd.firewall.template.conf";
        //        if (!File.Exists(storedtemplate)) {
        //            Terminal.Terminal.Execute($"cp {savedtemplate} {storedtemplate}");
        //        }
        //    }

        //    public static void ExportNewFirewallConfiguration() {
        //        var template = $"{Parameter.RepoConfig}/antd.firewall.template.conf";
        //        var text = Terminal.Terminal.Execute($"cat {template}");
        //        foreach (var values in FirewallLists.GetAll()) {
        //            var replace = text.Replace(values.TemplateWord, ConfigManagement.SupposeCommandReplacement(values.ReplaceTemplateValues));
        //            text = replace;
        //        }
        //        var newConfFile = GetLastFileName();
        //        File.WriteAllText(newConfFile, text);
        //    }

        //    public static void ApplyConfiguration(string filename = "") {
        //        var fileToApply = filename.Length < 1 && !File.Exists(filename) ? GetLastFileName() : filename;
        //        Terminal.Terminal.Execute($"nft -f {fileToApply}");
        //    }
        //}
    }
}
