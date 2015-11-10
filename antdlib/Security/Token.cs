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
using System.Collections.Generic;
using System.Linq;

namespace antdlib.Security {

    public class TokenModel {
        public string _Id { get; set; }

        public string Guid { get; set; }

        public string Session { get; set; }

        public string Value { get; set; }
    }

    public class TokenRepository {

        public static List<TokenModel> GetAll(string session) {
            var list = DeNSo.Session.New.Get<TokenModel>(i => i.Session == session).ToList();
            return list;
        }

        public static TokenModel GetBySession(string session) {
            var item = DeNSo.Session.New.Get<TokenModel>(i => i.Session == session).FirstOrDefault();
            return item;
        }

        public static TokenModel Create(string session) {
            var captchas = GetAll(session);
            foreach (var c in captchas) {
                DeNSo.Session.New.Delete(c);
            }
            var item = new TokenModel {
                _Id = Guid.NewGuid().ToString(),
                Guid = Guid.NewGuid().ToString(),
                Session = session,
                Value = Token.Generate()
            };
            DeNSo.Session.New.Set(item);
            return item;
        }

        public static void Delete(string session) {
            var item = DeNSo.Session.New.Get<TokenModel>(i => i.Session == session).FirstOrDefault();
            if (item != null) {
                DeNSo.Session.New.Delete(item);
            }
        }

        public static string Fetch(string session) {
            var item = GetBySession(session);
            var value = item.Value;
            DeNSo.Session.New.Delete(item);
            return value;
        }
    }

    public static class Token {

        public static string Generate() {
            return RandomNumbers(6).Aggregate("", (current, s) => current + s.ToString());
        }

        public static string Generate(int lenght) {
            return RandomNumbers(lenght).Aggregate("", (current, s) => current + s.ToString());
        }

        private static List<char> RandomNumbers(int lenght) {
            const string allowedChars = "0123456789";
            var allChar = allowedChars.ToCharArray();
            var chars = new List<char>();

            for (var i = 1; i <= lenght; i++) {
                var rnd = new Random(Guid.NewGuid().GetHashCode());
                chars.Add(allChar[rnd.Next(0, allChar.Length)]);
            }
            return chars;
        }
    }
}