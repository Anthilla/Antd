using anthilla.core;
using Nancy;
using Nancy.Authentication.Forms;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace AntdUi2.Modules.Auth {

    /// <summary>
    /// Metodi per gestire l'autenticazione dell'utente
    /// </summary>
    public class UserDatabase : IUserMapper {

        /// <summary>
        /// Validazione della richiesta di autenticazione da parte dell'utente
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="userIdentity"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Guid? ValidateUser(string serverUrl, string userIdentity, string password) {
            try {
                var authResult = Authenticate(serverUrl, userIdentity, password);
                return authResult.Item1 == HttpStatusCode.OK ? (Guid?)authResult.Item2 : null;
            }
            catch (NullReferenceException nrex) {
                Console.WriteLine("nrex");
                Console.WriteLine(nrex.ToString(), null, "ApiName");
                return null;
            }
            catch (Exception ex) {
                Console.WriteLine("ex");
                Console.WriteLine(ex.ToString(), null, "ApiName");
                return null;
            }
        }

        /// <summary>
        /// Autenticazione dell'utente
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="id"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public static Tuple<HttpStatusCode, Guid> Authenticate(string serverUrl, string id, string claim) {
            return Authenticate(serverUrl, id, new string[] { claim });
        }

        /// <summary>
        /// Autenticazione dell'utente
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="id"></param>
        /// <param name="claims"></param>
        /// <returns></returns>
        public static Tuple<HttpStatusCode, Guid> Authenticate(string serverUrl, string id, string[] claims) {
            //var data = new AuthenticationDataModel() {
            //    Id = id,
            //    Claims = claims
            //};
            //var json = JsonConvert.SerializeObject(data);
            var dict = new Dictionary<string, string> {
                { "Username", id },
                { "Password", claims[0] }
            };
            var a = ApiConsumer.Post<Tuple<HttpStatusCode, Guid>>($"{serverUrl}/phlegyas/authenticate", dict);
            return a;
        }

        /// <summary>
        /// Ottiene i dati relativi all'utente associato all'ID della sessione
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="cookies">Lista di cookie da cui recuperare l'ID della sessione</param>
        /// <returns></returns>
        public static object GetAuthenticatedUserGuid(string serverUrl, IDictionary<string, string> cookies) {
            var sessionCookie = cookies.FirstOrDefault(_ => _.Key == "anthillasp-session");
            if (string.IsNullOrEmpty(sessionCookie.Value)) {
                Console.WriteLine("no cookie");
                return HttpStatusCode.InternalServerError;
            }
            var userGuid = ApiConsumer.Get<Guid>($"{serverUrl}/phlegyas/userguid/by-session/" + sessionCookie.Value);
            return userGuid.ToString();
        }

        /// <summary>
        /// Controlla se la sessione dell'utente è autenticata
        /// </summary>
        /// <param name="serverUrl"></param>
        /// <param name="userIdentity"></param>
        /// <returns></returns>
        public static HttpStatusCode CheckUser(string serverUrl, string userIdentity) {
            try {
                var dict = new Dictionary<string, string> {
                    { "Username", userIdentity }
                };
                return ApiConsumer.Post($"{serverUrl}/phlegyas/checkuser", dict);

            }
            catch (NullReferenceException nrex) {
                Console.WriteLine("nrex");
                Console.WriteLine(nrex.ToString(), null, "ApiName");
                return HttpStatusCode.InternalServerError;
            }
            catch (Exception ex) {
                Console.WriteLine("ex");
                Console.WriteLine(ex.ToString(), null, "ApiName");
                return HttpStatusCode.InternalServerError;
            }
        }

        ClaimsPrincipal IUserMapper.GetUserFromIdentifier(Guid identifier, NancyContext context) {
            return new UserIdentity() {
                UserGuid = identifier,
                UserIdentifier = Encryption.XHash(identifier.ToString()),
                UserName = identifier.ToString().Substring(0, 8),
                Claims = new string[0]
            };
        }
    }
}