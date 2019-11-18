using System.IO;

namespace antd.core {
    public class CommonDirectory {
        public static void Create(string fullpath) {
            if(!System.IO.Directory.Exists(fullpath)) {
                System.IO.Directory.CreateDirectory(fullpath);
            }
        }

        public static void Copy(string sourcePath, string destinationPath) {
            foreach(string dirPath in System.IO.Directory.EnumerateDirectories(sourcePath, "*", SearchOption.AllDirectories))
                System.IO.Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
            foreach(string newPath in System.IO.Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, destinationPath), true);
        }

        public static string GetAbsolutePath(string relativePath, string basePath) {
            if(relativePath == null)
                return null;
            if(basePath == null)
                basePath = Path.GetFullPath("."); // quick way of getting current working directory
            else
                basePath = GetAbsolutePath(basePath, null); // to be REALLY sure ;)
            string path;
            // specific for windows paths starting on \ - they need the drive added to them.
            // I constructed this piece like this for possible Mono support.
            if(!Path.IsPathRooted(relativePath) || "\\".Equals(Path.GetPathRoot(relativePath))) {
                if(relativePath.StartsWith(Path.DirectorySeparatorChar.ToString()))
                    path = Path.Combine(Path.GetPathRoot(basePath), relativePath.TrimStart(Path.DirectorySeparatorChar));
                else
                    path = Path.Combine(basePath, relativePath);
            }
            else
                path = relativePath;
            // resolves any internal "..\" to get the true full path.
            return Path.GetFullPath(path);
        }
    }
}
