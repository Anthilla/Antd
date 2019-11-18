using System;
using System.IO;
using System.Security.Cryptography;

namespace antd.core {
    public class CommonFile {
        public static string GetHash(string fullpath) {
            if(string.IsNullOrEmpty(fullpath)) {
                return string.Empty;
            }
            if(!File.Exists(fullpath)) {
                return string.Empty;
            }
            using(var fileStreamToRead = File.OpenRead(fullpath)) {
                return BitConverter.ToString(new SHA1Managed().ComputeHash(fileStreamToRead)).Replace("-", string.Empty);
            }
        }
    }
}
