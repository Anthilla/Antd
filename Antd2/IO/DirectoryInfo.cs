//
// DirectoryInfo.cs
//
// Author:
//       Natalia Portillo <claunia@claunia.com>
//
// Copyright (c) 2015 © Claunia.com
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace Antd2.IO {
    [SerializableAttribute]
    [ComVisibleAttribute(true)]
    public sealed class DirectoryInfo : FileSystemInfo {
        readonly System.IO.DirectoryInfo _dirInfo;

        public DirectoryInfo(String path) {
            _dirInfo = new System.IO.DirectoryInfo(path);

            OriginalPath = path;
            FullPath = System.IO.Path.GetFullPath(path);
        }

        // For manual casting
        public DirectoryInfo(System.IO.DirectoryInfo dirInfo) {
            _dirInfo = dirInfo;
        }

        public override bool Exists {
            get {
                return _dirInfo.Exists;
            }
        }

        public override string Name {
            get { return _dirInfo.Name; }
        }

        public DirectoryInfo Parent {
            get {
                return new DirectoryInfo(_dirInfo.Parent);
            }
        }

        public DirectoryInfo Root {
            get {
                return new DirectoryInfo(_dirInfo.Root);
            }
        }

        public void Create() {
            _dirInfo.Create();
        }

        public DirectoryInfo CreateSubdirectory(string path) {
            return new DirectoryInfo(_dirInfo.CreateSubdirectory(path));
        }

        public FileInfo[] GetFiles() {
            System.IO.FileInfo[] _systemFileInfos = _dirInfo.GetFiles();
            FileInfo[] _fileInfos = new FileInfo[_systemFileInfos.Length];

            for (int i = 0; i < _fileInfos.Length; i++)
                _fileInfos[i] = new FileInfo(_systemFileInfos[i]);

            return _fileInfos;
        }

        public FileInfo[] GetFiles(string searchPattern) {
            System.IO.FileInfo[] _systemFileInfos = _dirInfo.GetFiles(searchPattern);
            FileInfo[] _fileInfos = new FileInfo[_systemFileInfos.Length];

            for (int i = 0; i < _fileInfos.Length; i++)
                _fileInfos[i] = new FileInfo(_systemFileInfos[i]);

            return _fileInfos;
        }

        public FileInfo[] GetFiles(string searchPattern, System.IO.SearchOption searchOption) {
            System.IO.FileInfo[] _systemFileInfos = _dirInfo.GetFiles(searchPattern, searchOption);
            FileInfo[] _fileInfos = new FileInfo[_systemFileInfos.Length];

            for (int i = 0; i < _fileInfos.Length; i++)
                _fileInfos[i] = new FileInfo(_systemFileInfos[i]);

            return _fileInfos;
        }


        public DirectoryInfo[] GetDirectories() {
            System.IO.DirectoryInfo[] _systemDirInfos = _dirInfo.GetDirectories();
            DirectoryInfo[] _dirInfos = new DirectoryInfo[_systemDirInfos.Length];

            for (int i = 0; i < _dirInfos.Length; i++)
                _dirInfos[i] = new DirectoryInfo(_systemDirInfos[i]);

            return _dirInfos;
        }

        public DirectoryInfo[] GetDirectories(string searchPattern) {
            System.IO.DirectoryInfo[] _systemDirInfos = _dirInfo.GetDirectories(searchPattern);
            DirectoryInfo[] _dirInfos = new DirectoryInfo[_systemDirInfos.Length];

            for (int i = 0; i < _dirInfos.Length; i++)
                _dirInfos[i] = new DirectoryInfo(_systemDirInfos[i]);

            return _dirInfos;
        }

        public DirectoryInfo[] GetDirectories(string searchPattern, System.IO.SearchOption searchOption) {
            System.IO.DirectoryInfo[] _systemDirInfos = _dirInfo.GetDirectories(searchPattern, searchOption);
            DirectoryInfo[] _dirInfos = new DirectoryInfo[_systemDirInfos.Length];

            for (int i = 0; i < _dirInfos.Length; i++)
                _dirInfos[i] = new DirectoryInfo(_systemDirInfos[i]);

            return _dirInfos;
        }

        public System.IO.FileSystemInfo[] GetFileSystemInfos() {
            return _dirInfo.GetFileSystemInfos();
        }

        public System.IO.FileSystemInfo[] GetFileSystemInfos(string searchPattern) {
            return _dirInfo.GetFileSystemInfos(searchPattern);
        }

        public
        System.IO.FileSystemInfo[] GetFileSystemInfos(string searchPattern, System.IO.SearchOption searchOption) {
            return _dirInfo.GetFileSystemInfos(searchPattern, searchOption);
        }

        public override void Delete() {
            _dirInfo.Delete();
        }

        public void Delete(bool recursive) {
            _dirInfo.Delete(recursive);
        }

        public void MoveTo(string destDirName) {
            _dirInfo.MoveTo(destDirName);
        }

        public override string ToString() {
            return _dirInfo.ToString();
        }

        public void Create(DirectorySecurity directorySecurity) {
            _dirInfo.Create();
            _dirInfo.SetAccessControl(directorySecurity);
        }

        public DirectoryInfo CreateSubdirectory(string path, DirectorySecurity directorySecurity) {
            var dirInfo = new DirectoryInfo(_dirInfo.CreateSubdirectory(path));
            dirInfo.SetAccessControl(directorySecurity);
            return dirInfo;
        }

        public DirectorySecurity GetAccessControl() {
            return _dirInfo.GetAccessControl();
        }

        public DirectorySecurity GetAccessControl(AccessControlSections includeSections) {
            return _dirInfo.GetAccessControl(includeSections);
        }

        public void SetAccessControl(DirectorySecurity directorySecurity) {
            _dirInfo.SetAccessControl(directorySecurity);
        }

        public IEnumerable<DirectoryInfo> EnumerateDirectories() {
            foreach (System.IO.DirectoryInfo _sysDirInfo in _dirInfo.EnumerateDirectories())
                yield return new DirectoryInfo(_sysDirInfo);
        }

        public IEnumerable<DirectoryInfo> EnumerateDirectories(string searchPattern) {
            foreach (System.IO.DirectoryInfo _sysDirInfo in _dirInfo.EnumerateDirectories(searchPattern))
                yield return new DirectoryInfo(_sysDirInfo);
        }

        public IEnumerable<DirectoryInfo> EnumerateDirectories(string searchPattern, System.IO.SearchOption searchOption) {
            foreach (System.IO.DirectoryInfo _sysDirInfo in _dirInfo.EnumerateDirectories(searchPattern, searchOption))
                yield return new DirectoryInfo(_sysDirInfo);
        }

        public IEnumerable<FileInfo> EnumerateFiles() {
            foreach (System.IO.FileInfo _sysFileInfo in _dirInfo.EnumerateFiles())
                yield return new FileInfo(_sysFileInfo);
        }

        public IEnumerable<FileInfo> EnumerateFiles(string searchPattern) {
            foreach (System.IO.FileInfo _sysFileInfo in _dirInfo.EnumerateFiles(searchPattern))
                yield return new FileInfo(_sysFileInfo);
        }

        public IEnumerable<FileInfo> EnumerateFiles(string searchPattern, System.IO.SearchOption searchOption) {
            foreach (System.IO.FileInfo _sysFileInfo in _dirInfo.EnumerateFiles(searchPattern, searchOption))
                yield return new FileInfo(_sysFileInfo);
        }

        public IEnumerable<System.IO.FileSystemInfo> EnumerateFileSystemInfos() {
            return _dirInfo.EnumerateFileSystemInfos();
        }

        public IEnumerable<System.IO.FileSystemInfo> EnumerateFileSystemInfos(string searchPattern) {
            return _dirInfo.EnumerateFileSystemInfos(searchPattern);
        }

        public IEnumerable<System.IO.FileSystemInfo> EnumerateFileSystemInfos(string searchPattern, System.IO.SearchOption searchOption) {
            return _dirInfo.EnumerateFileSystemInfos(searchPattern, searchOption);
        }
    }
}

