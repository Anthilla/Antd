using System;
using WatsonWebserver;

namespace Kvpbase {
    public partial class StorageServer {
        public static HttpResponse PostRename(RequestMetadata md) {
            #region Variables

            string containerVal = "";
            bool container = false;

            #endregion

            #region Get-Values-from-Querystring

            containerVal = md.CurrentHttpRequest.RetrieveHeaderValue("container");
            if(!String.IsNullOrEmpty(containerVal)) {
                container = Common.IsTrue(containerVal);
            }

            #endregion

            #region Process

            if(container) {
                ContainerHandler ch = new ContainerHandler(CurrentSettings, CurrentTopology, CurrentNode, Users, Maintenance, Logger);
                return ch.ContainerRename(md);
            }
            else {
                ObjectHandler oh = new ObjectHandler(CurrentSettings, CurrentTopology, CurrentNode, Users, LockManager, Maintenance, EncryptionManager, Logger);
                return oh.ObjectRename(md);
            }

            #endregion
        }
    }
}