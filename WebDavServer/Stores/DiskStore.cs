using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using NWebDav.Server.Helpers;
using NWebDav.Server.Http;
using NWebDav.Server.Locking;

namespace NWebDav.Server.Stores {
    public sealed class DiskStore : IStore {
        public DiskStore(string directory, bool isWritable = true, ILockingManager lockingManager = null) {
            Console.WriteLine($"[webdav] new DiskStore {directory}");
            if(string.IsNullOrEmpty(directory)) {
                throw new ArgumentNullException(nameof(directory));
            }

            IsWritable = isWritable;
            BaseDirectory = directory;
            LockingManager = lockingManager ?? new InMemoryLockingManager();
        }

        public bool IsWritable { get; }
        public string BaseDirectory { get; }
        public ILockingManager LockingManager { get; }

        public Task<IStoreItem> GetItemAsync(Uri uri, IHttpContext httpContext) {
            Console.WriteLine($"[webdav] GetItemAsync: {uri}");
            // Determine the path from the uri
            var path = GetPathFromUri(uri);

            // Check if it's a directory
            if(Directory.Exists(path))
                return Task.FromResult<IStoreItem>(new DiskStoreCollection(LockingManager, new DirectoryInfo(path), IsWritable));

            // Check if it's a file
            if(File.Exists(path))
                return Task.FromResult<IStoreItem>(new DiskStoreItem(LockingManager, new FileInfo(path), IsWritable));

            // The item doesn't exist
            return Task.FromResult<IStoreItem>(null);
        }

        public Task<IStoreCollection> GetCollectionAsync(Uri uri, IHttpContext httpContext) {
            Console.WriteLine($"[webdav] GetCollectionAsync: {uri}");
            // Determine the path from the uri
            var path = GetPathFromUri(uri);
            if(!Directory.Exists(path))
                return Task.FromResult<IStoreCollection>(null);

            // Return the item
            return Task.FromResult<IStoreCollection>(new DiskStoreCollection(LockingManager, new DirectoryInfo(path), IsWritable));
        }

        private string GetPathFromUri(Uri uri) {
            // Determine the path
            //var requestedPath = UriHelper.GetDecodedPath(uri).Substring(1).Replace('/', Path.DirectorySeparatorChar);
            var requestedPath = UriHelper.GetDecodedPath(uri).Substring(1);
            Console.WriteLine($"[webdav] requestedPath: {requestedPath}");

            // Determine the full path
            //var fullPath = Path.GetFullPath(Path.Combine(BaseDirectory, requestedPath));
            var fullPath = BaseDirectory + "/" + requestedPath;

            // Make sure we're still inside the specified directory
            //if(fullPath != BaseDirectory && !fullPath.StartsWith(BaseDirectory + Path.DirectorySeparatorChar))
            if(fullPath != BaseDirectory && !fullPath.StartsWith(BaseDirectory))
                throw new SecurityException($"Uri '{uri}' is outside the '{BaseDirectory}' directory.");

            // Return the combined path
            Console.WriteLine($"[webdav] GetPathFromUri: {uri} -> {fullPath}");
            return fullPath;
        }


    }
}
