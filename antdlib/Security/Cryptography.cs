//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using antdlib.Common;

namespace antdlib.Security {

    public class Cryptography {

        private static string LocalKey() {
            return "6602703B-17C6-4191-B554-53039A3F4636";
        }

        private static string LocalVector() {
            return "4B8022ED-C6EE-46A5-9050-36350A4A3F45";
        }

        //Core Key&Vector
        public static byte[] CoreKey() {
            return Hash256(LocalKey());
        }

        public static byte[] CoreVector() {
            var coreVector = new byte[16];
            Array.Copy(Hash256(LocalVector()), 0, coreVector, 0, coreVector.Length);
            return coreVector;
        }

        #region Random

        public static byte[] CreateRandomKey() {
            var hashCore = Hash256(EncryptBytes(Guid.NewGuid().ToString(), CoreKey(), CoreVector()).GetString());
            return hashCore;
        }

        private static byte[] RandomKey { get { var x = CreateRandomKey(); return x; } }

        public static byte[] CreateRandomVector() {
            var coreVector = new byte[16];
            Array.Copy(Hash256(Guid.NewGuid().ToString()), 0, coreVector, 0, coreVector.Length);
            return coreVector;
        }

        private static byte[] RandomVector { get { var x = CreateRandomVector(); return x; } }

        #endregion Random

        public static byte[] GenerateKey(string key) {
            var hashCore = Hash256(key);
            var newArray = new byte[32];
            Array.Copy(hashCore, newArray, newArray.Length);
            return newArray;
        }

        public static byte[] GenerateVector(string vector) {
            var hashCore = Hash256(vector);
            var newArray = new byte[16];
            Array.Copy(hashCore, newArray, newArray.Length);
            return newArray;
        }

        private static byte[] EncryptBytes(string textValue, byte[] key, byte[] vector) {
            var crypt = new RijndaelManaged();
            var encryptor = crypt.CreateEncryptor(key, vector);
            var dataValueBytes = textValue.GetBytes();
            using (var memoryStream = new MemoryStream()) {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write)) {
                    cryptoStream.Write(dataValueBytes, 0, dataValueBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    memoryStream.Position = 0;
                    var transformedBytes = new byte[memoryStream.Length];
                    memoryStream.Read(transformedBytes, 0, transformedBytes.Length);
                    return transformedBytes;
                }
            }
        }

        private static string DecryptBytes(byte[] dataValue, byte[] key, byte[] vector) {
            if (dataValue == null)
                return string.Empty;
            var crypt = new RijndaelManaged();
            var decryptor = crypt.CreateDecryptor(key, vector);
            using (var memoryStream = new MemoryStream()) {
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Write)) {
                    cryptoStream.Write(dataValue, 0, dataValue.Length);
                    cryptoStream.FlushFinalBlock();
                    memoryStream.Position = 0;
                    var transformedBytes = new byte[memoryStream.Length];
                    memoryStream.Read(transformedBytes, 0, transformedBytes.Length);
                    var arr = transformedBytes.GetString();
                    return arr;
                }
            }
        }

        public static byte[] Encrypt(string value, byte[] key, byte[] vector) {
            var dataToEncrypt = EncryptBytes(value, key, vector);
            return dataToEncrypt;
        }

        public static string Decrypt(byte[] value, byte[] key, byte[] vector) {
            var dataToDecrypt = DecryptBytes(value, key, vector);
            return dataToDecrypt;
        }

        public static byte[] Hash256(string inputString) {
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString));
        }

        public static string Hash256ToString(string inputString) {
            return SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString)).ToHex();
        }

        public static string Hash256Terminal(string inputString, string salt) {
            return Terminal.Terminal.Execute("mkpasswd -m sha-512 " + inputString + " -s \"" + salt + "\"");
        }
    }
}