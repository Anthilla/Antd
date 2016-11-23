using System;
using System.IO;
using antdlib.common;
using Newtonsoft.Json;

namespace Antd.Configuration {
    public class HostConfiguration {

        private static readonly string FilePath = $"{Parameter.AntdCfg}/host.conf";
        private static readonly string FilePathBackup = $"{Parameter.AntdCfg}/host.conf.bck";

        public HostModel Load() {
            if(!File.Exists(FilePath)) {
                return new HostModel();
            }
            try {
                return JsonConvert.DeserializeObject<HostModel>(File.ReadAllText(FilePath));
            }
            catch(Exception) {
                return new HostModel();
            }
        }

        public void Setup() {
            Directory.CreateDirectory(Parameter.RepoConfig);
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
    }
}
