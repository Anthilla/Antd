using Antd2.Init;
using System;
using System.Collections.Generic;

namespace Antd2 {
    public class InitCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "start", StartFunc },
                { "stop", StopFunc },
            };

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
    }
}