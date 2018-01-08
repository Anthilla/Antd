using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace antd_tools {
    internal class Application {

        class Options {
            [Option('r', "read", Required = true,
              HelpText = "Input files to be processed.")]
            public IEnumerable<string> InputFiles { get; set; }

            // Omitting long name, default --verbose
            [Option(
              HelpText = "Prints all messages to standard output.")]
            public bool Verbose { get; set; }

            [Option(DefaultValue = "中文",
              HelpText = "Content language.")]
            public string Language { get; set; }
        }

        private static void Main(string[] args) {
            var resetEvent = new AutoResetEvent(initialState: false);
            Console.CancelKeyPress += (s, e) => { e.Cancel = true; resetEvent.Set(); };

            var options = new Options();
            var isValid = CommandLine.Parser.Default.ParseArgumentsStrict(args, options);

            resetEvent.WaitOne();
            Environment.Exit(0);
        }
    }
}
