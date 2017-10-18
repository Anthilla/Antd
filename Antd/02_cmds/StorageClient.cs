using Antd.models;
using anthilla.core;
using Newtonsoft.Json;
using RestSharp;
using System.IO;

namespace Antd.cmds {

    /// <summary>
    /// USAGE:
    /// var client = new Client("default", "default", $"http://{node.PublicIp}:8080");
    /// VfsClient.CreateFile(client, folderPath, fileType, data, fileName);
    /// 
    /// Dove l'indirizzo + porta sono quelli del VfsServer
    /// </summary>
    public class StorageClient {

        #region [    Header Keys    ]
        private const string HeaderMachineUidKey = "A_MACHINE_UID";
        private const string HeaderFolderPathKey = "A_FOLDER_PATH";
        private const string HeaderFolderNewPathKey = "A_FOLDER_NEWPATH";
        private const string HeaderFolderNewNameKey = "A_FOLDER_NEWNAME";
        private const string HeaderFilePathKey = "A_FILE_PATH";
        private const string HeaderFileNewPathKey = "A_FILE_NEWPATH";
        private const string HeaderFileNewNameKey = "A_FILE_NEWNAME";
        #endregion

        #region [    Urls    ]
        public const string getFolderServerPath = "/storageserver/folder/get";
        public const string verifyFolderServerPath = "/storageserver/folder/verify";
        public const string createFolderServerPath = "/storageserver/folder/create";
        public const string moveFolderServerPath = "/storageserver/folder/move";
        public const string renameFolderServerPath = "/storageserver/folder/rename";
        public const string deleteFolderServerPath = "/storageserver/folder/delete";

        public const string getFileServerPath = "/storageserver/file/get";
        public const string verifyFileServerPath = "/storageserver/file/verify";
        public const string createFileServerPath = "/storageserver/file/create";
        public const string moveFileServerPath = "/storageserver/file/move";
        public const string renameFileServerPath = "/storageserver/file/rename";
        public const string deleteFileServerPath = "/storageserver/file/delete";
        #endregion

