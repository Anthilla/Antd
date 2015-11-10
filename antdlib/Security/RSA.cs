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

    public class Rsa : RsaCore {
        public static readonly UnicodeEncoding _encoder = new UnicodeEncoding();

        public static void Test() {
            var keys = GenerateKeys();
            var rsa = new RSACryptoServiceProvider();
            var param = keys.Public.Param;
            rsa.ImportParameters(param);
            const string text = "helloworld";
            Console.WriteLine("Rsa // Text to encrypt: " + text);
            var enc = CoreEncrypt(text);
            Console.WriteLine("Rsa // Encrypted Text: " + enc);
            var dec = CoreDecrypt(enc);
            Console.WriteLine("Rsa // Decrypted Text: " + dec);
            Console.WriteLine("");
        }

        public static string Encrypt(string data, RsaKeys.Public key) {
            var rsa = new RSACryptoServiceProvider();
            var param = key.Param;
            rsa.ImportParameters(param);
            var dataToEncrypt = _encoder.GetBytes(data);
            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
            var length = encryptedByteArray.Length;
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

        public static string Decrypt(string data, RsaKeys.Private key) {
            var rsa = new RSACryptoServiceProvider();
            var dataArray = data.Split(',');
            var dataByte = new byte[dataArray.Length];
            for (var i = 0; i < dataArray.Length; i++) {
                dataByte[i] = Convert.ToByte(dataArray[i]);
            }
            var param = key.Param;
            rsa.ImportParameters(param);
            var decryptedByte = rsa.Decrypt(dataByte, false);
            return _encoder.GetString(decryptedByte);
        }
    }
}
