using System;
using System.Collections.Generic;
using antdlib.common.Tool;

namespace antd.commands {
    public class Command<TInput, TOutput> : ICommand {

        public TInput Arguments { get; set; }

        public Func<TInput, string, TOutput> Functions { get; set; }

        public string Grep { get; set; }

        public TOutput Launch() {
            var arguments = Arguments;
            try {
                return Functions(arguments, Grep);
            }
            catch(Exception) {
                return default(TOutput);
            }
        }

        public TOutput Launch(Dictionary<string, string> substitutions) {
            if(Arguments == null) {
                return default(TOutput);
            }
            var arguments = Arguments;
            var grep = Grep;
            foreach(var sub in substitutions) {
                //todo controlla se funziona
                arguments = arguments.ReplaceX(sub.Key, sub.Value);
                grep = grep.ReplaceX(sub.Key, sub.Value);
            }
            try {
                return Functions(arguments, Grep);
            }
            catch(Exception) {
                return default(TOutput);
            }
        }
    }
}
