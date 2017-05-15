using antdlib.common;
using antdlib.models;
using anthilla.commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace antdlib.config {
    public static class FirewallConfiguration {

        private static FirewallConfigurationModel ServiceModel => Load();
        private static readonly string CfgFile = $"{Parameter.AntdCfgServices}/firewall.conf";
        private const string MainFilePath = "/etc/nftables.conf";

        private static FirewallConfigurationModel Load() {
            if(!File.Exists(CfgFile)) {
                return new FirewallConfigurationModel();
            }
            try {
                var text = File.ReadAllText(CfgFile);
                var obj = JsonConvert.DeserializeObject<FirewallConfigurationModel>(text);
                return obj;
            }
            catch(Exception) {
                return new FirewallConfigurationModel();
            }
        }

        public static void Save(FirewallConfigurationModel model) {
            var text = JsonConvert.SerializeObject(model, Formatting.Indented);
            FileWithAcl.WriteAllText(CfgFile, text, "644", "root", "wheel");
            ConsoleLogger.Log("[firewall] configuration saved");
        }

        public static void Set() {
            Enable();
            #region [    nftables.conf generation    ]
            var lines = new List<string> {
                "flush ruleset;",
                "flush ruleset;",
                "flush ruleset;",
                "flush ruleset;",
                "flush ruleset;"
            };
            var table = ServiceModel.Ipv4FilterTable;
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
            table = ServiceModel.Ipv4NatTable;
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
            table = ServiceModel.Ipv6FilterTable;
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
            table = ServiceModel.Ipv6NatTable;
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
            FileWithAcl.WriteAllLines(MainFilePath, lines, "644", "root", "wheel");
            #endregion
            Start();
        }

        public static bool IsActive() {
            if(!File.Exists(CfgFile)) {
                return false;
            }
            return ServiceModel != null && ServiceModel.IsActive;
        }

        public static FirewallConfigurationModel Get() {
            return ServiceModel;
        }

        public static void Enable() {
            ServiceModel.IsActive = true;
            Save(ServiceModel);
            ConsoleLogger.Log("[firewall] enabled");
        }

        public static void Disable() {
            ServiceModel.IsActive = false;
            Save(ServiceModel);
            ConsoleLogger.Log("[firewall] disabled");
        }

        public static void Stop() {
            ConsoleLogger.Log("[firewall] stop");
        }

        public static void Start() {
            if(!IsActive())
                return;
            CommandLauncher.Launch("nft-f", new Dictionary<string, string> { { "$file", MainFilePath } });
            ConsoleLogger.Log("[firewall] start");
        }
    }
}
