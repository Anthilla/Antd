//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using IoDir = System.IO.Directory;

namespace antdlib.common.Directory {

    public class DirectoryLister {
        private readonly DirectoryInfo _root;

        public DirectoryLister(string path, bool getSubDir) {
            _root = new DirectoryInfo(path);
            WalkDirectoryTree(_root, getSubDir);
            UpwalkDirectoryTree(_root);
        }

        public HashSet<string> ParentList { get; } = new HashSet<string>();

        private void UpwalkDirectoryTree(FileSystemInfo root) {
            try {
                ParentList.Add(root.FullName);
                var parent = IoDir.GetParent(root.FullName);
                UpwalkDirectoryTree(parent);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }

        public HashSet<string> FullList { get; } = new HashSet<string>();

        public HashSet<DirItemModel> FullList2 { get; } = new HashSet<DirItemModel>();

        private void WalkDirectoryTree(DirectoryInfo root, bool getSubDir) {
            FileInfo[] files = null;
            try {
                files = root.GetFiles("*.*");
            }
            catch (UnauthorizedAccessException e) {
                Console.WriteLine(e.Message);
            }
            catch (DirectoryNotFoundException e) {
                Console.WriteLine(e.Message);
            }
            if (files == null)
                return;
            foreach (var fi in files) {
                FullList2.Add(new DirItemModel {
                    IsFile = true,
                    Path = fi.FullName,
                    Name = Path.GetFileName(fi.FullName)
                });
                FullList.Add(fi.FullName);
            }
            var subDirs = root.GetDirectories();
            foreach (var dirInfo in subDirs) {
                FullList2.Add(new DirItemModel {
                    IsFile = false,
                    Path = dirInfo.FullName,
                    Name = Path.GetDirectoryName(dirInfo.FullName)
                });
                FullList.Add(dirInfo.FullName);
                if (getSubDir) {
                    WalkDirectoryTree(dirInfo, true);
                }
            }
        }
    }
}