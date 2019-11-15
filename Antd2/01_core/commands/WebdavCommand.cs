using Antd2.Storage;
using System;
using System.Collections.Generic;

namespace Antd2 {
    public class WebdavCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "start", StartFunc },
            };

        public static void StartFunc(string[] args) {
            var wd = new WebDavServer("antd", "0.0.0.0", 1234);
            wd.Start();
        }
    }
}