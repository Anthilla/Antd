using System;
using System.IO;
using System.Linq;

namespace Antd2.FileManager {
    public class FileManagerRepository {

        public static DirectoryModel GetFolder(string directoryPath) {
            var directories = Directory.EnumerateDirectories(directoryPath).ToArray();
            var files = Directory.EnumerateFiles(directoryPath).ToArray();
            var currentFolder = new DirectoryModel() {
                Parent = GetParent(directoryPath),
                Path = GetPath(directoryPath),
                Name = Path.GetFileName(directoryPath),
                Folders = directories.Select(_ => GetDirInfo(_))
                    .Where(_ => !_.Name.StartsWith("."))
                    .Where(_ => _.Name != "lost+found")
                    .ToArray(),
                Files = files.Select(_ => GetFileInfo(_))
                    .Where(_ => !_.Name.StartsWith("."))
                    .ToArray()
            };
            currentFolder.Created = Directory.GetCreationTime(currentFolder.Path);
            currentFolder.LastModified = Directory.GetLastWriteTime(currentFolder.Path);
            Console.WriteLine($"[fm] dir {currentFolder.Path} - {currentFolder.Folders.Length}d {currentFolder.Files.Length}f ({currentFolder.Parent})");
            Console.WriteLine($"[fm] dir {currentFolder.Path} - {currentFolder.Created} {currentFolder.LastModified}");
            return currentFolder;
        }

        private static DirectoryModel GetDirInfo(string dirPath) {
            var di = new DirectoryInfo(dirPath);
            return new DirectoryModel() {
                Parent = GetParent(dirPath),
                Path = GetPath(dirPath),
                Name = Path.GetFileName(dirPath),
                Created = di.CreationTime,
                LastModified = di.LastWriteTime
            };
        }

        private static string GetPath(string path) {
            if (!path.StartsWith("/")) {
                path = "/" + path;
            }
            return path;
        }

        private static string GetParent(string path) {
            var parent = Path.GetDirectoryName(path);
            if (!parent.StartsWith("/")) {
                parent = "/" + parent;
            }
            return parent;
        }

        private static FileModel GetFileInfo(string filePath) {
            var fi = new FileInfo(filePath);
            return new FileModel() {
                Path = filePath,
                Parent = fi.DirectoryName,
                Name = fi.Name,
                Extension = fi.Extension,
                Dimension = fi.Length,
                Created = fi.CreationTime,
                LastModified = fi.LastWriteTime
            };
        }

        public static void NewFolder(string directoryPath, string folderName) {
            var path = Path.Combine(directoryPath, folderName);
            Directory.CreateDirectory(path);
        }

        public static void MoveFolder(string directoryPath, string newFolderName) {
            var newPath = Path.Combine(Path.GetDirectoryName(directoryPath), newFolderName);
            Directory.Move(directoryPath, newFolderName);
        }

        public static void DeleteFolder(string directoryPath, bool force = false) {
            Directory.Delete(directoryPath, force);
        }

        public static void RenameFile(string filePath, string newName) {
            File.Move(filePath, Path.Combine(Path.GetDirectoryName(filePath), newName));
        }

        public static void MoveFile(string filePath, string newFolder) {
            File.Move(filePath, Path.Combine(newFolder, Path.GetFileName(filePath)));
        }

        public static void DeleteFile(string filePath) {
            File.Delete(filePath);
        }
    }
}
