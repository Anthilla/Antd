using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using anthilla.core;

namespace antdsh {
    internal class AntdshApplication {

        private static readonly IDictionary<string, string> CommandList = new Dictionary<string, string>();

        private static bool TrustCertificate(object sender, X509Certificate x509Certificate, X509Chain x509Chain, SslPolicyErrors sslPolicyErrors) {
            return true;
        }

        private static void Main(string[] args) {
            ServicePointManager.ServerCertificateValidationCallback = TrustCertificate;
            if(args.Length < 1) {
                while(true) {
                    Console.Write($"{DateTime.Now:[dd-MM-yyyy] HH:mm} > antdsh > ");
                    var command = Console.ReadLine();
                    if(string.IsNullOrEmpty(command))
                        continue;
                    AddCommand(command);
                    var commandMain = command.Split(' ').First();
                    var arguments = command.Split(' ').Skip(1).ToArray();

                    if(LCommands.ContainsKey(commandMain)) {
                        Action<string[]> functionToExecute;
                        LCommands.TryGetValue(commandMain, out functionToExecute);
                        functionToExecute?.Invoke(arguments);
                    }
                    else {
                        Console.WriteLine("Command '" + commandMain + "' not found");
                    }
                }
            }
            else {
                var commandMain = args.First();
                var arguments = args.Skip(1).ToArray();
                if(LCommands.ContainsKey(commandMain)) {
                    Action<string[]> functionToExecute;
                    LCommands.TryGetValue(commandMain, out functionToExecute);
                    functionToExecute?.Invoke(arguments);
                }
                else {
                    Console.WriteLine("Command '" + commandMain + "' not found");
                }
            }
        }

        private static void AddCommand(string command) {
            if(command == "history")
                return;
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            CommandList.Add(new KeyValuePair<string, string>(timestamp, command));
        }

        private static readonly Dictionary<string, Action<string[]>> LCommands =
            new Dictionary<string, Action<string[]>> {
                { "help", HelpFunc },
                { "history", HistoryFunc },
                { "exit", ExitFunc },
                { "update" , UpdateFunc },
                { "hash" , HashFunc },
                { "start", StartFunc },
                { "stop", StopFunc },
                { "status", StatusFunc },
                { "download", DownloadFunc }
            };

        private static void HelpFunc(string[] args) {
            WriteHelp("help", "show this list");
            WriteHelp("start", "initialize a running version of antd");
            WriteHelp("stop", "stop any running version of antd");
            WriteHelp("restart", "restart antd related systemctl services and mounts");
            WriteHelp("update", "update the selected resource from the public repository, options are: antd,  antdsh, system, kernel");
            WriteHelp("history", "show the commands used in this antdsh session");
            WriteHelp("exit", "exit from antdsh");
            Console.WriteLine("");
        }

        private static void WriteHelp(string command, string description) {
            Console.WriteLine($"{command}:\t\t{description}");
        }

        private static void HistoryFunc(string[] args) {
            foreach(var cmd in CommandList) {
                Console.WriteLine(cmd.Value);
            }
        }

        private static void ExitFunc(string[] args) {
            Console.WriteLine("Goodbye!");
            Thread.Sleep(1000);
            Environment.Exit(0);
        }

        private static readonly Units Units = new Units();

        private static void HashFunc(string[] args) {
            Units.CreateRemountUnits();
            if(args.Length != 1) {
                return;
            }
            var verb = args.FirstOrDefault();
            if(File.Exists(verb)) {
                Console.WriteLine($"{verb} does not exist");
            }
            var h = Update.GetFileHash(verb);
            Console.WriteLine(h);
        }

        public static void DownloadFunc(string[] args) {
            if(args.Length != 2) {
                return;
            }
            var verb = args.FirstOrDefault();
            var filename = args.LastOrDefault();
            var success = new FileDownloader(verb, filename).DownloadFile();
            if(success == false) {
                throw new Exception("Download failed");
            }
        }

