using Antd;
using anthilla.core;
using System;
using System.IO;
using System.Linq;

namespace Antd2.cmds {
    public class Route {

        private const string ipFileLocation = "/bin/ip";
        private const string route = "route";
        private const string show = "show";
        private const string protoKernel = "proto kernel";
        private const string @default = "default";
        private const string comment = "#";

        private const string routingTableFile = "/etc/iproute2/rt_tables";
        private const string routingTableFileBackup = "/etc/iproute2/rt_tables.bck";

        public static NetRoutingTable[] DefaultRoutingTables = new NetRoutingTable[] {
            new NetRoutingTable() { Id = "255", Alias = "local" },
            new NetRoutingTable() { Id = "254", Alias = "main" },
            new NetRoutingTable() { Id = "253", Alias = "default" },
            new NetRoutingTable() { Id = "0", Alias = "unspec" }
        };

        public static NetRoute[] Get() {
            var args = CommonString.Append(route, " ", show);
            var result = CommonProcess.Execute(ipFileLocation, args).Where(_ => !_.Contains(protoKernel)).ToArray();
            var routes = new NetRoute[result.Length];
            for (var i = 0; i < result.Length; i++) {
                var currentLine = result[i];
                var currentLineData = currentLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                routes[i] = new NetRoute() {
                    Default = currentLine.Contains(@default) ? true : false,
                    Destination = currentLine.Contains(@default) ? @default : currentLineData[0],
                    Gateway = Help.CaptureGroup(currentLine, "(via [0-9\\.]+)").Replace("via", "").Trim(),
                    Device = Help.CaptureGroup(currentLine, "(dev [a-zA-Z0-9\\.]+)").Replace("dev", "").Trim()
                };
            }
            return routes;
        }

        //public static bool Set() {
        //    var current = Application.CurrentConfiguration.Network.Routing;
        //    var running = Application.RunningConfiguration.Network.Routing;
        //    bool defaultRouteAlreadySet = false;
        //    for (var i = 0; i < current.Length; i++) {
        //        var route = current[i];
        //        if (route.Default == true) {
        //            if (defaultRouteAlreadySet == false) {
        //                Ip.AddRoute(route.Device, route.Gateway, @default);
        //                defaultRouteAlreadySet = true;
        //            }
        //        }
        //        else {
        //            Ip.AddRoute(route.Device, route.Gateway, route.Destination);
        //        }
        //    }
        //    return true;
        //}

        public static NetRoutingTable[] GetRoutingTable() {
            if (!File.Exists(routingTableFile)) {
                return new NetRoutingTable[0];
            }
            var result = File.ReadAllLines(routingTableFile).Where(_ => !_.StartsWith(comment)).ToArray();
            var routes = new NetRoutingTable[result.Length];
            for (var i = 0; i < result.Length; i++) {
                var currentLine = result[i];
                var currentLineData = currentLine.Split(new[] { ' ', '\t' }, 2, StringSplitOptions.RemoveEmptyEntries);
                routes[i] = new NetRoutingTable() {
                    Id = currentLineData[0],
                    Alias = currentLineData[1]
                };
            }
            return routes;
        }

        //public static bool SetRoutingTable() {
        //    if (!Directory.Exists("/etc/iproute2")) {
        //        return true;
        //    }
        //    var currentHosts = CommonArray.Merge(DefaultRoutingTables, Application.CurrentConfiguration.Network.RoutingTables);
        //    var runningHosts = Application.RunningConfiguration.Network.RoutingTables;
        //    if (currentHosts.Select(_ => _.ToString()).SequenceEqual(runningHosts.Select(_ => _.ToString())) == false) {
        //        if (File.Exists(routingTableFile)) {
        //            File.Copy(routingTableFile, routingTableFileBackup, true);
        //        }
        //        var lines = new string[currentHosts.Length];
        //        for (var i = 0; i < currentHosts.Length; i++) {
        //            lines[i] = CommonString.Append(currentHosts[i].Id, " ", currentHosts[i].Alias);
        //        }
        //        File.WriteAllLines(routingTableFile, lines);
        //    }
        //    return true;
        //}
    }
}
