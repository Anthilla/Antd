using Antd.models;
using anthilla.core;
using anthilla.crypto;
using Nancy;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd.cmds {
    public class Login {

        public class Identity : IUserIdentity {
            public Guid UserGuid { get; set; }

            /// <summary>
            /// Implementato da IUserIdentity
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// Implementato da IUserIdentity
            /// </summary>
            public IEnumerable<string> Claims { get; set; }
        }

        /// <summary>
        /// Meccanismo di autenticazione centralizzato
        /// Prende i parametri id e password e li confronta coi dati di Application.CurrentConfiguration.Users.ApplicativeUsers
        /// Flusso:
        ///     - ci sono utenti?
        ///     - c'è l'utente richiesto?
        ///     - l'utente trovato è abilitato a fare l'autenticazione?
        ///     - il parametro password corrisponde?
        /// </summary>
        /// <param name="id">Parametro dell'utente, es: username</param>
        /// <param name="claims">Parametri dell'utente, es: hashing della password</param>
        /// <returns>Restituisce un HttpStatusCode, solo se l'autenticazione è andata a buon fine restituisce OK</returns>
        public static HttpStatusCode Authenticate(string id, string[] claims) {
            var current = Application.CurrentConfiguration.Users.ApplicativeUsers;
            if(!current.Any()) {
                return HttpStatusCode.Unauthorized;
            }
            var method = current.FirstOrDefault(_ => CommonString.AreEquals(_.Id, id) == true);
            if(method == null) {
                return HttpStatusCode.Unauthorized;
            }
            if(!method.Active) {
                return HttpStatusCode.Unauthorized;
            }

            switch(method.Type) {
                case AuthenticationType.none:
                    return HttpStatusCode.Unauthorized;
                case AuthenticationType.simple:
                    return SimpleAuthentication(claims, method.Claims);
                default:
                    return HttpStatusCode.Unauthorized;
            }
        }

        /// <summary>
        /// Autenticazione semplice confrontando i dati del repository
        /// Confronta tutti i valori delle claims
        /// </summary>
        /// <param name="claims"></param>
        /// <param name="foundClaims"></param>
        /// <returns></returns>
        private static HttpStatusCode SimpleAuthentication(string[] claims, string[] foundClaims) {
            if(claims.Length != foundClaims.Length) {
                return HttpStatusCode.Unauthorized;
            }
            for(var i = 0; i < claims.Length; i++) {
                var hash = SHA.Generate(claims[i]);
                if(!foundClaims.Contains(hash)) {
                    return HttpStatusCode.Unauthorized;
                }
            }
            return HttpStatusCode.OK;
        }
    }
}
