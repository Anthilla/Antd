using System;
using System.IO;
using antdlib.models;
using anthilla.core;
using Newtonsoft.Json;
using Parameter = antdlib.common.Parameter;

namespace antdlib.config {
    public class Host2Configuration {
        public static string FilePath => $"{Parameter.AntdCfg}/localhost.conf";
        public static string FilePathBackup => $"{Parameter.AntdCfg}/localhost.conf.bck";
        public static Host2Model Host => LoadModel();

        private static Host2Model LoadModel() {
            if(!File.Exists(FilePath)) {
                return new Host2Model();
            }
            return JsonConvert.DeserializeObject<Host2Model>(File.ReadAllText(FilePath));
        }

        public static void Export(Host2Model model) {
            FileWithAcl.WriteAllText(FilePath, JsonConvert.SerializeObject(model, Formatting.Indented), "644", "root", "wheel");
        }
    }
}
