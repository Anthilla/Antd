using Antd.Boot;
using Newtonsoft.Json;

namespace Antd.Database {
    public class AntdDatabase {
        private static string GetVersion() {
            return DeNSo.Configuration.Version.ToString();
        }

        public static string Version { get { return GetVersion(); } }

        private static string GetName() {
            return CoreParametersConfig.GetDb();
        }

        public static string Name { get { return GetName(); } }

        private static string GetPath() {
            var dbName = CoreParametersConfig.GetDb();
            var directories = new DirectoryFinder("/", dbName).List;
            return string.Join(", ", directories).ToString();
        }

        public static string Path { get { return GetPath(); } }

        private static string GetJnlPath() {
            var files = new DirectoryFinder("/", "*.jnl").List;
            return string.Join(", ", files).ToString();
        }

        public static string JournalPath { get { return GetJnlPath(); } }

        private static string GetDatabaseRaidPaths() {
            var files = JsonConvert.DeserializeObject<string[]>(ParametersConfig.Read("antddbraid"));
            return string.Join(", ", files).ToString();
        }

        public static string RaidPaths { get { return GetDatabaseRaidPaths(); } }

        public static void AddRaidPath(string path) {
            ParametersConfig.Write("antddbraid", path);
        }
    }
}
