using System.Collections.Generic;

namespace Antd.Websocket.Connections {
    public class MimeTypes : Dictionary<string, string> {
        private static readonly object Locker = new object();
        private static MimeTypes _instance;

        private MimeTypes() {
            Add(".html", "text/html");
            Add(".html", "text/html");
            Add(".css", "text/css");
            Add(".ico", "image/x-icon");
            Add(".jpg", "image/jpeg");
            Add(".jpeg", "image/jpeg");
            Add(".bmp", "image/bmp");
            Add(".png", "image/png");
            Add(".js", "text/javascript");
            Add(".map", "application/json");
        }

        public static MimeTypes Instance {
            get {
                if (_instance != null) return _instance;
                lock (Locker) {
                    _instance = new MimeTypes();
                }
                return _instance;
            }
        }
    }
}
