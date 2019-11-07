using Antd2.Init;
using System;
using System.Collections.Generic;

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
    }
}