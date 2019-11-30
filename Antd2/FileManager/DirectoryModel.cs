using System;

namespace Antd2.FileManager {
    public class DirectoryModel {
        public string Path { get; set; } = string.Empty;
        public string Parent { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }

        public DirectoryModel[] Folders { get; set; } = new DirectoryModel[0];
        public FileModel[] Files { get; set; } = new FileModel[0];
    }
}