        #region [    Folder Actions    ]
        /// <summary>
        /// Get folder
        /// </summary>
        /// <param name="node">Nodo del cluster di riferimento</param>
        /// <param name="folderPath">Percorso assoluto del file sul nodo di riferimento</param>
        /// <returns>Ottiene un oggetto StorageServerFolder contenente le informazioni sulla cartella richiesta</returns>
        public static StorageServerFolder GetFolder(ClusterNode node, string folderPath) {
            var client = new RestClient(node.EntryPoint);
            var request = new RestRequest(getFolderServerPath, Method.GET);
            request.AddHeader("session-instance-guid", CommonRandom.CrcGuid());
            request.AddHeader(HeaderMachineUidKey, CommonRandom.CrcGuid());
            request.AddHeader(HeaderFolderPathKey, folderPath);
            var response = client.Execute(request);
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: server error");
                return null;
            }
            if(string.IsNullOrEmpty(response.Content)) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: empty result");
                return null;
            }
            ConsoleLogger.Log($"[storage_client] get folder '{folderPath}'");
            return JsonConvert.DeserializeObject<StorageServerFolder>(response.Content);
        }

        /// <summary>
        /// Verify folder
        /// </summary>
        /// <param name="node">Nodo del cluster di riferimento</param>
        /// <param name="folderPath">Percorso assoluto del file sul nodo di riferimento</param>
        /// <returns></returns>
        public static bool VerifyFolder(ClusterNode node, string folderPath) {
            var client = new RestClient(node.EntryPoint);
            var request = new RestRequest(verifyFolderServerPath, Method.GET);
            request.AddHeader("session-instance-guid", CommonRandom.CrcGuid());
            request.AddHeader(HeaderMachineUidKey, CommonRandom.CrcGuid());
            request.AddHeader(HeaderFolderPathKey, folderPath);
            var response = client.Execute(request);
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: server error");
                return false;
            }
            if(string.IsNullOrEmpty(response.Content)) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: empty result");
                return false;
            }
            ConsoleLogger.Log($"[storage_client] verify folder '{folderPath}'");
            return true;
        }

        /// <summary>
        /// Create folder
        /// </summary>
        /// <param name="node">Nodo del cluster di riferimento</param>
        /// <param name="folderPath">Percorso assoluto del file sul nodo di riferimento</param>
        /// <returns></returns>
        public static bool CreateFolder(ClusterNode node, string folderPath) {
            var client = new RestClient(node.EntryPoint);
            var request = new RestRequest(createFolderServerPath, Method.POST);
            request.AddHeader("session-instance-guid", CommonRandom.CrcGuid());
            request.AddHeader(HeaderMachineUidKey, CommonRandom.CrcGuid());
            request.AddHeader(HeaderFolderPathKey, folderPath);
            var response = client.Execute(request);
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: server error");
                return false;
            }
            if(string.IsNullOrEmpty(response.Content)) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: empty result");
                return false;
            }
            ConsoleLogger.Log($"[storage_client] create folder '{folderPath}'");
            return true;
        }

        /// <summary>
        /// Move folder
        /// </summary>
        /// <param name="node">Nodo del cluster di riferimento</param>
        /// <param name="folderPath">Percorso assoluto del file sul nodo di riferimento</param>
        /// <param name="newFolder">Nuova cartella</param>
        /// <returns></returns>
        public static bool MoveFolder(ClusterNode node, string folderPath, string newFolder) {
            var client = new RestClient(node.EntryPoint);
            var request = new RestRequest(moveFolderServerPath, Method.PUT);
            request.AddHeader("session-instance-guid", CommonRandom.CrcGuid());
            request.AddHeader(HeaderMachineUidKey, CommonRandom.CrcGuid());
            request.AddHeader(HeaderFolderPathKey, folderPath);
            request.AddHeader(HeaderFolderNewPathKey, newFolder);
            var response = client.Execute(request);
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: server error");
                return false;
            }
            if(string.IsNullOrEmpty(response.Content)) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: empty result");
                return false;
            }
            ConsoleLogger.Log($"[storage_client] move folder '{folderPath}' to '{newFolder}'");
            return true;
        }

        /// <summary>
        /// Rename folder
        /// </summary>
        /// <param name="node">Nodo del cluster di riferimento</param>
        /// <param name="folderPath">Percorso assoluto del file sul nodo di riferimento</param>
        /// <param name="newName">Nuovo nome della cartella</param>
        /// <returns></returns>
        public static bool RenameFolder(ClusterNode node, string folderPath, string newName) {
            var client = new RestClient(node.EntryPoint);
            var request = new RestRequest(renameFolderServerPath, Method.PUT);
            request.AddHeader("session-instance-guid", CommonRandom.CrcGuid());
            request.AddHeader(HeaderMachineUidKey, CommonRandom.CrcGuid());
            request.AddHeader(HeaderFolderPathKey, folderPath);
            request.AddHeader(HeaderFolderNewNameKey, newName);
            var response = client.Execute(request);
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: server error");
                return false;
            }
            if(string.IsNullOrEmpty(response.Content)) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: empty result");
                return false;
            }
            ConsoleLogger.Log($"[storage_client] rename folder '{folderPath}' to '{newName}'");
            return true;
        }

        /// <summary>
        /// Delete folder
        /// </summary>
        /// <param name="node">Nodo del cluster di riferimento</param>
        /// <param name="folderPath">Percorso assoluto del file sul nodo di riferimento</param>
        /// <returns></returns>
        public static bool DeleteFolder(ClusterNode node, string folderPath) {
            var client = new RestClient(node.EntryPoint);
            var request = new RestRequest(deleteFolderServerPath, Method.DELETE);
            request.AddHeader("session-instance-guid", CommonRandom.CrcGuid());
            request.AddHeader(HeaderMachineUidKey, CommonRandom.CrcGuid());
            request.AddHeader(HeaderFolderPathKey, folderPath);
            var response = client.Execute(request);
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: server error");
                return false;
            }
            if(string.IsNullOrEmpty(response.Content)) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: empty result");
                return false;
            }
            ConsoleLogger.Log($"[storage_client] delete folder '{folderPath}'");
            return true;
        }
        #endregion

        #region [    File Actions    ]
        /// <summary>
        /// Get file
        /// Equivale a un download
        /// </summary>
        /// <param name="node">Nodo del cluster di riferimento</param>
        /// <param name="filePath">Percorso assoluto del file sul nodo di riferimento</param>
        /// <param name="destinationPath">Percorso assoluto del file sul file system locale</param>
        /// <returns></returns>
        public static void GetFile(ClusterNode node, string filePath, string destinationPath) {
            using(var fileStream = new FileStream(filePath, FileMode.Create)) {
                using(var memoryStream = new MemoryStream()) {
                    var client = new RestClient(node.EntryPoint);
                    var request = new RestRequest(getFileServerPath, Method.GET) {
                        //todo usa rsync
                        ResponseWriter = (responseStream) => responseStream.CopyTo(fileStream)
                    };
                    request.AddHeader("session-instance-guid", CommonRandom.CrcGuid());
                    request.AddHeader(HeaderMachineUidKey, CommonRandom.CrcGuid());
                    request.AddHeader(HeaderFilePathKey, filePath);
                    client.DownloadData(request);
                    int data;
                    while((data = memoryStream.ReadByte()) != -1) {
                        memoryStream.WriteByte((byte)data);
                    }
                }
            }
            ConsoleLogger.Log($"[storage_client] get file '{filePath}'");
        }

        /// <summary>
        /// Verify file
        /// Verifica l'esistenza del file
        /// </summary>
        /// <param name="node">Nodo del cluster di riferimento</param>
        /// <param name="filePath">Percorso assoluto del file sul nodo di riferimento</param>
        /// <returns></returns>
        public static bool VerifyFile(ClusterNode node, string filePath) {
            var client = new RestClient(node.EntryPoint);
            var request = new RestRequest(verifyFileServerPath, Method.GET);
            request.AddHeader("session-instance-guid", CommonRandom.CrcGuid());
            request.AddHeader(HeaderMachineUidKey, CommonRandom.CrcGuid());
            request.AddHeader(HeaderFilePathKey, filePath);
            var response = client.Execute(request);
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                ConsoleLogger.Log($"[storage_client] unable to get remote file: server error");
                return false;
            }
            if(string.IsNullOrEmpty(response.Content)) {
                ConsoleLogger.Log($"[storage_client] unable to get remote file: empty result");
                return false;
            }
            ConsoleLogger.Log($"[storage_client] verify file '{filePath}'");
            return true;
        }

        /// <summary>
        /// Create file
        /// Equivale a un upload
        /// </summary>
        /// <param name="sourcePath">Percorso assoluto del file sul nodo locale</param>
        /// <param name="node">Nodo del cluster di riferimento</param>
        /// <param name="destinationPath">Percorso assoluto del file sul nodo di riferimento</param>
        /// <returns></returns>
        public static bool CreateFile(ClusterNode node, string sourcePath, string destinationPath) {
            if(!File.Exists(sourcePath)) {
                ConsoleLogger.Log($"[storage_client] local file '{sourcePath}' not found");
                return false;
            }
            var client = new RestClient(node.EntryPoint);
            var request = new RestRequest(createFileServerPath, Method.POST);
            request.AddHeader("session-instance-guid", CommonRandom.CrcGuid());
            request.AddHeader(HeaderMachineUidKey, CommonRandom.CrcGuid());
            request.AddHeader(HeaderFilePathKey, destinationPath);
            var fileName = Path.GetFileName(sourcePath);
            var bytes = File.ReadAllBytes(sourcePath);
            request.AddFile(fileName, bytes, fileName);
            var response = client.Execute(request);
            var result = response.StatusCode;
            ConsoleLogger.Log($"[storage_client] create file '{destinationPath}'");
            return result == System.Net.HttpStatusCode.OK;
        }

        /// <summary>
        /// Move file
        /// Sposta il file in un altra cartella
        /// Il nome file rimane lo stesso
        /// </summary>
        /// <param name="node">Nodo del cluster di riferimento</param>
        /// <param name="filePath">Percorso assoluto del file sul nodo di riferimento</param>
        /// <param name="newFolder">Nuova cartella</param>
        /// <returns></returns>
        public static bool MoveFile(ClusterNode node, string filePath, string newFolder) {
            var client = new RestClient(node.EntryPoint);
            var request = new RestRequest(moveFileServerPath, Method.PUT);
            request.AddHeader("session-instance-guid", CommonRandom.CrcGuid());
            request.AddHeader(HeaderMachineUidKey, CommonRandom.CrcGuid());
            request.AddHeader(HeaderFilePathKey, filePath);
            request.AddHeader(HeaderFileNewPathKey, newFolder);
            var response = client.Execute(request);
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                ConsoleLogger.Log($"[storage_client] unable to get remote file: server error");
                return false;
            }
            if(string.IsNullOrEmpty(response.Content)) {
                ConsoleLogger.Log($"[storage_client] unable to get remote file: empty result");
                return false;
            }
            ConsoleLogger.Log($"[storage_client] move file '{filePath}' to '{newFolder}'");
            return true;
        }

        /// <summary>
        /// Rename file
        /// La directory del file rimane la stessa
        /// Cambia solo il nome del file
        /// </summary>
        /// <param name="node">Nodo del cluster di riferimento</param>
        /// <param name="filePath">Percorso assoluto del file sul nodo di riferimento</param>
        /// <param name="newName">Nuovo nome del file</param>
        /// <returns></returns>
        public static bool RenameFile(ClusterNode node, string filePath, string newName) {
            var client = new RestClient(node.EntryPoint);
            var request = new RestRequest(renameFileServerPath, Method.PUT);
            request.AddHeader("session-instance-guid", CommonRandom.CrcGuid());
            request.AddHeader(HeaderMachineUidKey, CommonRandom.CrcGuid());
            request.AddHeader(HeaderFilePathKey, filePath);
            request.AddHeader(HeaderFileNewNameKey, newName);
            var response = client.Execute(request);
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: server error");
                return false;
            }
            if(string.IsNullOrEmpty(response.Content)) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: empty result");
                return false;
            }
            ConsoleLogger.Log($"[storage_client] rename file '{filePath}' to '{newName}'");
            return true;
        }

        /// <summary>
        /// Delete file
        /// Elimina il file
        /// </summary>
        /// <param name="node">Nodo del cluster di riferimento</param>
        /// <param name="filePath">Percorso assoluto del file sul nodo di riferimento</param>
        /// <returns></returns>
        public static bool DeleteFile(ClusterNode node, string filePath) {
            var client = new RestClient(node.EntryPoint);
            var request = new RestRequest(deleteFileServerPath, Method.DELETE);
            request.AddHeader("session-instance-guid", CommonRandom.CrcGuid());
            request.AddHeader(HeaderMachineUidKey, CommonRandom.CrcGuid());
            request.AddHeader(HeaderFilePathKey, filePath);
            var response = client.Execute(request);
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: server error");
                return false;
            }
            if(string.IsNullOrEmpty(response.Content)) {
                ConsoleLogger.Log($"[storage_client] unable to get remote folder: empty result");
                return false;
            }
            ConsoleLogger.Log($"[storage_client] delete file '{filePath}'");
            return true;
        }
        #endregion


    }
}
