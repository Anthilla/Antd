using Antd.models;
using KvpbaseSDK;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace Antd.cmds {

    /// <summary>
    /// USAGE:
    /// var client = new Client("default", "default", $"http://{node.PublicIp}:8080");
    /// VfsClient.CreateObject(client, containerPath, fileType, data, fileName);
    /// 
    /// Dove l'indirizzo + porta sono quelli del VfsServer
    /// </summary>
    public class VfsClient {

        #region [    Object Actions    ]
        public static bool CreateObject(Client kvp, string containerPath, string fileType, byte[] data, string fileName = "") {
            string url;
            return string.IsNullOrEmpty(fileName) ?
                kvp.CreateObjectWithoutName(containerPath, fileType, data, out url) :
                kvp.CreateObjectWithName(containerPath, fileName, fileType, data, out url);
        }

        public static byte[] RetrieveObject(Client kvp, string objectPath) {
            byte[] data;
            return kvp.GetObject(objectPath, out data) == false ? null : data;
        }

        public static bool VerifyObject(Client kvp, string objectPath) {
            return kvp.ObjectExists(objectPath);
        }

        public static bool MoveObject(Client kvp, string objectPath, string newContainer) {
            var objectName = objectPath.Split('/').LastOrDefault();
            var objectContainer = objectPath.Replace(objectName, "").TrimEnd('/');
            return kvp.MoveObject(objectContainer, objectName, newContainer, objectName);
        }

        public static bool RenameObject(Client kvp, string objectPath, string newName) {
            var objectName = objectPath.Split('/').LastOrDefault();
            var objectContainer = objectPath.Replace(objectName, "").TrimEnd('/');
            return kvp.RenameObject(objectContainer, objectName, newName);
        }

        public static bool DeleteObject(Client kvp, string objectPath) {
            return kvp.DeleteObject(objectPath);
        }
        #endregion

        #region [    Container Actions    ]
        public static bool CreateContainer(Client kvp, string containerPath) {
            string url;
            return kvp.CreateContainer(containerPath, out url);
        }

        public static VfsContainerInfo GetContainer(Client kvp, string containerPath) {
            byte[] data;
            if(!kvp.GetContainer(containerPath, out data)) {
                return null;
            }
            else {
                var json = Encoding.UTF8.GetString(data);
                var containerInfo = JsonConvert.DeserializeObject<VfsContainerInfo>(json);
                return containerInfo;
            }
        }

        public static bool VerifyContainer(Client kvp, string containerPath) {
            return kvp.ContainerExists(containerPath);
        }

        public static bool MoveContainer(Client kvp, string containerPath, string newContainer) {
            var containerName = containerPath.Split('/').LastOrDefault();
            var containerContainer = containerPath.Replace(containerName, "").TrimEnd('/');
            return kvp.MoveContainer(containerContainer, containerName, newContainer, containerName);
        }

        public static bool RenameContainer(Client kvp, string containerPath, string newName) {
            var containerName = containerPath.Split('/').LastOrDefault();
            var containerContainer = containerPath.Replace(containerName, "").TrimEnd('/');
            return kvp.RenameContainer(containerContainer, containerName, newName);
        }

        public static bool DeleteContainer(Client kvp, string containerPath) {
            return kvp.DeleteContainer(containerPath, false);
        }
        #endregion
    }
}
