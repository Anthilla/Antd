namespace Antd {

    public class SelfConfig {
        private static string cfgAlias = "antdControlSet";

        private static string[] paths = new string[] {
                cfgAlias + "Current",
                cfgAlias + "001",
                cfgAlias + "002"
            };

        private static Anth_ParamWriter config = new Anth_ParamWriter(paths, "antdCore");

        public static void WriteDefaults() {
            string root;
            config.Write("root", "/antd");
            root = config.ReadValue("root");

            if (config.CheckValue("antdport") == false) {
                config.Write("antdport", "7777");
            }

            if (config.CheckValue("antddb") == false) {
                config.Write("antddb", "/Data/Data01Vol01/antdData");
            }

            if (config.CheckValue("antdfr") == false) {
                config.Write("antdfr", "/Data/Data01Vol01/antdData");
            }

            if (config.CheckValue("sysd") == false) {
                config.Write("sysd", "/etc/systemd/system");
            }
        }

        public static string GetAntdPort() {
            return config.ReadValue("antdport");
        }

        public static string GetAntdDb() {
            return config.ReadValue("antddb");
        }

        public static string GetAntdRepo() {
            return config.ReadValue("antdfr");
        }

        public static string GetUnitDir() {
            return config.ReadValue("sysd");
        }

        public static string GetAntdUri() {
            if (config.CheckValue("antddb") == false) {
                return "http://+:7777/";
            }
            else {
                var port = config.ReadValue("antdport");
                return "http://+:" + port + "/";
            }
        }
    }
}