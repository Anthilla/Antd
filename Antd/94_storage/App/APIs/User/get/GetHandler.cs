using WatsonWebserver;

namespace Kvpbase {
    public partial class StorageServer {
        public static HttpResponse GetHandler(RequestMetadata md) {
            #region Process-by-Operation-Type

            if(Common.IsTrue(md.CurrentObj.IsContainer)) {
                ContainerHandler ch = new ContainerHandler(CurrentSettings, CurrentTopology, CurrentNode, Users);
                return ch.ContainerRead(md);
            }
            else {
                ObjectHandler oh = new ObjectHandler(CurrentSettings, CurrentTopology, CurrentNode, Users, LockManager, EncryptionManager);
                return oh.ObjectRead(md);
            }

            #endregion
        }
    }
}