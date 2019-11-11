using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;

namespace AntdUi2 {
    /// <summary>
    /// Classe per gestire le chiamate Rest e le loro risposte tra i due servizi di HOPLITE
    /// </summary>
    public class RestConsumer {

        private static string ServerUrl;
        private static string SessionCookieKey;
        private static string ApplicationName;
        private const string SessionInstanceGuidHeader = "X_ASP_session_instance_guid";
        private const string SessionContextSourceHeader = "X_ANTH_session_context";

        /// <summary>
        /// Costruttore
        /// </summary>
        /// <param name="serverUrl">Assegnato all'attributo <see cref="ServerUrl"/></param>
        /// <param name="sessionCookieKey">Assegnato all'attributo <see cref="SessionCookieKey"/></param>
        /// <param name="appname">Assegnato all'attributo <see cref="ApplicationName"/></param>
        public RestConsumer(string serverUrl, string sessionCookieKey, string appname) {
            if (string.IsNullOrEmpty(serverUrl)) {
                throw new System.ArgumentException("La variabile deve essere popolata", nameof(serverUrl));
            }

            if (string.IsNullOrEmpty(sessionCookieKey)) {
                throw new System.ArgumentException("La variabile deve essere popolata", nameof(sessionCookieKey));
            }

            if (string.IsNullOrEmpty(appname)) {
                throw new System.ArgumentException("La variabile deve essere popolata", nameof(appname));
            }

            ServerUrl = serverUrl;
            SessionCookieKey = sessionCookieKey;
            ApplicationName = appname;
        }

        /// <summary>
        /// Dizionario statico per convertire i Metodi Rest da stringa a <see cref="RestSharp.Method"/>
        /// </summary>
        private readonly Dictionary<string, Method> METHODS = new Dictionary<string, Method>() {
            { "GET", Method.GET },
            { "POST", Method.POST },
            { "PUT", Method.PUT },
            { "DELETE", Method.DELETE },
            { "HEAD", Method.HEAD },
            { "OPTIONS", Method.OPTIONS },
            { "PATCH", Method.PATCH },
            { "MERGE", Method.MERGE },
            { "COPY", Method.COPY }
        };

        /// <summary>
        /// Converte la <see cref="Nancy.Request"/> e la ridireziona verso il servizio di backend.
        /// Ottiene quindi una <see cref="RestSharp.IRestResponse"/> dal backend che viene convertita in <see cref="Nancy.Response"/>.
        /// </summary>
        /// <param name="request">Richiesta di tipo <see cref="Nancy.Request"/></param>
        /// <param name="sessionGuid">ID della sessione che effettua la richiesta da inoltrare al backend</param>
        /// <param name="timeout">Timeout in ms per la richiesta al backend. Default <see cref="int.MaxValue"/> ms.</param>
        /// <param name="overridePath">Parametro facoltativo che sovrascrive il Path della richiesta <see cref="Nancy.Request.Path"/></param>
        /// <returns></returns>
        public Nancy.Response Redirect(Nancy.Request request, string sessionGuid, int timeout = int.MaxValue, string overridePath = "") {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            var requestPath = string.IsNullOrEmpty(overridePath) ? request.Path : overridePath;
            var client = new RestClient(ServerUrl);
            var restRequest = new RestRequest(requestPath, METHODS[request.Method]) {
                Timeout = timeout
            };
            restRequest.AddHeader(SessionInstanceGuidHeader, sessionGuid);
            restRequest.AddHeader(SessionContextSourceHeader, ApplicationName);

            foreach (var key in request.Form) {
                restRequest.AddParameter(key, request.Form[key]);
            }

            foreach (var parameter in request.Query) {
                restRequest.AddQueryParameter(parameter, request.Query[parameter]);
            }

            var response = client.Execute(restRequest);
            return ConvertResponse(response);
        }

