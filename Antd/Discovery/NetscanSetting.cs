using System;
using System.IO;
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
                File.WriteAllText(FilePath, JsonConvert.SerializeObject(new HostModel(), Formatting.Indented));
            }
        }

        public void Export(HostModel model) {
            if(File.Exists(FilePath)) {
                File.Copy(FilePath, FilePathBackup, true);
            }
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(model, Formatting.Indented));
        }

        #region [    repo    ] 
        public void SetSubnet(string subnet) {
            if(string.IsNullOrEmpty(subnet)) { return; }
            Settings = LoadSettingsModel();
            Settings.Subnet = subnet;
            Setup();
        }

        public void SetLabel(string letter, string number, string label) {
            Settings = LoadSettingsModel();
            var objects = Settings.Values;
            var mo = new NetscanLabel { Letter = letter, Number = number };
            objects[mo] = label;
            Settings.Values = objects;
            Setup();
        }
        #endregion
    }
}
