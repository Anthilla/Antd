using System;
using System.Collections.Generic;
using Nancy;
using Newtonsoft.Json;
using RestSharp;

namespace AntdUi {
    public class ApiConsumer {

        private static string _instance = Guid.NewGuid().ToString();

        public static string SessionHeaderRequest = "Anth-Session";
        public static string ApiInstanceId = "Anth-InstanceId";

        public ApiConsumer() {
            _instance = Guid.NewGuid().ToString();
        }

        public T Get<T>(string uri, string header = "") {
            try {
                var client = new RestClient(uri);
                var request = new RestRequest("/", Method.GET);
                request.AddHeader(ApiInstanceId, _instance);
                if(!string.IsNullOrEmpty(header)) {
                    request.AddHeader(SessionHeaderRequest, header);
                }
                request.OnBeforeDeserialization = resp => { resp.ContentType = "application/json"; };
                var response = client.Execute(request);
                var result = response.Content;
                return result != null ? JsonConvert.DeserializeObject<T>(result) : default(T);
            }
            catch(Exception) {
                return default(T);
            }
        }

        public void Get(string uri, string header = "") {
            try {
                var client = new RestClient(uri);
                var request = new RestRequest("/", Method.GET);
                request.AddHeader(ApiInstanceId, _instance);
                if(!string.IsNullOrEmpty(header)) {
                    request.AddHeader(SessionHeaderRequest, header);
                }
                client.Execute(request);
            }
            catch(Exception) {
            }
        }

        public T Post<T>(string uri, IDictionary<string, string> data, string header = "") {
            try {
                var client = new RestClient(uri);
                var request = new RestRequest("/", Method.POST);
                request.AddHeader(ApiInstanceId, _instance);
                foreach(var d in data) {
                    request.AddParameter(d.Key, d.Value);
                }
                if(!string.IsNullOrEmpty(header)) {
                    request.AddHeader(SessionHeaderRequest, header);
                }
                var response = client.Execute(request);
                var result = response.Content;
                return result != null ? JsonConvert.DeserializeObject<T>(result) : default(T);
            }
            catch(Exception) {
                return default(T);
            }
        }

        public HttpStatusCode Post(string uri, IDictionary<string, string> data, string header = "") {
            try {
                var client = new RestClient(uri);
                var request = new RestRequest("/", Method.POST);
                request.AddHeader(ApiInstanceId, _instance);
                foreach(var d in data) {
                    request.AddParameter(d.Key, d.Value);
                }
                if(!string.IsNullOrEmpty(header)) {
                    request.AddHeader(SessionHeaderRequest, header);
                }
                var response = client.Execute(request);
                var code = response.StatusCode;
                return (HttpStatusCode)code;
            }
            catch(Exception) {
                return HttpStatusCode.InternalServerError;
            }
        }
    }
}
