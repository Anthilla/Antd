//using System.Collections.Generic;
//using System.IO;
//using antdlib.common;
//using Antd.Database;
//using Newtonsoft.Json;

//namespace Antd.Configuration {
//    public class MachineConfiguration {

//        private static readonly string FilePath = $"{Parameter.RepoConfig}/machine.conf";

//        public void Set() {
//            Directory.CreateDirectory(Parameter.RepoConfig);
//            if(!File.Exists(FilePath)) {
//                ConsoleLogger.Log("machine configuration file does not exist");
//                var tempFlow = new AntdConfigurationModel {
//                    Name = ".machine.conf",
//                    Path = $"{Parameter.RepoConfig}/.machine.conf",
//                    LoadModules = new List<string> { "module#1", "module#2" },
//                    LoadServices = new List<string> { "service#1", "service#2" },
//                    LoadOsParameters = new Dictionary<string, string> { { "file", "value" } }
//                };
//                if(!File.Exists(tempFlow.Path)) {
//                    File.WriteAllText(tempFlow.Path, JsonConvert.SerializeObject(tempFlow, Formatting.Indented));
//                    ConsoleLogger.Log("a machine configuration file template has been created");
//                }
//                return;
//            }
//            var text = File.ReadAllText(FilePath);
//            var configuration = JsonConvert.DeserializeObject<AntdConfigurationModel>(text);
//        }

//        public void Export(List<string> loadModules, List<string> loadService, Dictionary<string, string> loadOsParam) {
//            Directory.CreateDirectory(Parameter.RepoConfig);
//            if(File.Exists(FilePath)) {
//                File.Delete(FilePath);
//            }
//            var configuration = new AntdConfigurationModel {
//                Name = "machine.conf",
//                Path = FilePath,
//                LoadModules = loadModules,
//                LoadServices = loadService,
//                LoadOsParameters = loadOsParam
//            };
//            if(!File.Exists(FilePath)) {
//                File.WriteAllText(FilePath, JsonConvert.SerializeObject(configuration, Formatting.Indented));
//            }
//        }
//    }
//}
