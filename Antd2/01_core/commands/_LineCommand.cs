﻿using anthilla.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using static Antd.Help;

namespace Antd {

    public class LineCommand {

        public static readonly Dictionary<string, Action<string[]>> Options = new Dictionary<string, Action<string[]>> {
            { "help", HelpFunc },
            { "exit", ExitFunc },
            { "quit", ExitFunc },
            { "start", StartFunc },
            { "conf", ConfFunc },
            { "default", DefaultFunc },
            { "packages", PackagesFunc },
            { "user", UserFunc },
            { "sysctl", SysctlFunc },
            { "nsswitch", NsswitchFunc },
            { "proc", ProcFunc },
            { "fd", OpenFilesFunc },
            { "netstat", NetstatFunc },
            { "init", InitFunc },
            { "unit", UnitFunc },
        };

        private static void HelpFunc(string[] args) {
            Console.WriteLine("  TOML:");
            Console.WriteLine("  -----");
            Console.WriteLine("  La documentazione sul formato si trova al seguente link:");
            Console.WriteLine("  https://github.com/toml-lang/toml");
            Console.WriteLine();

            Console.WriteLine("  Comandi:");
            Console.WriteLine("  --------");
            PrintHelp("help", new[] { "Mostra questo help" });
            PrintHelp("exit, quit", new[] { "Esce dall'applicazione" });
            Console.WriteLine();

            PrintHelp("start", new[] { "Avvia la procedura di configurazione di Antd utilizzando i parametri configurati",
                                        "Il path di default del file di configurazione è /cfg/antd/antd.toml"});
            PrintHelp("start <file>", new[] { "Può accettare come argomento il path del file di configurazione" });
            Console.WriteLine();

            PrintHelp("conf test", new[] { "Esegue un test di scrittura E lettura del file di configurazione" });
            PrintHelp("conf write", new[] { "Esegue un test di scrittura del file di configurazione" });
            PrintHelp("conf read", new[] { "Esegue un test di lettura del file di configurazione" });
            Console.WriteLine();

            PrintHelp("user check", new[] { "Verifica se utenti e gruppi di sistema di default sono presenti" });
            PrintHelp("user add", new[] { "Crea e configura utenti e gruppi di sistema di default" });
            Console.WriteLine();

            PrintHelp("sysctl check", new[] { "Verifica se i parametri di sysctl corrispondono a quelli di default" });
            PrintHelp("sysctl set", new[] { "Applica i parametri di sysctl di default" });
            PrintHelp("sysctl write", new[] { "Scrive su /etc/sysctl.conf i parametri di default" });
            Console.WriteLine();
        }

        private static void PrintHelp(string command, IEnumerable<string> description) {
            Console.WriteLine($"  {command}");
            foreach (var line in description) {
                Console.WriteLine($"    {line}");
            }
        }

        private static void ExitFunc(string[] args) {
            ConsoleLogger.Log("Exiting antd2.");
            Thread.Sleep(500);
            Environment.Exit(0);
        }

        private static void StartFunc(string[] args) {
            ConsoleLogger.Log("Start antd2.");
            StartCommand.Start(args);
        }

        private static void ConfFunc(string[] args) {
            var line = args.Length > 0 ? args : ReadLine();
            if (ConfCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                functionToExecute?.Invoke(line.Skip(1).ToArray());
            }
            else {
                ConsoleLogger.Log("Command '" + line[0] + "' not found");
            }
        }

        private static void DefaultFunc(string[] args) {
            var line = args.Length > 0 ? args : ReadLine();
            if (DefaultCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                functionToExecute?.Invoke(line.Skip(1).ToArray());
            }
            else {
                ConsoleLogger.Log("Command '" + line[0] + "' not found");
            }
        }

        private static void PackagesFunc(string[] args) {
            var line = args.Length > 0 ? args : ReadLine();
            if (PackagesCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                functionToExecute?.Invoke(line.Skip(1).ToArray());
            }
            else {
                ConsoleLogger.Log("Command '" + line[0] + "' not found");
            }
        }

        private static void UserFunc(string[] args) {
            var line = args.Length > 0 ? args : ReadLine();
            if (UserCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                functionToExecute?.Invoke(line.Skip(1).ToArray());
            }
            else {
                ConsoleLogger.Log("Command '" + line[0] + "' not found");
            }
        }

        private static void SysctlFunc(string[] args) {
            var line = args.Length > 0 ? args : ReadLine();
            if (SysctlCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                functionToExecute?.Invoke(line.Skip(1).ToArray());
            }
            else {
                ConsoleLogger.Log("Command '" + line[0] + "' not found");
            }
        }

        private static void NsswitchFunc(string[] args) {
            var line = args.Length > 0 ? args : ReadLine();
            if (NsswitchCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                functionToExecute?.Invoke(line.Skip(1).ToArray());
            }
            else {
                ConsoleLogger.Log("Command '" + line[0] + "' not found");
            }
        }

        private static void ProcFunc(string[] args) {
            foreach (var proc in Antd.ProcFs.ProcFs.Processes()) {
                Console.WriteLine($"{proc.Pid} {proc.Name} {proc.CommandLine}");
            }
        }

        private static void OpenFilesFunc(string[] args) {
            foreach (var file in new Antd.ProcFs.Process(1).OpenFiles) {
                Console.WriteLine(file);
            }
        }

        private static void NetstatFunc(string[] args) {
            foreach (var svc in Antd.ProcFs.ProcFs.Net.Services.Unix().Where(svc => svc.State == Antd.ProcFs.NetServiceState.Established)) {
                Console.WriteLine(svc);
            }
        }

        private static void InitFunc(string[] args) {
            var line = args.Length > 0 ? args : ReadLine();
            if (InitCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                functionToExecute?.Invoke(line.Skip(1).ToArray());
            }
            else {
                ConsoleLogger.Log("Command '" + line[0] + "' not found");
            }
        }

        private static void UnitFunc(string[] args) {
            var line = args.Length > 0 ? args : ReadLine();
            if (UnitCommand.Options.TryGetValue(line[0], out Action<string[]> functionToExecute)) {
                functionToExecute?.Invoke(line.Skip(1).ToArray());
            }
            else {
                ConsoleLogger.Log("Command '" + line[0] + "' not found");
            }
        }
    }
}