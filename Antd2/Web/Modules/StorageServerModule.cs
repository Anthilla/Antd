using Antd.cmds;
using Antd.models;
using Nancy;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System;
using anthilla.core;

namespace Antd2.Modules {
    /// <summary>
    /// Questo modulo sarà il punto di arrivo delle chiamate GET, POST, DELETE, PUT (?)
    /// dove passeranno le informazioni, in entrata e in uscita, verso e da altri nodi del cluster
    /// Quindi la classe VfsClient verrà comunque utilizzata da antd (server) (per esempio nel sync dei servizi)
    /// per ottenere informazioni o apportare modifiche su un file system remoto ( -> su un nodo gestito da antd)
    /// </summary>
    public class StorageServerModule : NancyModule {

        #region [    Header Keys    ]
        private const string HeaderMachineUidKey = "A_MACHINE_UID";
        private const string HeaderFolderPathKey = "A_FOLDER_PATH";
        private const string HeaderFolderNewPathKey = "A_FOLDER_NEWPATH";
        private const string HeaderFolderNewNameKey = "A_FOLDER_NEWNAME";
        private const string HeaderFilePathKey = "A_FILE_PATH";
        private const string HeaderFileNewPathKey = "A_FILE_NEWPATH";
        private const string HeaderFileNewNameKey = "A_FILE_NEWNAME";
        #endregion

        private string GetValueFromHeader(RequestHeaders headers, string key) {
            var headerKVP = headers.FirstOrDefault(_ => CommonString.AreEquals(_.Key, key));
            return headerKVP.Value.FirstOrDefault();
        }

