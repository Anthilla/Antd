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
            if (args.Length < 5) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var ip = args[0];
            var port = int.Parse(args[1]);
            var root = args[2];
            var user = args[3];
            var password = args[4];
            var wd = new WebDavServer($"antd_{root.Replace("/", "_")}", ip, port, root, user, password);
            wd.Start();
        }
    }
}