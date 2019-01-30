﻿using anthilla.core;
using System;
using WatsonWebserver;

namespace Kvpbase {
    public partial class StorageServer {
        public static HttpResponse PutHandler(RequestMetadata md) {
            if(Common.IsTrue(md.CurrentObj.IsContainer)) {
                ContainerHandler ch = new ContainerHandler(CurrentSettings, CurrentTopology, CurrentNode, Users, Maintenance, Logger);
                return ch.ContainerWrite(md);
            }
            else {
                if(String.IsNullOrEmpty(md.CurrentObj.Key)) {
                    ConsoleLogger.Warn("PutHandler unable to find key in URL");
                    return new HttpResponse(md.CurrentHttpRequest, false, 400, null, "application/json",
                        new ErrorResponse(2, 400, "Unable to find object key in URL.", null).ToJson(), true);
                }

                ObjectHandler oh = new ObjectHandler(CurrentSettings, CurrentTopology, CurrentNode, Users, LockManager, Maintenance, EncryptionManager, Logger);
                return oh.ObjectWrite(md);
            }
        }
    }
}