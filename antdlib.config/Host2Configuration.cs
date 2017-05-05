using System;
using System.IO;
using antdlib.common;
using antdlib.models;
using Newtonsoft.Json;

namespace antdlib.config {
    public class Host2Configuration {
        public static string FilePath => $"{Parameter.AntdCfg}/localhost.conf";
        public static string FilePathBackup => $"{Parameter.AntdCfg}/localhost.conf.bck";
        public static Host2Model Host => LoadModel();

        private static Host2Model LoadModel() {
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

        public static void Export(Host2Model model) {
            if(File.Exists(FilePath)) {
                File.Copy(FilePath, $"{FilePath}.bck", true);
            }
            File.WriteAllText(FilePath, JsonConvert.SerializeObject(model, Formatting.Indented));
        }
    }
}
