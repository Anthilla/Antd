using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;

namespace Antd {
    public class DirectoryLister {

        private DirectoryInfo root;

        public DirectoryLister(string _path) {
            root = new DirectoryInfo(_path);
            WalkDirectoryTree(root);
        }

        private HashSet<string> _cache = new HashSet<string>() { };

        private void WalkDirectoryTree(System.IO.DirectoryInfo root) {
            System.IO.FileInfo[] files = null;
            System.IO.DirectoryInfo[] subDirs = null;
            _cache.Add(root.FullName);
            // First, process all the files directly under this folder 
            try {
                files = root.GetFiles("*.*");
            }
            // This is thrown if even one of the files requires permissions greater 
            // than the application provides. 
            catch (UnauthorizedAccessException e) {
                // This code just writes out the message and continues to recurse. 
                // You may decide to do something different here. For example, you 
                // can try to elevate your privileges and access the file again.
            }
            catch (System.IO.DirectoryNotFoundException e) {
                Console.WriteLine(e.Message);
            }
            if (files != null) {
                foreach (System.IO.FileInfo fi in files) {
                    // In this example, we only access the existing FileInfo object. If we 
                    // want to open, delete or modify the file, then 
                    // a try-catch block is required here to handle the case 
                    // where the file has been deleted since the call to TraverseTree().
                    _cache.Add(fi.Attributes + " | " + fi.IsReadOnly + " | " + fi.FullName);
                    //Console.WriteLine(fi.FullName);
                }
                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();
                foreach (System.IO.DirectoryInfo dirInfo in subDirs) {
                    // Resursive call for each subdirectory.
                    _cache.Add(dirInfo.Attributes + " | " + dirInfo.FullName);
                    //Console.WriteLine(dirInfo.FullName);
                    WalkDirectoryTree(dirInfo);
                }
            }
        }

        public HashSet<string> FullList {
            get {
                return _cache;
            }
        }

        public DirectorySecurity GetFileACL() {
            var acc = root.GetAccessControl();
            return acc;
        }
    }
}
