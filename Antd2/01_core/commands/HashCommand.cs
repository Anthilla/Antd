using System;
using System.Collections.Generic;
using System.IO;

namespace Antd2 {
    public class HashCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "sha256", Sha256Func },
            };

        public static void Sha256Func(string[] args) {
            var file = args[0];
            if (!File.Exists(file)) {
                Console.WriteLine($"  file not found: {file}");
                return;
            }
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var hash = Antd2.Cryptography.FileHash.GetSHA256(file);
            Console.WriteLine($"  file:    {file}");
            Console.WriteLine($"  hash:    {hash}");
            Console.WriteLine($"  time:    {sw.ElapsedMilliseconds}ms");
        }
    }
}