        public StorageServerModule() : base("/storageserver") {

            #region [    Folder Actions    ]
            Get["/folder/get"] = x => {
                string sourceNode = GetValueFromHeader(Request.Headers, HeaderMachineUidKey);
                string folderPath = GetValueFromHeader(Request.Headers, HeaderFolderPathKey);
                if(!Directory.Exists(folderPath)) {
                    return HttpStatusCode.InternalServerError;
                }
                var folderInfo = new DirectoryInfo(folderPath);
                var requestedFolder = new StorageServerFolder() {
                    FolderPath = folderPath,
                    CreationTime = folderInfo.CreationTime,
                    LastAccessTime = folderInfo.LastAccessTime,
                    LastWriteTime = folderInfo.LastWriteTime
                };
                var folders = Directory.EnumerateDirectories(folderPath, "*", SearchOption.TopDirectoryOnly).ToArray();
                var contentFolders = new StorageServerFolder[folders.Length];
                for(var i = 0; i < folders.Length; i++) {
                    var subFolderInfo = new DirectoryInfo(folders[i]);
                    contentFolders[i] = new StorageServerFolder() {
                        FolderPath = folders[i],
                        CreationTime = subFolderInfo.CreationTime,
                        LastAccessTime = subFolderInfo.LastAccessTime,
                        LastWriteTime = subFolderInfo.LastWriteTime
                    };
                }
                requestedFolder.Folders = contentFolders;
                var files = Directory.EnumerateFiles(folderPath, "*", SearchOption.TopDirectoryOnly).ToArray();
                var contentFiles = new StorageServerFileMetadata[files.Length];
                for(var i = 0; i < files.Length; i++) {
                    var fileInfo = new FileInfo(files[i]);
                    contentFiles[i] = new StorageServerFileMetadata() {
                        FilePath = files[i],
                        CreationTime = fileInfo.CreationTime,
                        LastAccessTime = fileInfo.LastAccessTime,
                        LastWriteTime = fileInfo.LastWriteTime,
                        Size = fileInfo.Length
                    };
                }
                requestedFolder.Files = contentFiles;
                return JsonConvert.SerializeObject(requestedFolder);
            };

            Get["/folder/verify"] = x => {
                string sourceNode = GetValueFromHeader(Request.Headers, HeaderMachineUidKey);
                string folderPath = GetValueFromHeader(Request.Headers, HeaderFolderPathKey);
                if(!Directory.Exists(folderPath)) {
                    return HttpStatusCode.InternalServerError;
                }
                return HttpStatusCode.OK;
            };

            Post["/folder/create"] = x => {
                string sourceNode = GetValueFromHeader(Request.Headers, HeaderMachineUidKey);
                string folderPath = GetValueFromHeader(Request.Headers, HeaderFolderPathKey);
                if(Directory.Exists(folderPath)) {
                    return HttpStatusCode.InternalServerError;
                }
                Directory.CreateDirectory(folderPath);
                return HttpStatusCode.OK;
            };

            Put["/folder/move"] = x => {
                string sourceNode = GetValueFromHeader(Request.Headers, HeaderMachineUidKey);
                string folderPath = GetValueFromHeader(Request.Headers, HeaderFolderPathKey);
                string newPath = GetValueFromHeader(Request.Headers, HeaderFolderNewPathKey);
                if(!Directory.Exists(folderPath)) {
                    return HttpStatusCode.InternalServerError;
                }
                var folderName = folderPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault().Trim('/');
                var newFolderPath = CommonString.Append(newPath, "/", folderName);
                Directory.CreateDirectory(newPath);
                Directory.Move(folderPath, newFolderPath);
                return HttpStatusCode.OK;
            };

            Put["/folder/rename"] = x => {
                string sourceNode = GetValueFromHeader(Request.Headers, HeaderMachineUidKey);
                string folderPath = GetValueFromHeader(Request.Headers, HeaderFolderPathKey);
                string newName = GetValueFromHeader(Request.Headers, HeaderFolderNewNameKey);
                if(!Directory.Exists(folderPath)) {
                    return HttpStatusCode.InternalServerError;
                }
                var folderName = folderPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault().Trim('/');
                var newFolderPath = folderPath.Replace(folderName, newName);
                Directory.Move(folderPath, newFolderPath);
                return HttpStatusCode.OK;
            };

            Delete["/folder/delete"] = x => {
                string sourceNode = GetValueFromHeader(Request.Headers, HeaderMachineUidKey);
                string folderPath = GetValueFromHeader(Request.Headers, HeaderFolderPathKey);
                if(!Directory.Exists(folderPath)) {
                    return HttpStatusCode.InternalServerError;
                }
                Directory.Delete(folderPath, true);
                return HttpStatusCode.OK;
            };
            #endregion

            #region [    File Actions    ]
            Get["/file/get"] = x => {
                string sourceNode = GetValueFromHeader(Request.Headers, HeaderMachineUidKey);
                string filePath = GetValueFromHeader(Request.Headers, HeaderFolderPathKey);
                if(!File.Exists(filePath)) {
                    return HttpStatusCode.InternalServerError;
                }
                var fileName = Path.GetFileName(filePath);
                var response = new Response();
                response.Headers.Add("Content-Disposition", "attachment; filename=" + Nancy.Helpers.HttpUtility.UrlEncode(fileName, System.Text.Encoding.ASCII));
                response.ContentType = "application/zip";
                response.Contents = stream => {
                    using(var fileStream = File.OpenRead(filePath)) {
                        using(var memoryStream = new MemoryStream()) {
                            fileStream.CopyTo(stream);
                            int data;
                            while((data = memoryStream.ReadByte()) != -1) {
                                memoryStream.WriteByte((byte)data);
                            }
                        }
                    }
                };
                return response;
            };

            Get["/file/verify"] = x => {
                string sourceNode = GetValueFromHeader(Request.Headers, HeaderMachineUidKey);
                string filePath = GetValueFromHeader(Request.Headers, HeaderFolderPathKey);
                if(!File.Exists(filePath)) {
                    return HttpStatusCode.InternalServerError;
                }
                return HttpStatusCode.OK;
            };

            Post["/file/create"] = x => {
                string sourceNode = GetValueFromHeader(Request.Headers, HeaderMachineUidKey);
                string filePath = GetValueFromHeader(Request.Headers, HeaderFolderPathKey);
                if(File.Exists(filePath)) {
                    return HttpStatusCode.InternalServerError;
                }
                var files = Request.Files.ToArray();
                if(!files.Any()) {
                    return HttpStatusCode.BadRequest;
                }
                var file = files.FirstOrDefault();
                //todo usa rsync
                using(var fileStream = new FileStream(filePath, FileMode.Create)) {
                    file.Value.CopyTo(fileStream);
                }
                return HttpStatusCode.OK;
            };

            Put["/file/move"] = x => {
                string sourceNode = GetValueFromHeader(Request.Headers, HeaderMachineUidKey);
                string filePath = GetValueFromHeader(Request.Headers, HeaderFolderPathKey);
                string newPath = GetValueFromHeader(Request.Headers, HeaderFolderNewPathKey);
                if(!File.Exists(filePath)) {
                    return HttpStatusCode.InternalServerError;
                }
                var fileName = filePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault().Trim('/');
                var newFolderPath = CommonString.Append(newPath, "/", fileName);
                if(File.Exists(newFolderPath)) {
                    File.Delete(newFolderPath);
                }
                File.Move(filePath, newFolderPath);
                return HttpStatusCode.OK;
            };

            Put["/file/rename"] = x => {
                string sourceNode = GetValueFromHeader(Request.Headers, HeaderMachineUidKey);
                string filePath = GetValueFromHeader(Request.Headers, HeaderFolderPathKey);
                string newName = GetValueFromHeader(Request.Headers, HeaderFolderNewNameKey);
                if(!File.Exists(filePath)) {
                    return HttpStatusCode.InternalServerError;
                }
                var fileName = filePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault().Trim('/');
                var newFolderPath = filePath.Replace(fileName, newName);
                if(File.Exists(newFolderPath)) {
                    File.Delete(newFolderPath);
                }
                File.Move(filePath, newFolderPath);
                return HttpStatusCode.OK;
            };

            Delete["/file/delete"] = x => {
                string sourceNode = GetValueFromHeader(Request.Headers, HeaderMachineUidKey);
                string filePath = GetValueFromHeader(Request.Headers, HeaderFolderPathKey);
                if(!File.Exists(filePath)) {
                    return HttpStatusCode.InternalServerError;
                }
                File.Delete(filePath);
                return HttpStatusCode.OK;
            };
            #endregion
        }
    }
}