using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;

namespace Antd {
    public class DirectoryLister {

        private DirectoryInfo root;

        public DirectoryLister(string _path, bool getSubDir) {
            root = new DirectoryInfo(_path);
            WalkDirectoryTree(root, getSubDir);
            UpwalkDirectoryTree(root);
        }

        private HashSet<string> _parents = new HashSet<string>() { };

        private void UpwalkDirectoryTree(DirectoryInfo root) {
            try {
                _parents.Add(root.FullName);
                DirectoryInfo parent = Directory.GetParent(root.FullName);
                UpwalkDirectoryTree(parent);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public HashSet<string> ParentList {
            get {
                return _parents;
            }
        }

        private HashSet<string> _tree = new HashSet<string>() { };

        private void WalkDirectoryTree(DirectoryInfo root, bool getSubDir) {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;
            //_tree.Add(root.FullName);
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
            catch (DirectoryNotFoundException e) {
                Console.WriteLine(e.Message);
            }
            if (files != null) {
                foreach (FileInfo fi in files) {
                    // In this example, we only access the existing FileInfo object. If we 
                    // want to open, delete or modify the file, then 
                    // a try-catch block is required here to handle the case 
                    // where the file has been deleted since the call to TraverseTree().
                    _tree.Add(fi.FullName);
                    //Console.WriteLine(fi.FullName);
                }
                // Now find all the subdirectories under this directory.
                subDirs = root.GetDirectories();
                foreach (DirectoryInfo dirInfo in subDirs) {
                    // Resursive call for each subdirectory.
                    _tree.Add(dirInfo.FullName);
                    //Console.WriteLine(dirInfo.FullName);
                    if (getSubDir == true) {
                        WalkDirectoryTree(dirInfo, true);
                    }
                }
            }
        }

        public HashSet<string> FullList {
            get {
                return _tree;
            }
        }

        public DirectorySecurity GetFileACL() {
            var acc = root.GetAccessControl();
            return acc;
        }
    }
}
