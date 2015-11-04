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
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace antdlib.Security {
    public class RSACore {
        private static string _privateKey;
        private static string _publicKey;
        private static UnicodeEncoding _encoder = new UnicodeEncoding();

        public static RSAKeys.Pair GenerateKeys() {
            var rsa = new RSACryptoServiceProvider(3072);
            //Console.WriteLine("A new key pair of legth {0} was created", rsa.KeySize);
            _privateKey = rsa.ToXmlString(true);
            _publicKey = rsa.ToXmlString(false);
            var privateParam = rsa.ExportParameters(true);
            var publicParam = rsa.ExportParameters(false);
            var privateKey = MapPrivateKey(privateParam, _privateKey);
            var publicKey = MapPublicKey(publicParam, _publicKey);
            var keys = new RSAKeys.Pair() {
                Private = privateKey,
                Public = publicKey
            };
            return keys;
        }

        private static RSAKeys.Private MapPrivateKey(RSAParameters key, string xml) {
            return new RSAKeys.Private() {
                Param = key,
                XML = xml,
                Modulus = key.Modulus,
                Exponent = key.Exponent,
                P = key.P,
                Q = key.Q,
                DP = key.DP,
                DQ = key.DQ,
                InverseQ = key.InverseQ,
                D = key.D
            };
        }

        private static RSAKeys.Public MapPublicKey(RSAParameters key, string xml) {
            return new RSAKeys.Public() {
                Param = key,
                XML = xml,
                Modulus = key.Modulus,
                Exponent = key.Exponent
            };
        }

        public static string CoreEncrypt(string data) {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(_publicKey);
            var dataToEncrypt = _encoder.GetBytes(data);
            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            var length = encryptedByteArray.Count();
            var item = 0;
            var sb = new StringBuilder();
            foreach (var x in encryptedByteArray) {
                item++;
                sb.Append(x);

                if (item < length)
                    sb.Append(",");
            }
            return sb.ToString();
        }

        public static string CoreDecrypt(string data) {
            var rsa = new RSACryptoServiceProvider();
            var dataArray = data.Split(new char[] { ',' });
            byte[] dataByte = new byte[dataArray.Length];
            for (int i = 0; i < dataArray.Length; i++) {
                dataByte[i] = Convert.ToByte(dataArray[i]);
            }
            rsa.FromXmlString(_privateKey);
            var decryptedByte = rsa.Decrypt(dataByte, false);
            return _encoder.GetString(decryptedByte);
        }
    }
}
