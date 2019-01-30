using anthilla.core;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Kvpbase {
    public class PublicObjThread {
        public PublicObjThread(Settings settings) {
            if(settings == null)
                throw new ArgumentNullException(nameof(settings));
            Task.Run(() => PublicObjWorker(settings));
        }

        private void PublicObjWorker(Settings settings) {
            try {
                if(settings.PublicObj.RefreshSec <= 0) {
                    ConsoleLogger.Warn("PublicObjWorker setting expiration processing timer to 60 sec (config value too low: " + settings.PublicObj.RefreshSec + " sec)");
                    settings.PublicObj.RefreshSec = 60;
                }

                ConsoleLogger.Warn("PublicObjWorker starting with expiration processing timer set to " + settings.PublicObj.RefreshSec + " sec");

                bool firstRun = true;
                while(true) {
                    #region Wait

                    if(!firstRun) {
                        Thread.Sleep(settings.PublicObj.RefreshSec * 1000);
                    }
                    else {
                        firstRun = false;
                    }

                    #endregion

                    #region Ensure-Directory-Exists

                    if(!Common.DirectoryExists(settings.PublicObj.Directory)) {
                        if(!Common.CreateDirectory(settings.PublicObj.Directory)) {
                            ConsoleLogger.Warn("PublicObjWorker unable to create missing directory " + settings.PublicObj.Directory);
                            continue;
                        }
                    }

                    #endregion

                    #region Retrieve-File-List

                    string[] files = Directory.GetFiles(settings.PublicObj.Directory);
                    if(files == null || files.Length < 1) {
                        // ConsoleLogger.Warn( "PublicObjWorker no files found to process for expiration");
                        continue;
                    }

                    #endregion

                    #region Retrieve-Metadata

                    foreach(string curr in files) {
                        DateTime created = File.GetCreationTime(curr);
                        DateTime compare = created.AddSeconds(settings.PublicObj.DefaultExpirationSec);
                        if(!Common.IsLaterThanNow(compare)) {
                            #region File-Expired

                            try {
                                File.Delete(curr);
                                ConsoleLogger.Warn("PublicObjWorker successfully cleaned up expired metadata file " + curr + " (created " + created.ToString("MM/dd/yyyy HH:mm:ss") + " expired " + compare.ToString("MM/dd/yyyy HH:mm:ss"));
                            }
                            catch(Exception) {
                                ConsoleLogger.Warn("PublicObjWorker unable to delete expired metadata file " + curr);
                            }

                            #endregion
                        }
                    }

                    #endregion
                }
            }
            catch(Exception e) {
                ConsoleLogger.Error("PublicObjWorker Outer exception" + e.ToString());
                Common.ExitApplication("PublicObjWorker", "Outer exception", -1);
                return;
            }
        }
    }
}