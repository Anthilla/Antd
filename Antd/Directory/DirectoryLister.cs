///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using Antd.Models;
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

        #region parents

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

        #endregion parents

        #region tree

        private HashSet<string> _tree = new HashSet<string>() { };

        private HashSet<DirItemModel> _tree2 = new HashSet<DirItemModel>() { };

        private void WalkDirectoryTree(DirectoryInfo root, bool getSubDir) {
            FileInfo[] files = null;
            DirectoryInfo[] subDirs = null;
            try {
                files = root.GetFiles("*.*");
            }
            catch (UnauthorizedAccessException e) {
                Console.WriteLine(e.Message);
            }
            catch (DirectoryNotFoundException e) {
                Console.WriteLine(e.Message);
            }
            if (files != null) {
                foreach (FileInfo fi in files) {
                    _tree2.Add(new DirItemModel() {
                        isFile = true,
                        path = fi.FullName,
                        name = Path.GetFileName(fi.FullName)
                    });
                    _tree.Add(fi.FullName);
                }
                subDirs = root.GetDirectories();
                foreach (DirectoryInfo dirInfo in subDirs) {
                    _tree2.Add(new DirItemModel() {
                        isFile = false,
                        path = dirInfo.FullName,
                        name = Path.GetDirectoryName(dirInfo.FullName)
                    });
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

        public HashSet<DirItemModel> FullList2 {
            get {
                return _tree2;
            }
        }

        #endregion tree

        public DirectorySecurity GetFileACL() {
            var acc = root.GetAccessControl();
            return acc;
        }
    }
}