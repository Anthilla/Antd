using Antd2.cmds;
using System;
using System.Collections.Generic;
using System.IO;

namespace Antd2 {

    /// <summary>
    /// https://linux.die.net/man/1/getfacl
    /// https://linux.die.net/man/1/setfacl
    /// </summary>
    public class AclCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "get", GetFunc },
            };

        public static void GetFunc(string[] args) {
            var file = args[0];
            if (!File.Exists(file)) {
                Console.WriteLine($"  {file} not found...");
            }
            var fileInfo = new FileInfo(file);
            Console.WriteLine($"  file:      {fileInfo.Name}");
            Console.WriteLine($"  directory: {fileInfo.DirectoryName}");
            Console.WriteLine();
            Console.WriteLine();
            var facl = Acl.Get(file);
            Console.WriteLine($"  owner:     {facl.Owner}");
            Console.WriteLine($"  group:     {facl.Group}");
            foreach (var l in facl.Acl)
                Console.WriteLine($"  acl-rule:  {l}");
        }
    }
}