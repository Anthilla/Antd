using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Antd2.cmds {

    public class KnownPorts {

        private const string etcServicesFilePath = "/etc/services";

        public static IEnumerable<(string Service, string Port, string Protocol)> Get() {
            if (!File.Exists(etcServicesFilePath))
                return Array.Empty<(string Service, string Port, string Protocol)>();
            var lines = File.ReadAllLines(etcServicesFilePath)
                .Where(_ => !_.StartsWith("#", StringComparison.InvariantCulture))
                .Where(_ => !string.IsNullOrEmpty(_))
                .Select(_ => ParseServicesLine(_))
                .Where(_ => !string.IsNullOrEmpty(_.Service));
            return lines;
        }

        private static (string Service, string Port, string Protocol) ParseServicesLine(string line) {
            var arr = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length < 1) {
                return (string.Empty, string.Empty, string.Empty);
            }
            var portProtocol = arr[1].Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (portProtocol.Length != 2) {
                return (string.Empty, string.Empty, string.Empty);
            }
            return (arr[0], portProtocol[0], portProtocol[1]);
        }

    }
}
