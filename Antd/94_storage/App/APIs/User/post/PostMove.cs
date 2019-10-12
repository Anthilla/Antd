using System;
using WatsonWebserver;

namespace Kvpbase {
    public partial class StorageServer {
        public static HttpResponse PostMove(RequestMetadata md) {
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
                ContainerHandler ch = new ContainerHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                return ch.ContainerMove(md);
            }
            else {
                ObjectHandler oh = new ObjectHandler(CurrentSettings, CurrentTopology, CurrentNode, Users, LockManager, EncryptionManager);
                return oh.ObjectMove(md);
            }

            #endregion
        }
    }
}