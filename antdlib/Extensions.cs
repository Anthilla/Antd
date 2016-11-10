using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using antdlib.common.Tool;
using antdlib.Models;
using Nancy.IO;

namespace antdlib {
    public static class Extensions {

        private static readonly Bash Bash = new Bash();

        public static void DosToUnix(this string file, string otherFile = "") {
            var fileToConvert = otherFile.Length > 0 ? otherFile : file;
            Bash.Execute($"dos2unix {fileToConvert}", false);
        }

        public static string ReadAsString(this RequestStream requestStream) {
            using(var reader = new StreamReader(requestStream)) {
                return reader.ReadToEnd();
            }
        }

        public static CommandModel ConvertCommandToModel(this string commandOutput) {
            if(commandOutput == null)
                throw new ArgumentNullException(nameof(commandOutput));
            var command = new CommandModel {
                date = DateTime.Now,
                output = commandOutput,
                outputTable = TextToList(commandOutput),
                error = commandOutput,
                errorTable = TextToList(commandOutput)
            };
            return command;
        }

        public static List<string> TextToList(string text) {
            var rowDivider = new[] { "\n" };
            var rowList = text.Split(rowDivider, StringSplitOptions.None).ToArray();
            return rowList.Where(row => !string.IsNullOrEmpty(row)).ToList();
        }
    }
}
