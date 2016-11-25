using System;
using System.IO;
using System.Linq;
using antdlib.common;
using Antd.Host;
using Newtonsoft.Json;

namespace Antd.Discovery {
    public class NetscanSetting {

        public string FilePath { get; }
        public string FilePathBackup { get; }
        public NetscanSettingModel Settings { get; private set; }

        public NetscanSetting() {
            FilePath = $"{Parameter.AntdCfg}/services/netscan.conf";
            FilePathBackup = $"{Parameter.AntdCfg}/services/netscan.conf.bck";
            Settings = LoadSettingsModel();
        }

        private NetscanSettingModel LoadSettingsModel() {
            if(!File.Exists(FilePath)) {
                return new NetscanSettingModel();
            }
            try {
                return JsonConvert.DeserializeObject<NetscanSettingModel>(File.ReadAllText(FilePath));
            }
            catch(Exception) {
                return new NetscanSettingModel();
            }
        }

        public void Setup() {
            if(!File.Exists(FilePath)) {
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(new NetscanSettingModel(), Formatting.Indented));
            }
        }

        public void Export(NetscanSettingModel model) {
            if(File.Exists(FilePath)) {
                File.Copy(FilePath, FilePathBackup, true);
            }
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(model, Formatting.Indented));
        }

        #region [    repo    ] 
        public void SetSubnet(string subnet, string label) {
            if(string.IsNullOrEmpty(subnet)) { return; }
            Settings = LoadSettingsModel();
            Settings.Subnet = subnet;
            Export(Settings);
        }

        public void SetLabel(string letter, string number, string label) {
            Settings = LoadSettingsModel();
            var objects = Settings.Values;
            var mo = objects.FirstOrDefault(_ => _.Item1 == number && _.Item2 == letter);
            if(mo == null) { return; }
            objects.Remove(mo);
            var nmo = new Tuple<string, string, string>(number, letter, label);
            objects.Add(nmo);
            Settings.Values = objects;
            Export(Settings);
        }
        #endregion

        public void SaveEtcNetworks() {
            Settings = LoadSettingsModel();
            var settings = Settings.Values.Where(_ => !string.IsNullOrEmpty(_.Item3)).Select(
                _ => $"{Settings.SubnetLabel}-{_.Item3} {Settings.Subnet}{_.Item1}.0"
            );
            var hostConfiguration = new HostConfiguration();
            foreach(var set in settings) {
                hostConfiguration.SetHostEtcNetworks(set);
            }
        }
    }
}
