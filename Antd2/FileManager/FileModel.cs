using System;

namespace Antd2.FileManager {
    public class FileModel {

        public string Path { get; set; } = string.Empty;
        public string Parent { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public long Dimension { get; set; }
        public string Extension { get; set; } = string.Empty;

        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
    }
}
