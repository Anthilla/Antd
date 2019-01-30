using anthilla.core;
using System;
using WatsonWebserver;

namespace Kvpbase {
    public partial class StorageServer {
        public static HttpResponse PostKill(RequestMetadata md) {
            try {
                ConsoleLogger.Log("");
                ConsoleLogger.Log("PostKill requested at " + DateTime.Now + " by " + md.CurrentHttpRequest.SourceIp + ":" + md.CurrentHttpRequest.SourcePort);
                ConsoleLogger.Log("");

                return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "application/json", null, true);
            }
            finally {
                Common.ExitApplication("PostKill", "Kill requested at " + DateTime.Now + " by " + md.CurrentHttpRequest.SourceIp + ":" + md.CurrentHttpRequest.SourcePort, -1);
            }
        }
    }
}