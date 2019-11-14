using Antd2.cmds;
using System;
using System.Collections.Generic;

namespace Antd2 {
    public class CheckCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                {"os", Os },
                {"internet", Internet },
                {"dns", Dns },
            };

        private static void Os(string[] args) {
            Console.WriteLine("  os:");
            Console.WriteLine($"  {Uname.Get()}");
        }

        private static void Internet(string[] args) {
            var destination = args.Length > 0 ? args[0] : "8.8.8.8";
            Console.WriteLine($"  test ping: {destination}");
            foreach (var line in Ping.Get(destination))
                Console.WriteLine($"  {line}");
        }

        private static void Dns(string[] args) {
            var destination = args.Length > 0 ? args[0] : "google.com";
            Console.WriteLine($"  test ping: {destination}");
            foreach (var line in Ping.Get(destination))
                Console.WriteLine($"  {line}");
        }
    }
}