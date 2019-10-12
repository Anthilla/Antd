﻿using anthilla.core;
using RestWrapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Kvpbase {
    public class EncryptionModule {
        #region Public-Members

        public Settings CurrentSettings { get; set; }

        #endregion

        #region Private-Members

        #endregion

        #region Constructors-and-Factories

        public EncryptionModule(Settings currentSettings) {
            if(currentSettings == null)
                throw new ArgumentNullException(nameof(currentSettings));
            if(currentSettings.Encryption == null)
                throw new ArgumentException("Settings.Encryption is null or empty");
            if(String.Compare(currentSettings.Encryption.Mode, "local") != 0
                && String.Compare(currentSettings.Encryption.Mode, "server") != 0) {
                throw new ArgumentException("Settings.Encryption.Mode should either be local or server");
            }

            CurrentSettings = currentSettings;
        }

        #endregion

        #region Public-Methods

        public byte[] LocalEncrypt(byte[] clear) {
            if(clear == null)
                return null;
            if(clear.Length < 1)
                return null;

            byte[] iv = Encoding.ASCII.GetBytes(CurrentSettings.Encryption.Iv);
            byte[] salt = Encoding.ASCII.GetBytes(CurrentSettings.Encryption.Salt);
            PasswordDeriveBytes password = new PasswordDeriveBytes(CurrentSettings.Encryption.Passphrase, salt, "SHA1", 2);

            byte[] key = password.GetBytes(256 / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(key, iv);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
            cs.Write(clear, 0, clear.Length);
            cs.FlushFinalBlock();
            byte[] cipherBytes = ms.ToArray();
            ms.Close();
            cs.Close();

            return cipherBytes;
        }

        public string LocalEncrypt(string clear) {
            // Taken from http://www.obviex.com/samples/Encryption.aspx

            byte[] iv = Encoding.ASCII.GetBytes(CurrentSettings.Encryption.Iv);
            byte[] salt = Encoding.ASCII.GetBytes(CurrentSettings.Encryption.Salt);
            byte[] clearBytes = Encoding.UTF8.GetBytes(clear);
            PasswordDeriveBytes password = new PasswordDeriveBytes(CurrentSettings.Encryption.Passphrase, salt, "SHA1", 2);

            byte[] keyBytes = password.GetBytes(256 / 8);
            RijndaelManaged symmKey = new RijndaelManaged();
            symmKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmKey.CreateEncryptor(keyBytes, iv);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);

            cs.Write(clearBytes, 0, clearBytes.Length);
            cs.FlushFinalBlock();
            byte[] cipherBytes = ms.ToArray();
            ms.Close();
            cs.Close();
            string cipher = Convert.ToBase64String(cipherBytes);

            return cipher;
        }

        public byte[] LocalDecrypt(byte[] cipher) {
            if(cipher == null)
                return null;
            if(cipher.Length < 1)
                return null;

            // Taken from http://www.obviex.com/samples/Encryption.aspx
            //if(Common.IsTrue(CurrentSettings.Debug.DebugEncryption))
            //    ConsoleLogger.Warn("LocalDecrypt cipher: " + Common.BytesToBase64(cipher));

            byte[] iv = Encoding.ASCII.GetBytes(CurrentSettings.Encryption.Iv);
            byte[] salt = Encoding.ASCII.GetBytes(CurrentSettings.Encryption.Salt);
            PasswordDeriveBytes password = new PasswordDeriveBytes(CurrentSettings.Encryption.Passphrase, salt, "SHA1", 2);

            byte[] keyBytes = password.GetBytes(256 / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, iv);
            MemoryStream ms = new MemoryStream(cipher);
            CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            byte[] clear = new byte[cipher.Length];
            int decryptedCount = cs.Read(clear, 0, clear.Length);
            ms.Close();
            cs.Close();

            return clear;
        }

        public string LocalDecrypt(string cipher) {
            // Taken from http://www.obviex.com/samples/Encryption.aspx

            byte[] iv = Encoding.ASCII.GetBytes(CurrentSettings.Encryption.Iv);
            byte[] salt = Encoding.ASCII.GetBytes(CurrentSettings.Encryption.Salt);
            byte[] cipherBytes = Convert.FromBase64String(cipher);
            PasswordDeriveBytes password = new PasswordDeriveBytes(CurrentSettings.Encryption.Passphrase, salt, "SHA1", 2);

            byte[] keyBytes = password.GetBytes(256 / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, iv);
            MemoryStream ms = new MemoryStream(cipherBytes);
            CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            byte[] clearBytes = new byte[cipherBytes.Length];
            int decryptedCount = cs.Read(clearBytes, 0, clearBytes.Length);
            ms.Close();
            cs.Close();
            string clear = Encoding.UTF8.GetString(clearBytes, 0, decryptedCount);

            return clear;
        }

        public bool ServerDecrypt(byte[] cipher, string ksn, out byte[] clear) {
            clear = null;
            if(cipher == null)
                return false;
            if(cipher.Length < 1)
                return false;
            if(String.IsNullOrEmpty(ksn))
                return false;

            RestResponse resp = new RestResponse();
            string url = "";
            DateTime startTime = DateTime.Now;
            Dictionary<string, string> headers = new Dictionary<string, string>();
            EncryptedMessage req = new EncryptedMessage();

            if(Common.IsTrue(CurrentSettings.Encryption.Ssl))
                url = "https://";
            else
                url = "http://";
            url += CurrentSettings.Encryption.Server + ":" + CurrentSettings.Encryption.Port + "/decrypt";

            headers = Common.AddToDictionary(CurrentSettings.Encryption.ApiKeyHeader, CurrentSettings.Encryption.ApiKeyValue, null);

            req.Cipher = cipher;
            req.Ksn = ksn;

            resp = RestRequest.SendRequestSafe(
                url, "application/json", "POST", null, null, false,
                Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                headers,
                Encoding.UTF8.GetBytes(Common.SerializeJson(req)));

            if(resp == null) {
                ConsoleLogger.Warn("ServerDecrypt null rest response returned");
                return false;
            }

            if(resp.StatusCode != 200) {
                ConsoleLogger.Warn("ServerDecrypt non-200 response returned: " + resp.StatusCode);
                return false;
            }

            clear = resp.Data;

            //if(Common.IsTrue(CurrentSettings.Debug.DebugEncryption)) {
            //    ConsoleLogger.Warn("ServerDecrypt completed " + Common.TotalMsFrom(startTime) + "ms: " +
            //        cipher.Length + "B cipher " +
            //        clear.Length + "B clear, " +
            //        "KSN " + ksn);
            //}

            return true;
        }

        public bool ServerEncrypt(byte[] clear, out byte[] cipher, out string ksn) {
            cipher = null;
            ksn = "";

            if(clear == null)
                return false;
            if(clear.Length < 1)
                return false;

            RestResponse resp = new RestResponse();
            string url = "";
            DateTime startTime = DateTime.Now;
            Dictionary<string, string> headers = new Dictionary<string, string>();
            EncryptedMessage ret = new EncryptedMessage();

            if(Common.IsTrue(CurrentSettings.Encryption.Ssl))
                url = "https://";
            else
                url = "http://";
            url += CurrentSettings.Encryption.Server + ":" + CurrentSettings.Encryption.Port + "/encrypt";

            headers = Common.AddToDictionary(CurrentSettings.Encryption.ApiKeyHeader, CurrentSettings.Encryption.ApiKeyValue, null);

            resp = RestRequest.SendRequestSafe(
                url, null, "POST", null, null, false,
                Common.IsTrue(CurrentSettings.Rest.AcceptInvalidCerts),
                headers,
                clear);

            if(resp == null) {
                ConsoleLogger.Warn("ServerEncrypt null rest response returned");
                return false;
            }

            if(resp.StatusCode != 200) {
                ConsoleLogger.Warn("ServerEncrypt non-200 response returned: " + resp.StatusCode);
                return false;
            }

            try {
                ret = Common.DeserializeJson<EncryptedMessage>(resp.Data);
            }
            catch(Exception) {
                ConsoleLogger.Warn("ServerEncrypt unable to deserialize rest response body to encrypted message");
                return false;
            }

            if(ret == null) {
                ConsoleLogger.Warn("ServerEncrypt null response object after deserialization");
                return false;
            }

            cipher = ret.Cipher;
            ksn = ret.Ksn;

            //if(Common.IsTrue(CurrentSettings.Debug.DebugEncryption)) {
            //    ConsoleLogger.Warn("ServerEncrypt completed " + Common.TotalMsFrom(startTime) + "ms: " +
            //        clear.Length + "B clear, " +
            //        cipher.Length + "B cipher " +
            //        "KSN " + ksn);
            //}

            return true;
        }

        #endregion
    }
}
