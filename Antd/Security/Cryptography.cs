using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Antd.Security {

    public class Cryptography {

        private static string LocalKey() {
            return "6602703B-17C6-4191-B554-53039A3F4636";
        }

        private static string LocalVector() {
            return "4B8022ED-C6EE-46A5-9050-36350A4A3F45";
        }

        //Core Key&Vector
        public static byte[] CoreKey() {
            byte[] hashCore = Hash256ToBytes(LocalKey());
            return hashCore;
        }

        public static byte[] CoreVector() {
            byte[] hashCore = Hash256ToBytes(LocalVector());
            byte[] coreVector = new byte[16];
            Array.Copy((Array)hashCore, 0, (Array)coreVector, 0, coreVector.Length);
            return coreVector;
        }

        #region Random

        public static byte[] CreateRandomKey() {
            string key = Guid.NewGuid().ToString();
            byte[] kkk = encryptBytes(key, CoreKey(), CoreVector());
            byte[] hashCore = Hash256ToBytes(GetString(kkk));
            return hashCore;
        }

        private static byte[] RandomKey { get { var x = CreateRandomKey(); return x; } }

        public static byte[] CreateRandomVector() {
            string vector = Guid.NewGuid().ToString();
            byte[] hashCore = Hash256ToBytes(vector);
            byte[] coreVector = new byte[16];
            Array.Copy((Array)hashCore, 0, (Array)coreVector, 0, coreVector.Length);
            return coreVector;
        }

        private static byte[] RandomVector { get { var x = CreateRandomVector(); return x; } }

        #endregion Random

        public static byte[] GenerateKey(string key) {
            byte[] hashCore = Hash256ToBytes(key);
            byte[] newArray = new byte[32];
            Array.Copy(hashCore, newArray, newArray.Length);
            return newArray;
        }

        public static byte[] GenerateVector(string vector) {
            byte[] hashCore = Hash256ToBytes(vector);
            byte[] newArray = new byte[16];
            Array.Copy(hashCore, newArray, newArray.Length);
            return newArray;
        }

        private static byte[] encryptBytes(String textValue, byte[] key, byte[] vector) {
            if (textValue != null || textValue != "") {
                RijndaelManaged crypt = new RijndaelManaged();
                ICryptoTransform encryptor = crypt.CreateEncryptor(key, vector);
                byte[] dataValueBytes = GetBytes(textValue);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                cryptoStream.Write(dataValueBytes, 0, dataValueBytes.Length);
                cryptoStream.FlushFinalBlock();
                memoryStream.Position = 0;
                byte[] transformedBytes = new byte[memoryStream.Length];
                memoryStream.Read(transformedBytes, 0, transformedBytes.Length);
                cryptoStream.Close();
                memoryStream.Close();
                return transformedBytes;
            }
            else return new byte[] { };
        }

        private static string decryptBytes(byte[] dataValue, byte[] key, byte[] vector) {
            if (dataValue != null) {
                RijndaelManaged crypt = new RijndaelManaged();
                ICryptoTransform decryptor = crypt.CreateDecryptor(key, vector);
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write);
                cryptoStream.Write(dataValue, 0, dataValue.Length);
                cryptoStream.FlushFinalBlock();
                memoryStream.Position = 0;
                byte[] transformedBytes = new byte[memoryStream.Length];
                memoryStream.Read(transformedBytes, 0, transformedBytes.Length);
                cryptoStream.Close();
                memoryStream.Close();
                string arr = GetString(transformedBytes);
                return arr;
            }
            else return String.Empty;
        }

        public static byte[] Encrypt(string value, byte[] key, byte[] vector) {
            byte[] dataToEncrypt = encryptBytes(value, key, vector);
            return dataToEncrypt;
        }

        public static string Decrypt(byte[] value, byte[] key, byte[] vector) {
            string dataToDecrypt = decryptBytes(value, key, vector);
            return dataToDecrypt;
        }

        public static byte[] Hash256ToBytes(string inputString) {
            HashAlgorithm algorithm = SHA256.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string Hash256ToString(string inputString) {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in Hash256ToBytes(inputString))
                sb.Append(b.ToString("X2"));
            return sb.ToString();
        }

        #region Could be extensions
        private static string ToHex(string value) {
            char[] chars = value.ToCharArray();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in chars) {
                stringBuilder.Append(((Int16)c).ToString(""));
            }
            string hexed = stringBuilder.ToString();
            return hexed;
        }

        public static string ToHex(byte[] bytes) {
            string value = GetString(bytes);
            char[] chars = value.ToCharArray();
            StringBuilder stringBuilder = new StringBuilder();
            foreach (char c in chars) {
                stringBuilder.Append(((Int16)c).ToString(""));
            }
            string hexed = stringBuilder.ToString();
            return hexed;
        }

        private static byte[] GetBytes(string str) {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private static string GetString(byte[] bytes) {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
        #endregion Could be extensions
    }
}