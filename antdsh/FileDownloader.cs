using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;

namespace antdsh {
    public class FileDownloader {
        private readonly string _url;
        private readonly string _fullPathWhereToSave;
        private bool _result;
        //private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(0);

        public FileDownloader(string url, string fullPathWhereToSave) {
            if(string.IsNullOrEmpty(url))
                throw new ArgumentNullException(nameof(url));
            if(string.IsNullOrEmpty(fullPathWhereToSave))
                throw new ArgumentNullException(nameof(fullPathWhereToSave));

            _url = url;
            _fullPathWhereToSave = fullPathWhereToSave;
        }

        private bool StartDownload(int timeout) {
            try {
                var filename = Path.GetFileName(_fullPathWhereToSave);
                var dirName = Path.GetDirectoryName(_fullPathWhereToSave);
                if(dirName == null) {
                    throw new NullReferenceException(nameof(dirName));
                }
                Directory.CreateDirectory(dirName);
                if(File.Exists(_fullPathWhereToSave)) {
                    File.Delete(_fullPathWhereToSave);
                }
                using(var client = new WebClient()) {
                    client.Proxy = null;
                    // client.Credentials = new NetworkCredential("username", "password");
                    client.DownloadProgressChanged += WebClientDownloadProgressChanged;
                    client.DownloadFileCompleted += WebClientDownloadCompleted;
                    Console.WriteLine($"Downloading file {filename}:");
                    client.DownloadFileAsync(new Uri(_url), _fullPathWhereToSave);
                    //_semaphore.Wait(timeout);
                    return _result && File.Exists(_fullPathWhereToSave);
                }
            }
            catch(Exception e) {
                Console.WriteLine("Was not able to download file!");
                Console.Write(e);
                return false;
            }
            finally {
                //_semaphore.Dispose();
            }
        }

        private static void WebClientDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e) {
            Console.Write($"\r\t\t-->\t\t{e.ProgressPercentage}%.");
        }

        private void WebClientDownloadCompleted(object sender, AsyncCompletedEventArgs args) {
            _result = !args.Cancelled;
            if(!_result) {
                Console.Write(args.Error.ToString());
            }
            Console.WriteLine("Download finished!");
            //_semaphore.Release();
        }

        public bool DownloadFile() {
            return StartDownload(1000 * 60 * 60);
        }
    }
}
