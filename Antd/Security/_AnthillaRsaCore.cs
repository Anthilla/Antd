//using System;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;

//namespace Antd.Security {

//    public class AnthillaPrivateKey {

//        public RSAParameters Param { get; set; }

//        public string XML { get; set; }

//        public byte[] Modulus { get; set; }

//        public byte[] Exponent { get; set; }

//        public byte[] P { get; set; }

//        public byte[] Q { get; set; }

//        public byte[] DP { get; set; }

//        public byte[] DQ { get; set; }

//        public byte[] InverseQ { get; set; }

//        public byte[] D { get; set; }
//    }

//    public class AnthillaPublicKey {

//        public RSAParameters Param { get; set; }

//        public string XML { get; set; }

//        public byte[] Modulus { get; set; }

//        public byte[] Exponent { get; set; }
//    }

//    public class AnthillaRsaKeys {

//        public AnthillaPrivateKey Private { get; set; }

//        public AnthillaPublicKey Public { get; set; }
//    }

//    public class AnthillaRsaCore {
//        private static string _privateKey;
//        private static string _publicKey;
//        private static UnicodeEncoding _encoder = new UnicodeEncoding();

//        public static void Test() {
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

//        public static AnthillaRsaKeys GenerateKeys() {
//            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(3072);
//            Console.WriteLine("A new key pair of legth {0} was created", rsa.KeySize);
//            _privateKey = rsa.ToXmlString(true);
//            _publicKey = rsa.ToXmlString(false);
//            RSAParameters privateParam = rsa.ExportParameters(true);
//            RSAParameters publicParam = rsa.ExportParameters(false);
//            AnthillaPrivateKey anthPrivateKey = MapPrivate(privateParam, _privateKey);
//            AnthillaPublicKey anthPublicKey = MapPublic(publicParam, _publicKey);

//            AnthillaRsaKeys keys = new AnthillaRsaKeys();
//            keys.Private = anthPrivateKey;
//            keys.Public = anthPublicKey;
//            return keys;
//        }

//        private static AnthillaPrivateKey MapPrivate(RSAParameters key, string xml) {
//            AnthillaPrivateKey upk = new AnthillaPrivateKey();
//            upk.Param = key;
//            upk.XML = xml;
//            upk.Modulus = key.Modulus;
//            upk.Exponent = key.Exponent;
//            upk.P = key.P;
//            upk.Q = key.Q;
//            upk.DP = key.DP;
//            upk.DQ = key.DQ;
//            upk.InverseQ = key.InverseQ;
//            upk.D = key.D;
//            return upk;
//        }

//        private static AnthillaPublicKey MapPublic(RSAParameters key, string xml) {
//            AnthillaPublicKey upk = new AnthillaPublicKey();
//            upk.Param = key;
//            upk.XML = xml;
//            upk.Modulus = key.Modulus;
//            upk.Exponent = key.Exponent;
//            return upk;
//        }

//        public static string CoreEncrypt(string data) {
//            var rsa = new RSACryptoServiceProvider();
//            rsa.FromXmlString(_publicKey);
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

//        public static string CoreDecrypt(string data) {
//            var rsa = new RSACryptoServiceProvider();
//            var dataArray = data.Split(new char[] { ',' });
//            byte[] dataByte = new byte[dataArray.Length];
//            for (int i = 0; i < dataArray.Length; i++) {
//                dataByte[i] = Convert.ToByte(dataArray[i]);
//            }

//            rsa.FromXmlString(_privateKey);
//            var decryptedByte = rsa.Decrypt(dataByte, false);
//            return _encoder.GetString(decryptedByte);
//        }
//    }
//}