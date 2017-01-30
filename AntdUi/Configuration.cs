using System.IO;

namespace AntdUi {
    public class Configuration {

        public static void Setup() {
            Directory.CreateDirectory(Parameter.DirectoryCfg);
        }
    }
}
