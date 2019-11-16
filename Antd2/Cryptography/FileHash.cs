using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Antd2.Cryptography {
    public static class FileHash {

        public static string GetSHA256(string file, int bufferSize = 1200000) {
            using (var stream = new BufferedStream(File.OpenRead(file), bufferSize)) {
                var sha = new SHA256Managed();
                byte[] checksum = sha.ComputeHash(stream);
                return BitConverter.ToString(checksum).Replace("-", String.Empty).ToLower();
            }
        }
    }
}
