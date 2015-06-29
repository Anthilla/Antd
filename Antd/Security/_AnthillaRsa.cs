//using System;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;

//namespace Antd.Security {

//    public class AnthillaRsa : AnthillaRsaCore {
//        private static UnicodeEncoding _encoder = new UnicodeEncoding();

//        public static void Test2() {
//            AnthillaRsaKeys keys = GenerateKeys();
//            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
//            RSAParameters param = keys.Public.Param;
//            rsa.ImportParameters(param);

//            string text = "helloworld";
//            Console.WriteLine("RSA // Text to encrypt: " + text);
//            var enc = CoreEncrypt(text);
//            Console.WriteLine("RSA // Encrypted Text: " + enc);
//            var dec = CoreDecrypt(enc);
//            Console.WriteLine("RSA // Decrypted Text: " + dec);
//            Console.WriteLine("");
//        }

//        public static string Encrypt(string data, AnthillaPublicKey key) {
//            var rsa = new RSACryptoServiceProvider();
//            var param = key.Param;
//            rsa.ImportParameters(param);
//            var dataToEncrypt = _encoder.GetBytes(data);
//            var encryptedByteArray = rsa.Encrypt(dataToEncrypt, false).ToArray();
//            var length = encryptedByteArray.Count();
//            var item = 0;
//            var sb = new StringBuilder();
//            foreach (var x in encryptedByteArray) {
//                item++;
//                sb.Append(x);

//                if (item < length)
//                    sb.Append(",");
//            }
//            return sb.ToString();
//        }

//        public static string Decrypt(string data, AnthillaPrivateKey key) {
//            var rsa = new RSACryptoServiceProvider();
//            var dataArray = data.Split(new char[] { ',' });
//            byte[] dataByte = new byte[dataArray.Length];
//            for (int i = 0; i < dataArray.Length; i++) {
//                dataByte[i] = Convert.ToByte(dataArray[i]);
//            }
//            var param = key.Param;
//            rsa.ImportParameters(param);
//            var decryptedByte = rsa.Decrypt(dataByte, false);
//            return _encoder.GetString(decryptedByte);
//        }
//    }
//}