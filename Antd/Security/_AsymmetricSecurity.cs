//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tickets;

//namespace Antd.Security {
//    public class AnthillaPrivateKeyTmp {
//        public string password { get; set; }
//        public byte[] key { get; set; }
//        public byte[] vector { get; set; }
//    }

//    public class AnthillaPublicKeyTmp {
//        public byte[] key { get; set; }
//        public byte[] vector { get; set; }
//    }

//    public static class ArrayExtensions {
//        public static byte[] CutForKey(this Byte[] arr) {
//            byte[] newArray = new byte[32];
//            Array.Copy(arr, newArray, newArray.Length);
//            return newArray;
//        }

//        public static byte[] CutForVector(this Byte[] arr) {
//            byte[] newArray = new byte[16];
//            Array.Copy(arr, newArray, newArray.Length);
//            return newArray;
//        }
//    }

//    public class AsymmetricSecurity : AnthillaSecurity {
//        private static RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
//        public static AnthillaPrivateKeyTmp GeneratePrivateKey(string password) {
//            var aPrivateKey = new AnthillaPrivateKeyTmp();
//            aPrivateKey.password = AnthillaHash(password);
//            var k = AnthillaKey(password);
//            var v = AnthillaVector(password);

//            string xmlPrivateKey = rsa.ToXmlString(true);
//            byte[] privateKey = encryptBytes(xmlPrivateKey, k, v);
//            byte[] privateVector = encryptBytes(xmlPrivateKey, k, v);

//            aPrivateKey.key = privateKey;
//            aPrivateKey.vector = privateVector;

//            return aPrivateKey;
//        }

//        public static AnthillaPublicKeyTmp GeneratePublicKey(AnthillaPrivateKeyTmp privateKey) {
//            var k = privateKey.key;
//            var v = privateKey.vector;

//            string xmlPublicKey = rsa.ToXmlString(false);
//            byte[] publicKey = encryptBytes(xmlPublicKey, k.CutForKey(), v.CutForVector());
//            byte[] publicVector = v;

//            var aPublicKey = new AnthillaPublicKeyTmp();
//            aPublicKey.key = publicKey;
//            aPublicKey.vector = publicVector;

//            return aPublicKey;
//        }

//        public static bool CheckKeys(AnthillaPrivateKeyTmp privateKey, AnthillaPublicKeyTmp publicKey) {
//            var privateVector = privateKey.vector;
//            var publicVector = publicKey.vector;
//            if (privateVector == publicVector) {
//                return true;
//            }
//            else {
//                return false;
//            }
//        }

//        public static byte[] PublicEncrypt(string message, AnthillaPublicKeyTmp publicKey) {
//            var k = publicKey.key;
//            var v = publicKey.vector;
//            byte[] key = k.CutForKey();
//            byte[] vector = v.CutForVector();
//            var encryptedMessage = Encrypt(message, key, vector);
//            return encryptedMessage;
//        }

//        public static string PublicDecrypt(byte[] encryptedMessage, AnthillaPublicKeyTmp publicKey, AnthillaPrivateKeyTmp privateKey) {
//            string message;
//            var k = publicKey.key;
//            var v = publicKey.vector;
//            byte[] key = k.CutForKey();
//            byte[] vector = v.CutForVector();
//            var check = CheckKeys(privateKey, publicKey);
//            if (check == true) {
//                message = Decrypt(encryptedMessage, key, vector);
//            }
//            else {
//                message = ToHex(encryptedMessage);
//            }
//            return message;
//        }

//        public static void Test() {
//            //qui ho la chiave per leggere, che è quella del destinatario del messaggio
//            var pri = GeneratePrivateKey("anthilla");
//            //la chiave pubblica è fatta per crittografare il messaggio, è del destinatario ma la usa il mittente
//            var pub = GeneratePublicKey(pri);

//            var message = "ciao a tutti";
//            Console.WriteLine(message);
//            Console.WriteLine("");

//            var encrypted = PublicEncrypt(message, pub);
//            Console.WriteLine(encrypted);
//            Console.WriteLine("");

//            //todo: usare solo la chiave privata, inserire nelle chiavi un SN e recuperare così quella pubblica
//            //oppure, sapendo di chi è la chiave pubblica si risale al proprietario e si recupera quella privata
//            //all'interno dei metadati dell'utente c'è comunque la coppia di chiavi
//            //utilizzare quindi l'utente per risalire all'altra chiave
//            var decrypted = PublicDecrypt(encrypted, pub, pri);
//            Console.WriteLine(decrypted);
//            Console.WriteLine("");
//        }
//    }
//}