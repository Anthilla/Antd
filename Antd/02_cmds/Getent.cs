using anthilla.core;
using System.Collections.Generic;

namespace Antd.cmds {
    public class Getent {

        private const string getentFileLocation = "/usr/bin/getent";
        private const string passwdArg = "passwd";

        public static IEnumerable<string> Passwd() {
            return CommonProcess.Execute(getentFileLocation, passwdArg);
        }
    }
}
