using System;
using System.Collections.Generic;
using System.Text;

namespace Antd2.Import {
    public class Dhcpd {


        private static dynamic get_config_file(string file) {
            string[] tok = new string[0];
            int i = 0;
            int j = 0;
            List<dynamic> rv = new List<dynamic>();
            dynamic c = "?";

            string[] lines = tokenize_file(file, tok);

            while (i < tok.Length) {
                var str = parse_struct(tok, i, j++, file);
                if (str != null) {
                    if (str["name"] == "include") {
                        var p = str["values"];

                    }
                    else {
                        rv.Add(str);
                    }
                }
            }
            return rv;
        }

        private static string[] tokenize_file(string file, string[] tok) {
            dynamic lines = 0;
            dynamic line = "?";
            dynamic cmode = "?";




            return null;
        }

        private static IDictionary<string, dynamic> parse_struct(string[] tok, int i, int j, string file) {
            return null;
        }

    }
}
