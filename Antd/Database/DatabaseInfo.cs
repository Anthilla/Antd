using Antd.Boot;
using System;
using System.Collections.Generic;
using System.IO;

namespace Antd.Database {
    public class DatabaseInfo {
        private static string GetVersion() {
            return DeNSo.Configuration.Version.ToString();
        }

        public static string Version { get { return GetVersion(); } }

        private static string GetName() {
            return CoreParametersConfig.GetAntdDb();
        }

        public static string Name { get { return GetName(); } }

        private static string GetPath() {
            var dbName = CoreParametersConfig.GetAntdDb();
            var directories = new DirectoryFinder("/", dbName).List;
            return string.Join(", ", directories).ToString();
        }

        public static string Path { get { return GetPath(); } }

        private static string GetJnlPath() {
            var files = new DirectoryFinder("/", "*.jnl").List;
            return string.Join(", ", files).ToString();
        }

        public static string JournalPath { get { return GetJnlPath(); } }

        private static string[] GetDatabaseRaidPaths() {
            var files = new string[] { };
            return files;
        }

        public static string[] RaidPaths { get { return GetDatabaseRaidPaths(); } }
    }
}
