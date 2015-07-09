using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antdsh {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("> antdsh");
            Console.WriteLine("> {0} args", args.Length);
            if (args.Length == 0) {
                args[0] = Console.ReadLine();
                Command(args[0]);
            }
            else {
                Command(args[0]);
            }
            Console.ReadLine();
        }

        static void Command(string command) {
            Console.WriteLine("> " + command);
            if (command == "check-update") {
                shell.CheckUpdate();
            }
        }
    }
}
