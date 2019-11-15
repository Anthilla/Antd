using Antd2.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Systemctl = Antd2.cmds.Systemctl;

namespace Antd2 {
    public class ServiceCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "list", ListFunc },
                { "status", StatusFunc },
                { "start", StartFunc },
                { "stop", StopFunc },
                { "disable", DisableFunc },
                { "enable", EnableFunc },
                { "mask", MaskFunc },
                { "import", ImportFunc },
            };

        public static void ListFunc(string[] args) {
            var services = Systemctl.Get(models.SystemctlType.Service);
            foreach (var service in services) {
                Console.Write($"  {service.Service}\t");
                ListFunc_PrintEnabled(service.Active);
                ListFunc_PrintActive(service.Start);
                Console.WriteLine();
            }
        }

        private static void ListFunc_PrintEnabled(bool enabled) {
            if (enabled) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("enabled\t");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("disabled\t");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        private static void ListFunc_PrintActive(bool enabled) {
            if (enabled) {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("active\t");
                Console.ForegroundColor = ConsoleColor.White;
            }
            else {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("inactive\t");
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        public static void StatusFunc(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var service = args[0];
            foreach (var l in Systemctl.Status(service))
                Console.WriteLine(l);
        }

        public static void StartFunc(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var service = args[0];
            Console.WriteLine($"  start {service}");
            Systemctl.Start(service);
        }

        public static void StopFunc(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var service = args[0];
            Console.WriteLine($"  stop {service}");
            Systemctl.Stop(service);
        }

        public static void DisableFunc(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var service = args[0];
            Console.WriteLine($"  disable {service}");
            Systemctl.Disable(service);
        }

        public static void EnableFunc(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var service = args[0];
            Console.WriteLine($"  enable {service}");
            Systemctl.Enable(service);
        }

        public static void MaskFunc(string[] args) {
            if (args.Length < 1) {
                Console.WriteLine("  not enough arguments");
                return;
            }
            var service = args[0];
            Console.WriteLine($"  mask {service}");
            Systemctl.Mask(service);
        }

        public static void ImportFunc(string[] args) {
            Console.WriteLine("  Importing current service status...");
            var services = Systemctl.Get(models.SystemctlType.Service);

            var activeService = services.Where(_ => _.Start == true).ToArray();
            Console.WriteLine($"  Currently {activeService.Length} active services are found");

            Console.Write("  Do you want to import these services in ? (Y/n)");
            if (Console.ReadLine() != "Y")
                return;

            ConfigManager.Config.Saved.Boot.ActiveServices = activeService.Select(_ => _.Service).ToArray();
            ConfigManager.Config.Dump();
            Console.WriteLine($"  {activeService.Length} active services imported in the current antd configuration!");
        }


    }
}