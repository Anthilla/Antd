using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Kvpbase {
    public partial class StorageServer {
        public static Settings CurrentSettings;

        public static UserManager Users;
        public static ApiKeyManager ApiKeys;
        public static Topology CurrentTopology;
        public static Node CurrentNode;

        public static ConnectionManager ConnManager;
        public static EncryptionModule EncryptionManager;
        public static UrlLockManager LockManager;
        public static LoggerManager Logger;
        public static MaintenanceManager Maintenance;
        public static ConsoleManager CurrentConsole;
        public static ConcurrentQueue<Dictionary<string, object>> FailedRequests;
    }
}
