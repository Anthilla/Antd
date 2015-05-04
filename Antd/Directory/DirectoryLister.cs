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
            try {
                files = root.GetFiles("*.*");
            }
            catch (UnauthorizedAccessException e) {
            }
            catch (DirectoryNotFoundException e) {
                Console.WriteLine(e.Message);
            }
            if (files != null) {
                foreach (FileInfo fi in files) {
                    _tree.Add(fi.FullName);
                }
                subDirs = root.GetDirectories();
                foreach (DirectoryInfo dirInfo in subDirs) {
                    _tree.Add(dirInfo.FullName);
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
