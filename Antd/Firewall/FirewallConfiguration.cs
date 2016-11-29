using System;
using System.Collections.Generic;
using System.IO;
using antd.commands;
using antdlib.common;
using Newtonsoft.Json;
using IoDir = System.IO.Directory;

namespace Antd.Firewall {
    public class FirewallConfiguration {

        private readonly FirewallConfigurationModel _serviceModel;

        private readonly string _cfgFile = $"{Parameter.AntdCfgServices}/firewall.conf";
        private readonly string _cfgFileBackup = $"{Parameter.AntdCfgServices}/firewall.conf.bck";
        private const string MainFilePath = "/etc/nftables.conf";

        public FirewallConfiguration() {
            IoDir.CreateDirectory(Parameter.AntdCfgServices);
            if(!File.Exists(_cfgFile)) {
                _serviceModel = new FirewallConfigurationModel();
            }
            else {
                try {
                    var text = File.ReadAllText(_cfgFile);
                    var obj = JsonConvert.DeserializeObject<FirewallConfigurationModel>(text);
                    _serviceModel = obj;
                }
                catch(Exception) {
                    _serviceModel = new FirewallConfigurationModel();
                }

            }
        }

        public void Save(FirewallConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            if(File.Exists(_cfgFile)) {
                File.Copy(_cfgFile, _cfgFileBackup, true);
            }
            File.WriteAllText(_cfgFile, text);
        }

        public void Set() {
            if(_serviceModel == null) {
                return;
            }
            Enable();

            #region [    nftables.conf generation    ]
            var lines = new List<string> {
                "flush ruleset;",
                "flush ruleset;",
                "flush ruleset;",
                "flush ruleset;",
                "flush ruleset;"
            };
            var table = _serviceModel.Ipv4FilterTable;
            lines.Add($"table {table.Type} {table.Name} {{");
            foreach(var set in table.Sets) {
                lines.Add($"    set {set.Name} {{");
                lines.Add($"        type {set.Type}");
                lines.Add($"        elements = {{ {set.Elements}}}");
                lines.Add("    }");
            }
            foreach(var chain in table.Chains) {
                lines.Add($"    chain {chain.Name} {{");
                foreach(var rule in chain.Rules) {
                    lines.Add($"        {rule}");
                }
                lines.Add("    }");

            }
            lines.Add("}");
            table = _serviceModel.Ipv4NatTable;
            lines.Add($"table {table.Type} {table.Name} {{");
            foreach(var set in table.Sets) {
                lines.Add($"    set {set.Name} {{");
                lines.Add($"        type {set.Type}");
                lines.Add($"        elements = {{ {set.Elements}}}");
                lines.Add("    }");
            }
            foreach(var chain in table.Chains) {
                lines.Add($"    chain {chain.Name} {{");
                foreach(var rule in chain.Rules) {
                    lines.Add($"        {rule}");
                }
                lines.Add("    }");

            }
            lines.Add("}");
            table = _serviceModel.Ipv6FilterTable;
            lines.Add($"table {table.Type} {table.Name} {{");
            foreach(var set in table.Sets) {
                lines.Add($"    set {set.Name} {{");
                lines.Add($"        type {set.Type}");
                lines.Add($"        elements = {{ {set.Elements}}}");
                lines.Add("    }");
            }
            foreach(var chain in table.Chains) {
                lines.Add($"    chain {chain.Name} {{");
                foreach(var rule in chain.Rules) {
                    lines.Add($"        {rule}");
                }
                lines.Add("    }");

            }
            lines.Add("}");
            table = _serviceModel.Ipv6NatTable;
            lines.Add($"table {table.Type} {table.Name} {{");
            foreach(var set in table.Sets) {
                lines.Add($"    set {set.Name} {{");
                lines.Add($"        type {set.Type}");
                lines.Add($"        elements = {{ {set.Elements}}}");
                lines.Add("    }");
            }
            foreach(var chain in table.Chains) {
                lines.Add($"    chain {chain.Name} {{");
                foreach(var rule in chain.Rules) {
                    lines.Add($"        {rule}");
                }
                lines.Add("    }");

            }
            lines.Add("}");
            File.WriteAllLines(MainFilePath, lines);
            #endregion

            Restart();
        }

        public bool IsActive() {
            if(!File.Exists(_cfgFile)) {
                return false;
            }
            return _serviceModel != null && _serviceModel.IsActive;
        }

        public FirewallConfigurationModel Get() {
            return _serviceModel;
        }

        public void Enable() {
            if(_serviceModel == null) {
                return;
            }
            _serviceModel.IsActive = true;
            Save(_serviceModel);
        }

        public void Disable() {
            if(_serviceModel == null) {
                return;
            }
            _serviceModel.IsActive = false;
            Save(_serviceModel);
        }

        public void Stop() {
        }

        public void Restart() {
            if(!IsActive())
                return;
            var launcher = new CommandLauncher();
            launcher.Launch("nft-f", new Dictionary<string, string> { { "$file", MainFilePath } });
        }
    }
}
