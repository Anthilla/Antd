using System;
using System.Collections.Generic;
using Nancy;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Extensions;

namespace antd.core {
    public class ApiConsumer {

        public static T Get<T>(string uri) {
            var client = new RestClient(uri);
            var request = new RestRequest("/", Method.GET);
            request.AddHeader("session-instance-guid", Guid.NewGuid().ToString());
            var response = client.Execute(request);
            var result = response.Content;
            try {
                return result != null ? JsonConvert.DeserializeObject<T>(result) : default(T);
            }
            catch(System.Exception) {
                return default(T);
            }
        }

        public static HttpStatusCode Get(string uri) {
            var client = new RestClient(uri);
            var request = new RestRequest("/", Method.GET);
            request.AddHeader("session-instance-guid", Guid.NewGuid().ToString());
            var response = client.Execute(request);
            return (HttpStatusCode)response.StatusCode;
        }

        public static T Post<T>(string uri) {
            var client = new RestClient(uri);
            var request = new RestRequest("/", Method.POST);
            request.AddHeader("session-instance-guid", Guid.NewGuid().ToString());
            var response = client.Execute(request);
            var result = response.Content;
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                return default(T);
            }
            return result != null ? JsonConvert.DeserializeObject<T>(result) : default(T);
        }

        public static T Post<T>(string uri, IDictionary<string, string> data) {
            var client = new RestClient(uri);
            var request = new RestRequest("/", Method.POST);
            request.AddHeader("session-instance-guid", Guid.NewGuid().ToString());
            foreach(var d in data)
                request.AddParameter(d.Key, d.Value);
            var response = client.Execute(request);
            var result = response.Content;
            if(response.StatusCode != System.Net.HttpStatusCode.OK) {
                return default(T);
            }
            return result != null ? JsonConvert.DeserializeObject<T>(result) : default(T);
        }

        public static HttpStatusCode Post(string uri) {
            var client = new RestClient(uri);
            var request = new RestRequest("/", Method.POST);
            request.AddHeader("session-instance-guid", Guid.NewGuid().ToString());
            var response = client.Execute(request);
            var code = response.StatusCode;
            return (HttpStatusCode)code;
        }

        public static HttpStatusCode Post(string uri, IDictionary<string, string> data = null) {
            var client = new RestClient(uri);
            var request = new RestRequest("/", Method.POST);
            request.AddHeader("session-instance-guid", Guid.NewGuid().ToString());
            if(data != null) {
                foreach(var d in data) {
                    request.AddParameter(d.Key, d.Value);
                }
            }
            var response = client.Execute(request);
            var code = response.StatusCode;
            return (HttpStatusCode)code;
        }

        public static HttpStatusCode Post(string uri, IDictionary<string, string> data = null, int timeout = 100) {
            var client = new RestClient(uri) {
                Timeout = timeout
            };
            var request = new RestRequest("/", Method.POST);
            request.AddHeader("session-instance-guid", Guid.NewGuid().ToString());
            if(data != null) {
                foreach(var d in data) {
                    request.AddParameter(d.Key, d.Value);
                }
            }
            var response = client.Execute(request);
            var code = response.StatusCode;
            return (HttpStatusCode)code;
        }

        public static string PostAndGetJson(string uri, IDictionary<string, string> data = null) {
            var client = new RestClient(uri);
            var request = new RestRequest("/", Method.POST);
            request.AddHeader("session-instance-guid", Guid.NewGuid().ToString());
            if(data != null) {
                foreach(var d in data) {
                    request.AddParameter(d.Key, d.Value);
                }
            }
            request.Timeout = int.MaxValue;
            var response = client.Execute(request);
            return response.Content;
        }

        public static void Post2(string uri, IDictionary<string, string> data = null) {
            var client = new RestClient(uri);
            var request = new RestRequest("/", Method.POST);
            request.AddHeader("session-instance-guid", Guid.NewGuid().ToString());
            if(data != null) {
                foreach(var d in data) {
                    request.AddParameter(d.Key, d.Value);
                }
            }
            client.Execute(request);
        }

        public static string GetString(string uri) {
            var client = new RestClient(uri);
            var request = new RestRequest("/", Method.GET);
            request.AddHeader("session-instance-guid", Guid.NewGuid().ToString());
            var response = client.Execute(request);
            var result = response.Content;
            return result;
        }

        public static System.Tuple<HttpStatusCode, string> GetStringWithStatus(string uri) {
            var client = new RestClient(uri);
            var request = new RestRequest("/", Method.GET);
            request.AddHeader("session-instance-guid", Guid.NewGuid().ToString());
            var response = client.Execute(request);
            var result = response.Content;
            var status = response.StatusCode;
            return new System.Tuple<HttpStatusCode, string>((HttpStatusCode)status, result);
        }

        public static string GetJson(string uri) {
            var client = new RestClient(uri);
            var request = new RestRequest("/", Method.GET);
            request.AddHeader("session-instance-guid", Guid.NewGuid().ToString());
            request.Timeout = int.MaxValue;
            var response = client.Execute(request);
            return response.Content;
        }

        public static void GetFile(string uri, string destination) {
            var client = new RestClient(uri);
            var request = new RestRequest("/", Method.GET);
            request.AddHeader("session-instance-guid", Guid.NewGuid().ToString());
            client.DownloadData(request).SaveAs(destination);
        }

        public static void Send(string uri, IDictionary<string, string> data = null, int timeout = 100) {
            var client = new RestClient(uri) {
                Timeout = timeout
            };
            var request = new RestRequest("/", Method.POST);
            request.AddHeader("session-instance-guid", Guid.NewGuid().ToString());
            if(data != null) {
                foreach(var d in data) {
                    request.AddParameter(d.Key, d.Value);
                }
            }
            client.Execute(request);
        }

        public static RestRequest UploadRequest(string uri, IDictionary<string, string> data) {
            var request = new RestRequest(uri, Method.POST);
            foreach(var d in data)
                request.AddParameter(d.Key, d.Value);
            return request;
        }

        public static RestRequest UploadRequest(string uri) {
            var request = new RestRequest(uri, Method.POST);
            return request;
        }

        public static RestRequest DownloadRequest(string uri) {
            var request = new RestRequest(uri, Method.GET);
            return request;
        }
    }
}