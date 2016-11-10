using System;
using System.IO;
using System.Text.RegularExpressions;

namespace antdlib.common.Tool {
    class Program {
        private static string pattern = string.Empty;
        private static string path = "";
        private static bool recursive = false;
        private static bool supress = false;
        private static bool IgnoreCase = false;
        public static bool inverse = false;
        private static string Help = @"This application lists files based on matching Regular Expression.
                Arguments:
                -?                  Show help message.
                -r                  Recursive search
                -l                  Supress results to show only file names.
                -i                  ignore case.
                -v                  invert pattern.
                path must look like this: C:\folder\.
                pattern must begin with %.
                ";

        static void Main(string[] args) {
            readArgs(args);
            DisplayArgs(args);
            if(path == "") {
                path = Directory.GetCurrentDirectory();
            }
            else if(path != "") {
                var x = path;
                path = x;
            }

            if(pattern == "") {
                string msg = string.Format("Pattern can not be empty ");
                Console.Write(msg);
            }
            if(pattern != "") {
                System.Console.WriteLine(@"Your search in {0} for {1} returned the following:       
****************************************************************************
****************************************************************************
            ", path, pattern);
                SearchDirectory(path, pattern, recursive, supress, IgnoreCase);
            }
            System.Console.ReadKey();
        }
        //Reads and parses argumets that are passed in from the command line
        private static void readArgs(string[] args) {
            foreach(string arg in args) {
                if(arg.ToLower().Contains(":" + "\\")) {
                    path = arg;
                }
                else if(arg.StartsWith("-?")) {
                    System.Console.WriteLine(Help);
                }
                else if(arg.ToLower().StartsWith("-r")) {
                    recursive = true;
                }
                else if(arg.ToLower().StartsWith("-l")) {
                    supress = true;
                }
                else if(arg.ToLower().StartsWith("-i")) {
                    IgnoreCase = true;
                }
                else if(arg.ToLower().StartsWith("-v")) {
                    inverse = true;
                }
                else if(arg.StartsWith("%")) {
                    var x = arg.Substring(1);
                    pattern = x;
                }
                else {
                    System.Console.WriteLine("{0} is not a valid argument. Type -? for help", arg);
                }
            }
        }
        //Displays the arguments and number of arguments in the results
        private static void DisplayArgs(string[] args) {
            string msg = string.Format("There were {0} args passed", args.Length);
            System.Console.WriteLine(msg);
            foreach(string arg in args) {
                Console.WriteLine(arg);
            }
        }
        //Searches directory based on parameters gathered from the arguments that were parsed
        public static void SearchDirectory(string path, string pattern, bool recursive, bool supress, bool IgnoreCase) {
            string[] files;
            try {
                if(recursive == true) {
                    files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
                }
                else {
                    files = Directory.GetFiles(path);
                }
                foreach(var f in files) {
                    var file = new FileInfo(f);
                    {
                        if(IgnoreCase == true) {
                            if(Regex.IsMatch(file.Name, pattern, RegexOptions.IgnoreCase) && supress == true) {
                                Console.WriteLine(file.Name);
                            }
                            else if(Regex.IsMatch(file.Name, pattern, RegexOptions.IgnoreCase) && supress == false) {
                                Console.WriteLine(file.DirectoryName + "\\" + file.Name);
                            }
                        }
                        else if(IgnoreCase == false) {
                            if(Regex.IsMatch(file.Name, pattern) && supress == true) {
                                Console.WriteLine(file.Name);
                            }
                            else if(Regex.IsMatch(file.Name, pattern) && supress == false) {
                                Console.WriteLine(file.DirectoryName + "\\" + file.Name);
                            }

                        }
                    }
                }
            }
            catch(Exception e) {
                Console.WriteLine(e);
            }
        }

    }
}
