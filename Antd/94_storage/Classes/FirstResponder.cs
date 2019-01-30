using anthilla.core;
using RestWrapper;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Kvpbase {
    public class FirstResponder {
        #region Public-Members

        #endregion

        #region Constructors-and-Factories

        #endregion

        #region Public-Static-Methods

        public static RestResponse SendRequest(
            Settings settings,
            RequestMetadata md,
            List<string> urls,
            string encoding,
            string method,
            string user,
            string pass,
            bool encodeCredentials,
            Dictionary<string, string> headers,
            byte[] body) {
            #region Check-for-Null-Values

            if(md == null) {
                ConsoleLogger.Warn("SendRequest null request metadata detected");
                return null;
            }

            if(urls == null || urls.Count < 1) {
                ConsoleLogger.Warn("SendRequest null or empty URL list detected");
                return null;
            }

            #endregion

            #region Variables

            DateTime startTime = DateTime.Now;

            #endregion

            #region Process

            md.FirstResponse = null;
            md.FirstResponseUrl = null;
            md.FirstResponseLock = new object();

            List<Thread> threads = new List<Thread>();

            ConsoleLogger.Warn("SendRequest received " + urls.Count + " URLs to try");
            foreach(string url in urls) {
                ConsoleLogger.Warn("SendRequest starting thread to " + url);
                Thread t = new Thread(() => SendRequestThread(settings, ref md, url, encoding, method, user, pass, encodeCredentials, headers, body));
                threads.Add(t);
                t.Start();
                // Thread.Sleep(1000);
            }

            bool respReceived = false;

            while(!respReceived) {
                if(md.FirstResponse != null) {
                    ConsoleLogger.Warn("SendRequest first response received from: " + md.FirstResponseUrl);
                    if(
                        (md.FirstResponse.StatusCode == 200) ||
                        (md.FirstResponse.StatusCode == 201)
                        ) {
                        respReceived = true;
                    }
                    else {
                        // may have received a failure response from an active node, wait until another
                        // responds to potentially get a success response
                    }
                }

                bool threadsFinished = true;
                if(!respReceived) {
                    foreach(Thread currThread in threads) {
                        if(currThread.IsAlive) {
                            threadsFinished = false;
                            break;
                        }
                    }
                }

                if(threadsFinished) {
                    respReceived = true;
                }
            }

            if(md.FirstResponse == null)
                ConsoleLogger.Warn("SendRequest first response null after iterating through list of " + urls.Count + " URLs (" + Common.TotalMsFrom(startTime) + "ms)");
            else
                ConsoleLogger.Warn("SendRequest response received " + md.FirstResponse.StatusCode + " " + md.FirstResponse.StatusDescription + " from list of " + urls.Count + " URLs (" + Common.TotalMsFrom(startTime) + "ms)");

            return md.FirstResponse;

            #endregion
        }

        private static void SendRequestThread(
            Settings settings,
            ref RequestMetadata md,
            string url,
            string encoding,
            string method,
            string user,
            string pass,
            bool encodeCredentials,
            Dictionary<string, string> headers,
            byte[] body) {
            DateTime startTime = DateTime.Now;

            RestResponse resp = RestRequest.SendRequestSafe(
                url, encoding, method, user, pass, encodeCredentials,
                Common.IsTrue(settings.Rest.AcceptInvalidCerts),
                headers, body);

            lock(md.FirstResponseLock) {
                if(md.FirstResponse == null) {
                    md.FirstResponse = resp;
                    md.FirstResponseUrl = url;

                    if(md.FirstResponse.Data != null && md.FirstResponse.Data.Length > 0) {
                        ConsoleLogger.Warn("SendRequestThread END " + method + " " + url + " completed " + Common.TotalMsFrom(startTime) + "ms, response: " + resp.ContentLength + "B " + resp.ContentType + " status " + resp.StatusCode + " " + resp.StatusDescription);
                    }
                    else {
                        ConsoleLogger.Warn("SendRequestThread END " + method + " " + url + " completed " + Common.TotalMsFrom(startTime) + "ms, response: [no data] " + resp.ContentType + " status " + resp.StatusCode + " " + resp.StatusDescription);
                    }
                }
            }

            return;
        }

        #endregion
    }
}