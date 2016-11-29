using System;
using System.Collections.Generic;
using antdlib.common;
using antdlib.common.Tool;

namespace antd.commands {
    public class Command : ICommand {

        public IEnumerable<string> Arguments { get; set; }

        public Func<IEnumerable<string>, string, IEnumerable<string>> Function { get; set; }

        public string Grep { get; set; } = "";

        public Type CommandType { get; set; } = typeof(Command);

        public IEnumerable<string> Launch() {
            var arguments = Arguments;
            try {
                return Function(arguments, Grep);
            }
            catch(Exception ex) {
                ConsoleLogger.Error(ex);
                return new List<string>();
            }
        }

        public IEnumerable<string> LaunchD(Dictionary<string, string> substitutions) {
            if(Arguments == null) {
                return new List<string>();
            }
            var arguments = Arguments;
            var grep = Grep;
            foreach(var sub in substitutions) {
                arguments = arguments.ReplaceInList(sub.Key, sub.Value);
                grep = grep.ReplaceInString(sub.Key, sub.Value);
            }
            try {
                return Function(arguments, Grep);
            }
            catch(Exception ex) {
                ConsoleLogger.Error(ex);
                return new List<string>();
            }
        }
    }
}
