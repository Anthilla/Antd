using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace antdsh {
    public class ApiConsumer {

        private static string _instance;
        private const string InstanceHeader = "instance-guid";

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
            catch (Exception ex) {
#if DEBUG
                Console.WriteLine("----------------------------------------");
                Console.WriteLine(uri);
                Console.WriteLine(ex);
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("");
#endif
                return default(T);
            }
            #endregion
        }

        public void Get(string uri) {
            try {
                var client = new RestClient(uri);
                var request = new RestRequest("/", Method.GET);
                request.AddHeader(InstanceHeader, _instance);
                client.Execute(request);
            }
            #region Exception
            catch (Exception ex) {
#if DEBUG
                Console.WriteLine("----------------------------------------");
                Console.WriteLine(uri);
                Console.WriteLine(ex);
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("");
#endif
            }
            #endregion
        }

        public T Post<T>(string uri, IDictionary<string, string> data) {
            try {
                var client = new RestClient(uri);
                var request = new RestRequest("/", Method.POST);
                request.AddHeader(InstanceHeader, _instance);
                foreach (var d in data) {
                    request.AddParameter(d.Key, d.Value);
                }
                var response = client.Execute(request);
                var result = response.Content;
                return result != null ? JsonConvert.DeserializeObject<T>(result) : default(T);
            }
            #region Exception
            catch (Exception ex) {
#if DEBUG
                Console.WriteLine("----------------------------------------");
                Console.WriteLine(uri);
                Console.WriteLine(ex);
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("");
#endif
                return default(T);
            }
            #endregion
        }

        public HttpStatusCode Post(string uri, IDictionary<string, string> data) {
            try {
                var client = new RestClient(uri);
                var request = new RestRequest("/", Method.POST);
                request.AddHeader(InstanceHeader, _instance);
                foreach (var d in data) {
                    request.AddParameter(d.Key, d.Value);
                }
                var response = client.Execute(request);
                var code = response.StatusCode;
                return code;
            }
            #region Exception
            catch (Exception ex) {
#if DEBUG
                Console.WriteLine("----------------------------------------");
                Console.WriteLine(uri);
                Console.WriteLine(ex);
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("");
#endif
                return HttpStatusCode.InternalServerError;
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
            catch (Exception ex) {
#if DEBUG
                Console.WriteLine("----------------------------------------");
                Console.WriteLine(uri);
                Console.WriteLine(ex);
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("");
#endif
                return null;
            }
            #endregion
        }

        public FileInfo GetFile(string uri, string destination) {
            try {
                //var client = new RestClient(uri);
                //var request = new RestRequest("/", Method.GET);
                //request.AddHeader(InstanceHeader, _instance);
                //client.DownloadData(request).SaveAs(destination);
                //var finfo = new FileInfo(destination);
                //return finfo;
                using (var writer = File.OpenWrite(destination)) {
                    var client = new RestClient(uri);
                    var request = new RestRequest("/", Method.GET) {
                        ResponseWriter = responseStream => responseStream.CopyTo(writer)
                    };
                    client.DownloadData(request);
                    var finfo = new FileInfo(destination);
                    return finfo;
                }
            }
            #region Exception
            catch (Exception ex) {
#if DEBUG
                Console.WriteLine("----------------------------------------");
                Console.WriteLine(uri);
                Console.WriteLine(ex);
                Console.WriteLine("----------------------------------------");
                Console.WriteLine("");
#endif
                return null;
            }
            #endregion
        }
    }
}