        private static void UpdateFunc(string[] args) {
            Units.CreateRemountUnits();
            if(args.Length < 1) {
                WriteHelp("update usage", "update [options] [verb]");
                Console.WriteLine("");
                Console.WriteLine("update options:");
                Console.WriteLine("\t-h\t--help\tshow this message");
                Console.WriteLine("\t-f\t--force\tforce the update");
                Console.WriteLine("\t-u\t--unit\tdownload only the context units");
                Console.WriteLine("");
                Console.WriteLine("update verbs:");
                Console.WriteLine("\tcheck");
                Console.WriteLine("\tall");
                Console.WriteLine("\tantd");
                Console.WriteLine("\tantdsh");
                Console.WriteLine("\taossvc");
                Console.WriteLine("\tsystem");
                Console.WriteLine("\tkernel");
                Console.WriteLine("\txen");
                Console.WriteLine("");
                return;
            }
            if(args.Length > 2) {
                Console.WriteLine("Too many arguments, try 'update --help' to check its usage.");
            }
            if(args.Contains("-h") || args.Contains("--help")) {
                WriteHelp("update usage", "update [options] [verb]");
                Console.WriteLine("");
                Console.WriteLine("update options:");
                Console.WriteLine("\t-h\t--help\tshow this message");
                Console.WriteLine("\t-f\t--force\tforce the update");
                Console.WriteLine("\t-u\t--unit\tdownload only the context units");
                Console.WriteLine("");
                Console.WriteLine("update verbs:");
                Console.WriteLine("\tcheck");
                Console.WriteLine("\tall");
                Console.WriteLine("\tantd");
                Console.WriteLine("\tantdsh");
                Console.WriteLine("\taossvc");
                Console.WriteLine("\tsystem");
                Console.WriteLine("\tkernel");
                Console.WriteLine("\txen");
                Console.WriteLine("");
                return;
            }
            var verb = args.Last();

            var forced = args.Contains("-f") || args.Contains("--force");
            var unitsOnly = args.Contains("-u") || args.Contains("--units");

            switch(verb) {
                case "check":
                    Update.Check();
                    break;
                case "all":
                    Update.Antd(forced, unitsOnly);
                    Update.AntdUi(forced, unitsOnly);
                    Update.Antdsh(forced, unitsOnly);
                    Update.Aossvc(forced, unitsOnly);
                    Update.KernelSystemMap(forced, unitsOnly);
                    Update.KernelFirmware(forced, unitsOnly);
                    Update.KernelInitrd(forced, unitsOnly);
                    Update.KernelKernel(forced, unitsOnly);
                    Update.KernelModules(forced, unitsOnly);
                    Update.System(forced, unitsOnly);
                    break;
                case "antd":
                    Update.Antd(forced, unitsOnly);
                    Update.AntdUi(forced, unitsOnly);
                    break;
                case "antdsh":
                    Update.Antdsh(forced, unitsOnly);
                    break;
                case "aossvc":
                    Update.Aossvc(forced, unitsOnly);
                    break;
                case "system":
                    Update.System(forced, unitsOnly);
                    break;
                case "kernel":
                    Update.KernelSystemMap(forced, unitsOnly);
                    Update.KernelFirmware(forced, unitsOnly);
                    Update.KernelInitrd(forced, unitsOnly);
                    Update.KernelKernel(forced, unitsOnly);
                    Update.KernelModules(forced, unitsOnly);
                    break;
                case "xen":
                    //Update.Xen(forced, unitsOnly);
                    break;
                default:
                    Console.WriteLine("Nothing to update...");
                    break;
            }
        }

        //private static bool IsAntdRunning() => Bash.Execute("ps -aef").Grep("Antd.exe").Any();

        //private static int _startCount;
        private static void StartFunc(string[] args) {
            //var versionToRun = args.First();
            //while(true) {
            //    _startCount++;
            //    Console.WriteLine($"Retry #{_startCount}");
            //    if(_startCount < 5) {
            //        Execute.LinkVersionToRunning(versionToRun);
            //        Console.WriteLine($"New antd '{versionToRun}' linked to running version");
            //        Console.WriteLine("Restarting services now...");
            //        Execute.RestartSystemctlAntdServices();
            //        if(IsAntdRunning()) {
            //            Console.WriteLine("Antd is running now!");
            //        }
            //        else {
            //            Console.WriteLine("Something went wrong starting antd... retrying starting it...");
            //            continue;
            //        }
            //    }
            //    else {
            //        Console.WriteLine("Error: too many retries...");
            //    }
            //    _startCount = 0;
            //    break;
            //}
        }

        //private static int _stopCount;
        private static void StopFunc(string[] args) {
            //while(true) {
            //    _stopCount++;
            //    Console.WriteLine($"Retry #{_stopCount}");
            //    if(_stopCount < 5) {
            //        Console.WriteLine("Removing everything and stopping antd.");
            //        Execute.StopServices();
            //        UmountAll();
            //        if(IsAntdRunning() == false) {
            //            Console.WriteLine("Antd has been stopped now!");
            //        }
            //        else {
            //            Console.WriteLine("Something went wrong stopping antd... retrying stopping it...");
            //            continue;
            //        }
            //    }
            //    else {
            //        Console.WriteLine("Error: too many retries...");
            //    }
            //    _stopCount = 0;
            //    break;
            //}
        }

        //private static void UmountAll() {
        //    Console.WriteLine("Unmounting Antd");
        //    while(true) {
        //        if(File.Exists("/proc/mounts")) {
        //            var procMounts = File.ReadAllLines("/proc/mounts");
        //            if(procMounts.Any(_ => _.Contains("/antd")))
        //                return;
        //            Bash.Execute($"umount {Parameter.AntdCfg}");
        //            Bash.Execute($"umount {Parameter.AntdCfgDatabase}");
        //            Bash.Execute("umount /framework/antd");
        //        }
        //    }
        //}

        private static void StatusFunc(string[] args) {
            var res = Bash.Execute("ps -aef").Grep("Antd.exe");
            Console.WriteLine(res.Any() ? "Antd is running." : "Antd is NOT running");
            Console.WriteLine("");
            Console.WriteLine(Bash.Execute("systemctl status "));
        }
    }
}