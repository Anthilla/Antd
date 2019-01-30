﻿using WatsonWebserver;

namespace Kvpbase {
    public partial class StorageServer {
        public static HttpResponse HeadHandler(RequestMetadata md) {
            #region Process-by-Operation-Type

            if(Common.IsTrue(md.CurrentObj.IsContainer)) {
                #region Get-Container

                ContainerHandler ch = new ContainerHandler(CurrentSettings, CurrentTopology, CurrentNode, Users, Maintenance, Logger);
                return ch.ContainerHead(md);

                #endregion
            }
            else {
                #region Get-Object

                ObjectHandler oh = new ObjectHandler(CurrentSettings, CurrentTopology, CurrentNode, Users, LockManager, Maintenance, EncryptionManager, Logger);
                return oh.ObjectHead(md);

                #endregion
            }

            #endregion
        }
    }
}