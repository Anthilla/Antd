using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace antdlib.common {
    public class Encryption {
        #region Hashing methods
        public static byte[] Hash(string inputString) {
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string XHash(string inputString) {
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString)).ToHex();
        }
        #endregion

        #region Core Key & Vector
        private const string LocalKey = "6602703B-17C6-4191-B554-53039A3F4636";
        private const string LocalVector = "4B8022ED-C6EE-46A5-9050-36350A4A3F45";

        private static byte[] GetCoreKey() {
            return Hash(LocalKey);
        }

        public static byte[] CoreKey => GetCoreKey();

        private static byte[] GetCoreVector() {
            var coreVector = new byte[16];
            Array.Copy(Hash(LocalVector), 0, coreVector, 0, coreVector.Length);
            return coreVector;
        }

        public static byte[] CoreVector => GetCoreVector();
        #endregion

        #region Random Key & Vector
        private static byte[] CreateRandomKey() {
            return Hash(Guid.NewGuid().ToString());
        }

        public static byte[] RandomKey => CreateRandomKey();

        private static byte[] CreateRandomVector() {
            var coreVector = new byte[16];
            Array.Copy(Hash(Guid.NewGuid().ToString()), 0, coreVector, 0, coreVector.Length);
            return coreVector;
        }

        public static byte[] RandomVector => CreateRandomVector();
        #endregion

        #region Core methods
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

        #endregion

        #region Encryption and Decryption Methods
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
            var value = Encoding.ASCII.GetString(decryptedObject);
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static T XDecrypt<T>(byte[] dataToDecrypt, byte[] key, byte[] vector) {
            var decryptedObject = _decrypt(dataToDecrypt, key, vector);
            var value = Encoding.ASCII.GetString(decryptedObject);
            return JsonConvert.DeserializeObject<T>(value);
        }
        #endregion

        #region Database Encryption
        public static byte[] DbEncrypt<T>(T dataToEncrypt, byte[] key, byte[] vector) {
            var ms = new MemoryStream();
            using (var writer = new BsonWriter(ms)) {
                var serializer = new JsonSerializer();
                serializer.Serialize(writer, dataToEncrypt);
            }
            var array = ms.ToArray();
            return _encrypt(array, key, vector);
        }

        public static T DbDecrypt<T>(byte[] dataToDecrypt, byte[] key, byte[] vector) {
            var decryptedObject = _decrypt(dataToDecrypt, key, vector);
            var ms = new MemoryStream(decryptedObject);
            using (var reader = new BsonReader(ms)) {
                var serializer = new JsonSerializer();
                return serializer.Deserialize<T>(reader);
            }
        }
        #endregion

        public static string Hash256ToString(string inputString) {
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString)).ToHex();
        }
    }
}