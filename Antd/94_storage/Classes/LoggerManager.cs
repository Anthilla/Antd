using anthilla.core;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Kvpbase {
    public class LoggerManager {
        #region Public-Members

        #endregion

        #region Private-Members

        private ConcurrentQueue<Tuple<string, string>> LoggerQueue;

        #endregion

        #region Constructors-and-Factories

        public LoggerManager() {

        }

        public LoggerManager(Settings settings) {
            if(settings == null)
                throw new ArgumentNullException(nameof(settings));
            LoggerQueue = new ConcurrentQueue<Tuple<string, string>>();

            Task.Run(() => LoggerWorker(settings, LoggerQueue));
        }

        #endregion

        #region Public-Methods

        public void Add(string logfile, string contents) {
            DateTime utc = DateTime.Now.ToUniversalTime();
            string datestr = utc.Date.ToString("MMddyyyy");
            string timestr = utc.TimeOfDay.ToString("hhmmss");

            contents = datestr + "," + timestr + "," + contents;
            LoggerQueue.Enqueue(new Tuple<string, string>(logfile, contents));
        }

        #endregion

        #region Private-Methods

        private void LoggerWorker(Settings settings, ConcurrentQueue<Tuple<string, string>> queue) {
            #region Setup

            if(settings.Topology.RefreshSec <= 0) {
                ConsoleLogger.Warn("LoggerWorker setting topology refresh timer to 10 sec (config value too low: " + settings.Topology.RefreshSec + " sec)");
                settings.Topology.RefreshSec = 10;
            }

            ConsoleLogger.Warn("LoggerWorker starting with topology refresh timer set to " + settings.Topology.RefreshSec + " sec");

            if(settings.Logger == null) {
                ConsoleLogger.Warn("LoggerWorker null logger section in config file, exiting");
                return;
            }

            if(settings.Logger.RefreshSec < 10) {
                ConsoleLogger.Warn("LoggerWorker invalid value for refresh interval, using default of 10");
                settings.Topology.RefreshSec = 10;
            }

            #endregion

            #region Process

            bool firstRun = true;
            while(true) {
                #region Wait

                if(!firstRun) {
                    Thread.Sleep(settings.Topology.RefreshSec * 1000);
                }
                else {
                    firstRun = false;
                }

                #endregion

                #region Process

                if(queue != null) {
                    Tuple<string, string> message;
                    while(queue.TryDequeue(out message)) {
                        string logfile = String.Copy(message.Item1);
                        string contents = String.Copy(message.Item2);

                        if(!Common.WriteFile(logfile, contents, true)) {
                            ConsoleLogger.Warn("LoggerWorker unable to append the following message to " + logfile);
                            ConsoleLogger.Warn(contents);
                        }
                    }
                }

                #endregion
            }

            #endregion
        }

        #endregion

        #region Public-Static-Methods

        public static string BuildMessage(RequestMetadata md, string operation, string text) {
            // src_ip,UserMasterId,user_guid,ApiKeyId,api_key_guid,operation(RCD)

            string ret = "";

            if(md != null) {
                if(md.CurrentHttpRequest != null) {
                    ret += md.CurrentHttpRequest.SourceIp + ",";
                }
                else {
                    ret += "0.0.0.0,";
                }
            }
            else {
                ret += "0.0.0.0,";
            }

            if(md.CurrentUserMaster != null) {
                ret += md.CurrentUserMaster.UserMasterId + "," + md.CurrentUserMaster.Guid + ",";
            }
            else {
                ret += "0,,";
            }

            if(md.CurrentApiKey != null) {
                ret += md.CurrentApiKey.ApiKeyId + "," + md.CurrentApiKey.Guid + ",";
            }
            else {
                ret += "0,,";
            }

            ret += operation + "," + text;
            return ret;
        }

        #endregion
    }
}