        /// <summary>
        /// Converte la <see cref="Nancy.Request"/> e la ridireziona verso il servizio di backend.
        /// Ottiene quindi una <see cref="RestSharp.IRestResponse"/> dal backend che viene convertita in <see cref="Nancy.Response"/>.
        /// </summary>
        /// <param name="request">Richiesta di tipo <see cref="Nancy.Request"/></param>
        /// <param name="sessionGuid">ID della sessione che effettua la richiesta da inoltrare al backend</param>
        /// <param name="parameters">Dizionario di parametri da aggiungere alla richiesta</param>
        /// <param name="timeout">Timeout in ms per la richiesta al backend. Default <see cref="int.MaxValue"/> ms.</param>
        /// <param name="overridePath">Parametro facoltativo che sovrascrive il Path della richiesta <see cref="Nancy.Request.Path"/></param>
        /// <returns></returns>
        public Nancy.Response RedirectWithParameters(Nancy.Request request, string sessionGuid, IDictionary<string, string> parameters, int timeout = int.MaxValue, string overridePath = "") {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            if (parameters == null) {
                throw new ArgumentNullException(nameof(parameters));
            }
            var requestPath = string.IsNullOrEmpty(overridePath) ? request.Path : overridePath;
            var client = new RestClient(ServerUrl);
            var restRequest = new RestRequest(requestPath, METHODS[request.Method]) {
                Timeout = timeout
            };
            restRequest.AddHeader(SessionInstanceGuidHeader, sessionGuid);
            restRequest.AddHeader(SessionContextSourceHeader, ApplicationName);

            foreach (var key in request.Form) {
                restRequest.AddParameter(key, request.Form[key]);
            }
            foreach (var parameter in parameters) {
                restRequest.AddParameter(parameter.Key, parameter.Value);
            }

            foreach (var parameter in request.Query) {
                restRequest.AddQueryParameter(parameter, request.Query[parameter]);
            }

            var response = client.Execute(restRequest);
            return ConvertResponse(response);
        }

        public Nancy.Response RedirectWithHeaders(Nancy.Request request, string sessionGuid, IDictionary<string, string> headers, int timeout = int.MaxValue, string overridePath = "") {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            if (headers == null) {
                throw new ArgumentNullException(nameof(headers));
            }
            var requestPath = string.IsNullOrEmpty(overridePath) ? request.Path : overridePath;
            var client = new RestClient(ServerUrl);
            var restRequest = new RestRequest(requestPath, METHODS[request.Method]) {
                Timeout = timeout
            };
            restRequest.AddHeader(SessionInstanceGuidHeader, sessionGuid);
            restRequest.AddHeader(SessionContextSourceHeader, ApplicationName);

            foreach (var header in headers) {
                restRequest.AddHeader(header.Key, header.Value);
            }

            foreach (var key in request.Form) {
                restRequest.AddParameter(key, request.Form[key]);
            }
            foreach (var parameter in request.Query) {
                restRequest.AddQueryParameter(parameter, request.Query[parameter]);
            }

            var response = client.Execute(restRequest);
            return ConvertResponse(response);
        }

        #region [    Private Methods    ]
        /// <summary>
        /// Converte <see cref="RestSharp.IRestResponse"/> in <see cref=Nancy.Response"/>
        /// </summary>
        /// <param name="restResponse"></param>
        /// <returns></returns>
        private Nancy.Response ConvertResponse(IRestResponse restResponse) {
            var nancyResponse = (Nancy.Response)restResponse.Content;
            nancyResponse.StatusCode = (Nancy.HttpStatusCode)restResponse.StatusCode;
            foreach (var restCookie in restResponse.Cookies) {
                nancyResponse.Cookies.Add(new Nancy.Cookies.NancyCookie(restCookie.Name, restCookie.Value, restCookie.HttpOnly, restCookie.Secure, restCookie.Expires));
            }
            nancyResponse.ContentType = restResponse.ContentType;
            foreach (var restHeader in restResponse.Headers) {
                nancyResponse.Headers.Add(restHeader.Name, restHeader.Value.ToString());
            }
            return nancyResponse;
        }
        #endregion

        /// <summary>
        /// GET e deserializza su T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">Url della richiesta</param>
        /// <returns></returns>
        public T Get<T>(string path) {
            var client = new RestClient(ServerUrl);
            var request = new RestRequest(path, Method.GET);
            request.AddHeader(SessionContextSourceHeader, ApplicationName);
            var response = client.Execute(request);
            var result = response.Content;
            return result != null ? JsonConvert.DeserializeObject<T>(result) : default(T);
        }

        public string GetString(string sessionGuid, string path) {
            var client = new RestClient(ServerUrl);
            var request = new RestRequest(path, Method.GET);
            request.AddHeader(SessionInstanceGuidHeader, sessionGuid);
            request.AddHeader(SessionContextSourceHeader, ApplicationName);
            var response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK) {
                return response.Content;
            }
            throw new System.Exception($"chiamata a {path} risulta {response.StatusCode}");
        }
    }
}