using Antd2.cmds;
using Antd2.FileManager;
using Nancy;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace Antd2.Modules {
    public class FileManagerModule : NancyModule {

        public FileManagerModule() : base("/fm") {

            Get("/{path*}", x => ApiGet(x));

            Post("/folder/create", x => ApiPostFolderCreate());

            Post("/folder/move", x => ApiPostFolderMove());

            Post("/folder/delete", x => ApiPostFolderDelete());

            Post("/folder/sync", x => ApiPostFolderSync());

            Post("/file/rename", x => ApiPostFileRename());

            Post("/file/move", x => ApiPostFileMove());

            Post("/file/delete", x => ApiPostFileDelete());

            Post("/file/sync", x => ApiPostFileSync());

            Get("/file/download/{path*}", x => ApiPostFileDownload(x));

            Post("/file/upload", x => ApiPostFileUpload());
        }

        private dynamic ApiGet(dynamic x) {
            string path = x.path;
            var a = FileManagerRepository.GetFolder(path);
            var jsonString = JsonConvert.SerializeObject(a);
            var jsonBytes = Encoding.UTF8.GetBytes(jsonString);
            return new Response {
                ContentType = "application/json",
                Contents = s => s.Write(jsonBytes, 0, jsonBytes.Length)
            };
        }

        private dynamic ApiPostFolderCreate() {
            string parent = Request.Form.Parent;
            string folder = Request.Form.Folder;
            FileManagerRepository.NewFolder(parent, folder);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostFolderMove() {
            string path = Request.Form.Path;
            string folderName = Request.Form.Folder;
            FileManagerRepository.MoveFolder(path, folderName);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostFolderDelete() {
            string path = Request.Form.Path;
            FileManagerRepository.DeleteFolder(path, false);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostFolderSync() {
            string src = Request.Form.Source;
            string dst = Request.Form.Destination;
            src += "/";
            dst += "/";
            Console.WriteLine($"[sync] {src} > {dst}");
            Rsync.SyncArchive(src, dst);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostFileRename() {
            string path = Request.Form.Path;
            string newName = Request.Form.Name;
            FileManagerRepository.RenameFile(path, newName);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostFileMove() {
            string path = Request.Form.Path;
            string fileName = Request.Form.Folder;
            FileManagerRepository.MoveFile(path, fileName);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostFileDelete() {
            string path = Request.Form.Path;
            FileManagerRepository.DeleteFile(path);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostFileSync() {
            string src = Request.Form.Source;
            string dst = Request.Form.Destination;
            dst = Path.Combine(dst, Path.GetFileName(src));
            Console.WriteLine($"[sync] {src} > {dst}");
            Rsync.SyncArchive(src, dst);
            return HttpStatusCode.OK;
        }

        private dynamic ApiPostFileDownload(dynamic x) {
            string path = x.path;
            throw new System.NotImplementedException();
        }

        private dynamic ApiPostFileUpload() {
            throw new System.NotImplementedException();
        }
    }
}