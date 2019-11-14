using System;
using System.Collections.Generic;

#if NETCOREAPP
using Antd2.Init;
#endif

namespace Antd2 {
    public class UnitCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                {"start", StartUnits },
                {"stop", StopUnits },
                {"restart", RestartUnits },
                {"reload", ReloadUnits },
                {"list", ListUnits },
                {"daemon-reload", RescanUnits },
                {"status", GetUnitStatus },
                {"describe-deps", DescribeDependenciesFunc }
            };

#if NETCOREAPP
        static void DescribeDependenciesFunc(string[] args) {
            InitController.Instance.DescribeDependenciesFunc(args);
        }

        static void RescanUnits(string[] args) {
            InitController.Instance.RescanUnits(args);
        }

        static void GetUnitStatus(string[] args) {
            InitController.Instance.GetUnitStatus(args);
        }

        static void StartUnits(string[] args) {
            InitController.Instance.StartUnits(args);
        }

        static void StopUnits(string[] args) {
            InitController.Instance.StopUnits(args);
        }

        static void RestartUnits(string[] args) {
            InitController.Instance.RestartUnits(args);
        }

        static void ReloadUnits(string[] args) {
            InitController.Instance.ReloadUnits(args);
        }

        static void ListUnits(string[] args) {
            InitController.Instance.ListUnits(args);
        }
#endif

#if NETFRAMEWORK
        static void DescribeDependenciesFunc(string[] args) {
            Console.WriteLine("  not supported by .net framework");
        }

        static void RescanUnits(string[] args) {
            Console.WriteLine("  not supported by .net framework");
        }

        static void GetUnitStatus(string[] args) {
            Console.WriteLine("  not supported by .net framework");
        }

        static void StartUnits(string[] args) {
            Console.WriteLine("  not supported by .net framework");
        }

        static void StopUnits(string[] args) {
            Console.WriteLine("  not supported by .net framework");
        }

        static void RestartUnits(string[] args) {
            Console.WriteLine("  not supported by .net framework");
        }

        static void ReloadUnits(string[] args) {
            Console.WriteLine("  not supported by .net framework");
        }

        static void ListUnits(string[] args) {
            Console.WriteLine("  not supported by .net framework");
        }
#endif
    }
}