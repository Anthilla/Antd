using System;
using System.Security.Cryptography;

namespace Antd.cmds {
    public class Secret {
        public static string Gen(int length = 64) {
            var key = new byte[64];
            using(var rng = new RNGCryptoServiceProvider()) {
                rng.GetBytes(key);
            }
            var c = new byte[length];
            using(var rng = new RNGCryptoServiceProvider()) {
                rng.GetBytes(c);
            }
            byte[] hashValue;

            using(var hmac = new HMACMD5(key)) {
                hashValue = hmac.ComputeHash(c);
            }
            return Convert.ToBase64String(hashValue);
        }
    }
}
