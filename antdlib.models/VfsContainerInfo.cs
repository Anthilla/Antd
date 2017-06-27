using System.Collections.Generic;

namespace antdlib.models {
    public class ObjectMetadata {
        public string Key { get; set; }
        public int Size { get; set; }
        public string Created { get; set; }
        public string LastUpdate { get; set; }
        public string LastAccess { get; set; }
    }

    public class VfsContainerInfo {
        public string Url { get; set; }
        public string UserGuid { get; set; }
        public string ContainerName { get; set; }
        public int Size { get; set; }
        public int NumObjects { get; set; }
        public string Created { get; set; }
        public string LastUpdate { get; set; }
        public string LastAccess { get; set; }
        public List<object> ContainerPath { get; set; }
        public List<string> ChildContainers { get; set; }
        public List<ObjectMetadata> ObjectMetadata { get; set; }
    }
}
