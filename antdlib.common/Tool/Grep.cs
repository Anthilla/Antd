using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using IoDir = System.IO.Directory;

namespace antdlib.common.Tool {
    public class Grep {

        private readonly IEnumerable<KeyValuePair<string, string>> _empty = new List<KeyValuePair<string, string>>();

        public IEnumerable<KeyValuePair<string, string>> InText(string input, string pattern, bool caseInsensitive = false) {
            if(string.IsNullOrEmpty(pattern)) {
                return _empty;
            }
            if(string.IsNullOrEmpty(input)) {
                return _empty;
            }
            try {
                var list = new List<KeyValuePair<string, string>>();
                if(caseInsensitive) {
                    if(Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase)) {
                        list.Add(new KeyValuePair<string, string>("", input));
                    }
                }
                else {
                    if(Regex.IsMatch(input, pattern)) {
                        list.Add(new KeyValuePair<string, string>("", input));
                    }
                }
                return list;
            }
            catch(Exception) {
                return _empty;
            }
        }

        public IEnumerable<KeyValuePair<string, string>> InText(IEnumerable<string> inputLines, string pattern, bool caseInsensitive = false) {
            if(string.IsNullOrEmpty(pattern)) {
                return _empty;
            }
            var inputList = inputLines as IList<string> ?? inputLines.ToList();
            if(!inputList.Any()) {
                return _empty;
            }
            try {
                var list = new List<KeyValuePair<string, string>>();
                foreach(var input in inputList) {
                    if(caseInsensitive) {
                        if(Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase)) {
                            list.Add(new KeyValuePair<string, string>("", input));
                        }
                    }
                    else {
                        if(Regex.IsMatch(input, pattern)) {
                            list.Add(new KeyValuePair<string, string>("", input));
                        }
                    }
                }
                return list;
            }
            catch(Exception) {
                return _empty;
            }
        }

        public IEnumerable<KeyValuePair<string, string>> InFiles(string directoryPath, string pattern, bool recursive = true, bool caseInsensitive = false) {
            if(string.IsNullOrEmpty(pattern)) {
                return _empty;
            }
            if(directoryPath == "") {
                directoryPath = IoDir.GetCurrentDirectory();
            }
            else if(directoryPath != "") {
                var x = directoryPath;
                directoryPath = x;
            }
            var results = SearchDirectory(directoryPath, pattern, recursive, caseInsensitive);
            return results;
        }

        private IEnumerable<KeyValuePair<string, string>> SearchDirectory(string path, string pattern, bool recursive, bool caseInsensitive) {
            var list = new List<KeyValuePair<string, string>>();
            try {
                var files = recursive ? IoDir.GetFiles(path, "*.*", SearchOption.AllDirectories) : IoDir.GetFiles(path);
                foreach(var f in files) {
                    var file = new FileInfo(f);
                    if(caseInsensitive) {
                        if(Regex.IsMatch(file.Name, pattern, RegexOptions.IgnoreCase)) {
                            list.Add(new KeyValuePair<string, string>(file.Name, file.Name));
                            if(file.DirectoryName != null) {
                                list.Add(new KeyValuePair<string, string>(file.Name, Path.Combine(file.DirectoryName, file.Name)));
                            }
                        }

                        using(var fileRead = new FileStream(f, FileMode.Open, FileAccess.Read))
                        using(var streamRead = new StreamReader(fileRead)) {
                            string srccurrline;
                            while((srccurrline = streamRead.ReadLine()) != null) {
                                if(Regex.IsMatch(srccurrline, pattern, RegexOptions.IgnoreCase)) {
                                    list.Add(new KeyValuePair<string, string>(file.Name, srccurrline));
                                }
                            }
                        }
                    }
                    else {
                        if(Regex.IsMatch(file.Name, pattern)) {
                            list.Add(new KeyValuePair<string, string>(file.Name, file.Name));
                            if(file.DirectoryName != null) {
                                list.Add(new KeyValuePair<string, string>(file.Name, Path.Combine(file.DirectoryName, file.Name)));
                            }
                        }

                        using(var fileRead = new FileStream(f, FileMode.Open, FileAccess.Read))
                        using(var streamRead = new StreamReader(fileRead)) {
                            string srccurrline;
                            while((srccurrline = streamRead.ReadLine()) != null) {
                                if(Regex.IsMatch(srccurrline, pattern)) {
                                    list.Add(new KeyValuePair<string, string>(file.Name, srccurrline));
                                }
                            }
                        }
                    }
                }
                return list.ToArray();
            }
            catch(Exception) {
                return _empty;
            }
        }
    }
}
