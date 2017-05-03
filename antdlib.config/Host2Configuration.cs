using System;
using System.IO;
using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;

namespace antdlib.config {
    public class Host2Configuration {
        public string FilePath { get; }
        public string FilePathBackup { get; }
        public Host2Model Host { get; }

        public Host2Configuration() {
            FilePath = $"{Parameter.AntdCfg}/localhost.conf";
            FilePathBackup = $"{Parameter.AntdCfg}/localhost.conf.bck";
            Host = LoadModel();
        }

        private Host2Model LoadModel() {
            if(!File.Exists(FilePath)) {
                return new Host2Model();
            }
            try {
                return JsonConvert.DeserializeObject<Host2Model>(File.ReadAllText(FilePath));
            }
            catch(Exception) {
                return new Host2Model();
            }
        }

        public void Export(Host2Model model) {
            if(File.Exists(FilePath)) {
                File.Copy(FilePath, $"{FilePath}.bck", true);
            }
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(model, Formatting.Indented));
        }
    }
}
