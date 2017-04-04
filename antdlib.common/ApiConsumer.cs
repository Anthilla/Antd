using Nancy;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;

namespace antdlib.common {
    public class ApiConsumer {
        private const string InstanceHeader = "instance-guid";

        private static string _instance;

        public ApiConsumer() {
            _instance = Guid.NewGuid().ToString();
        }

        public T Get<T>(string uri) {
            try {
                var client = new RestClient(uri);
                var request = new RestRequest("/", Method.GET);
                request.AddHeader(InstanceHeader, _instance);
                var response = client.Execute(request);
                var result = response.Content;
                return result != null ? JsonConvert.DeserializeObject<T>(result) : default(T);
            }
            #region Exception

            catch(Exception ex) {
                Console.WriteLine($"Error requesting {uri}");
                Console.WriteLine(ex);
                return default(T);
            }

            #endregion
        }

        public string Get(string uri) {
            try {
                var client = new RestClient(uri);
                var request = new RestRequest("/", Method.GET);
                request.AddHeader(InstanceHeader, _instance);
                var response = client.Execute(request);
                var result = response.Content;
                return result;
            }
            #region Exception

            catch(Exception ex) {
                Console.WriteLine($"Error requesting {uri}");
                Console.WriteLine(ex);
                return null;
            }

            #endregion
        }

        public T Post<T>(string uri, IDictionary<string, string> data) {
            try {
                var client = new RestClient(uri);
                var request = new RestRequest("/", Method.POST);
                request.AddHeader(InstanceHeader, _instance);
                foreach(var d in data)
                    request.AddParameter(d.Key, d.Value);
                var response = client.Execute(request);
                var result = response.Content;
                return result != null ? JsonConvert.DeserializeObject<T>(result) : default(T);
            }
            #region Exception

            catch(Exception ex) {
                Console.WriteLine($"Error requesting {uri}");
                Console.WriteLine(ex);
                return default(T);
            }

            #endregion
        }

        public HttpStatusCode Post(string uri, IDictionary<string, string> data = null) {
            try {
                var client = new RestClient(uri);
                var request = new RestRequest("/", Method.POST);
                request.AddHeader(InstanceHeader, _instance);
                if(data != null) {
                    foreach(var d in data) {
                        request.AddParameter(d.Key, d.Value);
                    }
                }
                var response = client.Execute(request);
                var code = response.StatusCode;
                return (HttpStatusCode)code;
            }
            #region Exception

            catch(Exception ex) {
                Console.WriteLine($"Error requesting {uri}");
                Console.WriteLine(ex);
                return HttpStatusCode.InternalServerError;
            }

            #endregion
        }

        public void Post2(string uri, IDictionary<string, string> data = null) {
            try {
                var client = new RestClient(uri);
                var request = new RestRequest("/", Method.POST);
                request.AddHeader(InstanceHeader, _instance);
                if(data != null) {
                    foreach(var d in data) {
                        request.AddParameter(d.Key, d.Value);
                    }
                }
                client.Execute(request);
            }
            #region Exception

            catch(Exception ex) {
                Console.WriteLine($"Error requesting {uri}");
                Console.WriteLine(ex);
            }

            #endregion
        }

        public string GetString(string uri) {
            try {
                var client = new RestClient(uri);
                var request = new RestRequest("/", Method.GET);
                request.AddHeader(InstanceHeader, _instance);
                var response = client.Execute(request);
                var result = response.Content;
                return result;
            }
            #region Exception

            catch(Exception ex) {
                Console.WriteLine($"Error requesting {uri}");
                Console.WriteLine(ex);
                return null;
            }

            #endregion
        }

        public void GetFile(string uri, string destination) {
            try {
                var client = new RestClient(uri);
                var request = new RestRequest("/", Method.GET);
                request.AddHeader(InstanceHeader, _instance);
                client.DownloadData(request).SaveAs(destination);
            }
            #region Exception

            catch(Exception ex) {
                Console.WriteLine($"Error requesting {uri}");
                Console.WriteLine(ex);
            }

            #endregion
        }
    }
}