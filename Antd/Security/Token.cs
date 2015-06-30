using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Antd.Security {

    public class TokenModel {

        [Key]
        public string _Id { get; set; }

        public string Guid { get; set; }

        public string Session { get; set; }

        public string Value { get; set; }
    }

    public class TokenRepository {

        public List<TokenModel> GetAll(string session) {
            List<TokenModel> list = DeNSo.Session.New.Get<TokenModel>(i => i.Session == session).ToList();
            return list;
        }

        public TokenModel GetBySession(string session) {
            TokenModel item = DeNSo.Session.New.Get<TokenModel>(i => i.Session == session).FirstOrDefault();
            return item;
        }

        public TokenModel Create(string session) {
            var captchas = GetAll(session);
            foreach (var c in captchas) {
                DeNSo.Session.New.Delete(c);
            }

            TokenModel item = new TokenModel();
            item._Id = Guid.NewGuid().ToString();
            item.Guid = Guid.NewGuid().ToString();
            item.Session = session;
            item.Value = Token.Generate();

            DeNSo.Session.New.Set(item);
            return item;
        }

        public void Delete(string session) {
            TokenModel item = DeNSo.Session.New.Get<TokenModel>(i => i.Session == session).FirstOrDefault();
            if (item != null) {
                DeNSo.Session.New.Delete(item);
            }
        }

        public string Fetch(string session) {
            var item = GetBySession(session);
            var value = item.Value;
            DeNSo.Session.New.Delete(item);
            return value;
        }
    }

    public static class Token {

        public static string Generate() {
            string randomString = "";
            foreach (var s in RandomStrings(6)) {
                randomString += s.ToString();
            }
            return randomString;
        }

        public static string Generate(int lenght) {
            string randomString = "";
            foreach (var s in RandomStrings(lenght)) {
                randomString += s.ToString();
            }
            return randomString;
        }

        private static List<char> RandomStrings(int lenght) {
            const string AllowedChars = "0123456789";
            char[] allChar = AllowedChars.ToCharArray();
            List<char> chars = new List<char>();

            for (int i = 1; i <= lenght; i++) {
                Random rnd = new Random(Guid.NewGuid().GetHashCode());
                chars.Add(allChar[rnd.Next(0, allChar.Length)]);
            }
            return chars;
        }
    }
}