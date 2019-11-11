using anthilla.core;
using System;
using System.Collections.Generic;

namespace Antd2 {
    public class DefaultCommand {
        public static readonly Dictionary<string, Action<string[]>> Options =
            new Dictionary<string, Action<string[]>> {
                { "apply", ApplyFunc },
            };

        private static void ApplyFunc(string[] args) {
            Console.WriteLine("  Apply global default settings!");
            UserCommand.AddFunc(args);
            UserCommand.CheckFunc(args);
            SysctlCommand.SetFunc(args);
            SysctlCommand.CheckFunc(args);
            NsswitchCommand.ApplyFunc(args);
            NsswitchCommand.CheckFunc(args);
        }
    }
}