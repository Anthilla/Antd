using anthilla.core;
using RestWrapper;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Kvpbase {
    public class MessengerThread {
        public MessengerThread(Settings settings, Topology topology, Node self) {
            if(settings == null)
                throw new ArgumentNullException(nameof(settings));
            if(self == null)
                throw new ArgumentNullException(nameof(self));
            Task.Run(() => MessengerWorker(settings, topology, self));
        }

        private void MessengerWorker(Settings settings, Topology topology, Node self) {
            #region Setup

            if(topology == null || topology.IsEmpty()) {
                ConsoleLogger.Warn("MessengerWorker exiting, no topology");
                return;
            }

            if(String.IsNullOrEmpty(settings.Messages.Directory)) {
                ConsoleLogger.Warn("MessengerWorker unable to open message queue directory from configuration file");
                Common.ExitApplication("MessengerWorker", "Undefined message queue directory", -1);
                return;
            }

            if(!Common.VerifyDirectoryAccess(settings.Environment, settings.Messages.Directory)) {
                ConsoleLogger.Warn("MessengerWorker unable to access message queue directory " + settings.Messages.Directory);
                Common.ExitApplication("MessengerWorker", "Unable to access message queue directory", -1);
                return;
            }

            if(settings.Messages.RefreshSec <= 0) {
                ConsoleLogger.Warn("MessengerWorker setting message queue retry timer to 10 sec (config value too low: " + settings.Messages.RefreshSec + " sec)");
                settings.Messages.RefreshSec = 10;
            }

            ConsoleLogger.Warn("MessengerWorker starting with message queue retry timer set to " + settings.Messages.RefreshSec + " sec");

            #endregion

            #region Process

            bool firstRun = true;
            while(true) {
                #region Wait

                if(!firstRun) {
                    Thread.Sleep(settings.Messages.RefreshSec * 1000);
                }
                else {
                    firstRun = false;
                }

                #endregion

                #region Get-Subdirectory-List

                List<string> subdirectories = new List<string>();
                subdirectories = Common.GetSubdirectoryList(settings.Messages.Directory, true);

                if(subdirectories == null || subdirectories.Count < 1)
                    continue;

                #endregion

                #region Process-Each-Subdirectory

                foreach(string subdirectory in subdirectories) {
                    #region Enumerate

                    // ConsoleLogger.Warn( "MessengerWorker processing directory " + subdirectory);

                    #endregion

                    #region Get-File-List

                    List<string> files = Common.GetFileList(settings.Environment, subdirectory, false);
                    if(files == null)
                        continue;
                    if(files.Count < 1)
                        continue;

                    #endregion

                    #region Process-Each-File

                    foreach(string file in files) {
                        #region Enumerate

                        // ConsoleLogger.Warn( "MessengerWorker processing file " + subdirectory + file);

                        #endregion

                        #region Read-File

                        string contents = Common.ReadTextFile(subdirectory + Common.GetPathSeparator(settings.Environment) + file);
                        if(String.IsNullOrEmpty(contents)) {
                            ConsoleLogger.Warn("MessengerWorker empty file detected at " + subdirectory + Common.GetPathSeparator(settings.Environment) + file + ", deleting");
                            if(!Common.DeleteFile(subdirectory + file)) {
                                ConsoleLogger.Warn("MessengerWorker unable to delete file " + subdirectory + Common.GetPathSeparator(settings.Environment) + file);
                                continue;
                            }
                        }

                        #endregion

                        #region Deserialize

                        Message currMessage = null;
                        try {
                            currMessage = Common.DeserializeJson<Message>(contents);
                            if(currMessage == null) {
                                ConsoleLogger.Warn("MessengerWorker unable to deserialize file " + subdirectory + file + ", skipping");
                                continue;
                            }
                        }
                        catch(Exception) {
                            ConsoleLogger.Warn("MessengerWorker unable to deserialize file " + subdirectory + file + ", skipping");
                            continue;
                        }

                        #endregion

                        #region Set-Message-Parameters

                        currMessage.From = self;

                        string url = "";
                        if(Common.IsTrue(currMessage.To.Ssl)) {
                            url = "https://" + currMessage.To.DnsHostname + ":" + currMessage.To.Port + "/admin/message";
                        }
                        else {
                            url = "http://" + currMessage.To.DnsHostname + ":" + currMessage.To.Port + "/admin/message";
                        }

                        #endregion

                        #region Send-Message

                        string req = Common.SerializeJson(currMessage);
                        RestWrapper.RestResponse resp = RestRequest.SendRequestSafe(
                            url, "application/json", "POST", null, null, false,
                            Common.IsTrue(settings.Rest.AcceptInvalidCerts),
                            Common.AddToDictionary(settings.Server.HeaderApiKey, settings.Server.AdminApiKey, null),
                            Encoding.UTF8.GetBytes(req));

                        if(resp == null) {
                            #region No-REST-Response

                            ConsoleLogger.Warn("MessengerWorker null response connecting to " + url + ", message " + subdirectory + Common.GetPathSeparator(settings.Environment) + file + " will remain queued");
                            continue;

                            #endregion
                        }
                        else {
                            if(resp.StatusCode != 200) {
                                #region Failed-Message

                                ConsoleLogger.Warn("MessengerWorker non-200 response connecting to " + url + ", message " + subdirectory + file + " will remain queued");
                                continue;

                                #endregion
                            }
                            else {
                                #region Successful-Message

                                ConsoleLogger.Warn("MessengerWorker successfully sent message " + subdirectory + Common.GetPathSeparator(settings.Environment) + file);

                                if(!Common.DeleteFile(subdirectory + Common.GetPathSeparator(settings.Environment) + file)) {
                                    ConsoleLogger.Warn("MessengerWorker unable to delete file " + subdirectory + Common.GetPathSeparator(settings.Environment) + file);
                                }
                                else {
                                    ConsoleLogger.Warn("MessengerWorker successfully deleted file " + subdirectory + Common.GetPathSeparator(settings.Environment) + file);
                                }

                                continue;

                                #endregion
                            }
                        }

                        #endregion
                    }

                    #endregion
                }

                #endregion
            }

            #endregion
        }
    }
}