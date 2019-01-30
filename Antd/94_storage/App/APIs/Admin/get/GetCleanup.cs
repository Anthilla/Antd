using anthilla.core;
using System;
using System.Threading.Tasks;
using WatsonWebserver;

namespace Kvpbase {
    public partial class StorageServer {
        public static HttpResponse GetCleanup(RequestMetadata md) {
            Task.Run(() => CleanupThread());
            return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", null, true);
        }

        public static void CleanupThread() {
            DateTime startTime = DateTime.Now;

            ConsoleLogger.Log("CleanupThread starting cleanup at " + startTime);
            ConsoleLogger.Log("CleanupThread ending cleanup after " + Common.TotalMsFrom(startTime) + "ms");
            return;
        }
    }
}