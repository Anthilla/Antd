using System;
using System.IO;
using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;

namespace antdlib.config {
    public class VariablesConfiguration {
        public string FilePath { get; }
        public string FilePathBackup { get; }
        public VariablesModel Host { get; private set; }

        public VariablesConfiguration() {
            FilePath = $"{Parameter.AntdCfg}/localvar.conf";
            FilePathBackup = $"{Parameter.AntdCfg}/localvar.conf.bck";
            Host = LoadModel();
        }

        private VariablesModel LoadModel() {
            if(!File.Exists(FilePath)) {
                return new VariablesModel();
            }
            try {
                return JsonConvert.DeserializeObject<VariablesModel>(File.ReadAllText(FilePath));
            }
            catch(Exception) {
                return new VariablesModel();
            }
        }

        public void Export(VariablesModel model) {
            if(File.Exists(FilePath)) {
                File.Copy(FilePath, $"{FilePath}.bck", true);
            }
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(model, Formatting.Indented));
        }
    }
}
