using antdlib.Models;
using antdlib.Security;
using library.u2f;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace antdlib.Auth {

    public class TokenAuthentication {
        public static IEnumerable<GlobalUserModel> Show() {
            return DeNSo.Session.New.Get<GlobalUserModel>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="values">
        ///     TKey è il tipo di identità (facoltativo)
        ///     TValue è il valore dell'identità
        /// </param>
        public static void RegisterUser(string guid, HashSet<KeyValuePair<string, string>> values) {
            var i = DeNSo.Session.New.Get<GlobalUserModel>(_ => _.GlobalUID == guid).FirstOrDefault();
            if (i == null) {
                i = new GlobalUserModel() { _Id = Guid.NewGuid().ToString(), GlobalUID = Guid.NewGuid().ToString() };
            }
            i.Identities = i.Identities.Union(values);
            DeNSo.Session.New.Set(i);
        }

        public static void AssignOTPToken(string guid, string tokenID) {
            var otpKVP = new KeyValuePair<string, string>("otptoken", tokenID.Substring(0, 12));
            RegisterUser(guid, new HashSet<KeyValuePair<string, string>>() { otpKVP });
        }

        public static void DeleteRelation(string guid) {
            var i = DeNSo.Session.New.Get<GlobalUserModel>(_ => _.GlobalUID == guid).FirstOrDefault();
            DeNSo.Session.New.Delete(i);
        }

        /// <summary>
        /// controlla se esiste un'identità nel DB e ne restituisce il GlobalUID
        /// return null se non esiste niente
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public static string CheckIdentity(string identity) {
            var i = DeNSo.Session.New.Get<GlobalUserModel>().Where(g => g.Identities.Select(_ => _.Value).ToList().Contains(identity)).First();
            return (i == null) ? null : i.GlobalUID;
        }

        private static bool ValidateUserIdentity(string username, string password) {
            var authenticationUrl = "http://127.0.0.1/wsapi/u2fval/";
            var request = (HttpWebRequest)WebRequest.Create(authenticationUrl);
            request.Method = "GET";
            request.UseDefaultCredentials = false;
            request.PreAuthenticate = true;
            request.Credentials = new NetworkCredential(username, password);
            var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK) {
                return true;
            }
            return false;
        }

        //private static bool ValidateToken(string username, string otp) {
        //    var user = Show().Where(u => u.username == username).First();
        //    if (otp.Length < 12) {
        //        return false;
        //    }
        //    if (user == null) {
        //        return false;
        //    }
        //    else {
        //        if (user.tokenID != otp.Substring(0, 12)) {
        //            return false;
        //        }
        //        else {
        //            return true;
        //        }
        //    }
        //}

        private static bool ConfirmTokenValidity(string otp) {
            try {
                if (otp.Length < 12) {
                    return false;
                }
                var answer = new U2FRequest("25311", "5hQfQbHQGLIauepG9Sa5LQAMGYk=").Validate(otp);
                if (answer.IsSignatureValid == false && answer.IsValid == false) {
                    return false;
                }
                else {
                    return true;
                }
            }
            catch (Exception) {
                return false;
            }
        }

        public static bool Validate(string username, string password, string otp) {
            var _validateUserIdentity = ValidateUserIdentity(username, password);
            var _validateToken = CheckIdentity(otp);
            var _confirmTokenValidity = ConfirmTokenValidity(otp);
            if (_validateUserIdentity == true && _confirmTokenValidity == true) {
                return true;
            }
            return false;
        }
    }
}
