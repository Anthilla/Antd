using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace antdlib.Database {
    public class Encryption {
        private static byte[] Hash256(string inputString) {
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static byte[] Hash(string inputString) {
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        #region Core Key & Vector
        private static byte[] GetCoreKey() {
            return Hash256("3DB23244-884E-4CF9-8E10-82E6E7C20F5A");
        }

        public static byte[] CoreKey { get { return GetCoreKey(); } }

        private static byte[] GetCoreVector() {
            var coreVector = new byte[16];
            Array.Copy(Hash256("3DB23244-884E-4CF9-8E10-82E6E7C20F5A"), 0, coreVector, 0, coreVector.Length);
            return coreVector;
        }

        public static byte[] CoreVector { get { return GetCoreVector(); } }

        #endregion

        #region Random Key & Vector
        private static byte[] CreateRandomKey() {
            return Hash256(Guid.NewGuid().ToString());
        }

        public static byte[] RandomKey { get { return CreateRandomKey(); } }

        private static byte[] CreateRandomVector() {
            var coreVector = new byte[16];
            Array.Copy(Hash256(Guid.NewGuid().ToString()), 0, coreVector, 0, coreVector.Length);
            return coreVector;
        }

        public static byte[] RandomVector { get { return CreateRandomVector(); } }
        #endregion

        private static byte[] _encrypt(byte[] data, byte[] key, byte[] vector) {
            var encryptor = new RijndaelManaged().CreateEncryptor(key, vector);
            using (var memoryStream = new MemoryStream()) {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    memoryStream.Position = 0;
                    var transformedBytes = new byte[memoryStream.Length];
                    memoryStream.Read(transformedBytes, 0, transformedBytes.Length);
                    return transformedBytes;
                }
            }
        }

        private static byte[] _decrypt(byte[] data, byte[] key, byte[] vector) {
            var decryptor = new RijndaelManaged().CreateDecryptor(key, vector);
            using (var memoryStream = new MemoryStream()) {
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write)) {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.FlushFinalBlock();
                    memoryStream.Position = 0;
                    var transformedBytes = new byte[memoryStream.Length];
                    memoryStream.Read(transformedBytes, 0, transformedBytes.Length);
                    return transformedBytes;
                }
            }
        }

        public static byte[] Encrypt(object dataToEncrypt) {
            using (var ms = new MemoryStream()) {
                new BinaryFormatter().Serialize(ms, dataToEncrypt);
                return _encrypt(ms.ToArray(), CoreKey, CoreVector);
            }
        }

        public static byte[] Encrypt(object dataToEncrypt, byte[] key, byte[] vector) {
            using (var ms = new MemoryStream()) {
                new BinaryFormatter().Serialize(ms, dataToEncrypt);
                return _encrypt(ms.ToArray(), key, vector);
            }
        }

        public static object Decrypt(byte[] dataToDecrypt) {
            var decryptedObject = _decrypt(dataToDecrypt, CoreKey, CoreVector);
            using (var memStream = new MemoryStream()) {
                var binForm = new BinaryFormatter();
                memStream.Write(decryptedObject, 0, decryptedObject.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                return binForm.Deserialize(memStream);
            }
        }

        public static object Decrypt(byte[] dataToDecrypt, byte[] key, byte[] vector) {
            var decryptedObject = _decrypt(dataToDecrypt, key, vector);
            using (var memStream = new MemoryStream()) {
                var binForm = new BinaryFormatter();
                memStream.Write(decryptedObject, 0, decryptedObject.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                return binForm.Deserialize(memStream);
            }
        }

        public static byte[] XEncrypt<T>(T dataToEncrypt) {
            var js = JsonConvert.SerializeObject(dataToEncrypt);
            var array = Encoding.ASCII.GetBytes(js);
            return _encrypt(array, CoreKey, CoreVector);
        }

        public static byte[] XEncrypt<T>(T dataToEncrypt, byte[] key, byte[] vector) {
            var js = JsonConvert.SerializeObject(dataToEncrypt);
            var array = Encoding.ASCII.GetBytes(js);
            return _encrypt(array, key, vector);
        }

        public static T XDecrypt<T>(byte[] dataToDecrypt) {
            var decryptedObject = _decrypt(dataToDecrypt, CoreKey, CoreVector);
            string value = Encoding.ASCII.GetString(decryptedObject);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static T XDecrypt<T>(byte[] dataToDecrypt, byte[] key, byte[] vector) {
            var decryptedObject = _decrypt(dataToDecrypt, key, vector);
            string value = Encoding.ASCII.GetString(decryptedObject);
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}