using System;
using System.Collections.Generic;

#if NETCOREAPP
using Antd2.Init;
#endif

namespace Antd2 {
    public class InitCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "start", StartFunc },
                { "stop", StopFunc },
            };

#if NETCOREAPP
        private static ServiceInit ServiceInit = null;

        public static void StartFunc(string[] args) {
            if (ServiceInit == null) {
                ServiceInit = new ServiceInit();
            }
            ServiceInit.Start();
        }

        public static void StopFunc(string[] args) {
            if (ServiceInit == null) {
                ServiceInit.Stop();
            }
        }
#endif

#if NETFRAMEWORK

        public static void StartFunc(string[] args) {
            Console.WriteLine("  not supported by .net framework");
        }

        public static void StopFunc(string[] args) {
            Console.WriteLine("  not supported by .net framework");
        }
#endif

    }
}