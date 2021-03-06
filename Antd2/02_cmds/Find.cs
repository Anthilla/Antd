﻿using System.Collections.Generic;

namespace Antd2.cmds {
    public class Find {

        private const string findCommand = "find";

        public static IEnumerable<string> File(string path, string pattern, string type = "f") {
            var typeString = string.IsNullOrEmpty(type) ? "" : $" -type {type} ";
            var command = $"{findCommand} {path} {typeString} -name {pattern}";
            System.Console.WriteLine(command);
            return Bash.Execute(command);
        }
    }
}
