//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace antdlib.Common {
    public class Param {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class Section {
        public string SectionName { get; set; }
        public Param Param { get; set; }
    }

    public class ParameterXmlWriter {
        private readonly string[] _path;

        public ParameterXmlWriter(string[] fileNames) {
            var applicationRoot = Folder.AntdCfg;
            var tmplist = new List<string>();
            tmplist.AddRange(from fileName in fileNames let p = Path.Combine(applicationRoot, fileName + ".xml") select Path.Combine(applicationRoot, fileName + ".xml"));
            _path = tmplist.ToArray();
        }

        public void Write(string key, string value) {
            List<Param> tList;
            Param tItem;
            var readList = ReadAll();
            var paramList = new List<Param>();
            if (readList != null) {
                paramList.AddRange(from sect in readList
                                   where sect.Param != null
                                   select sect.Param);
            }

            var oldItem = (from i in paramList
                           where i.Key == key
                           select i).FirstOrDefault();
            if (oldItem == null) {
                var item = new Param {
                    Key = key,
                    Value = value
                };
                tItem = item;
            }
            else {
                tItem = oldItem;
                tItem.Value = value;
            }
            if (paramList.ToArray().Length < 1) {
                var list = new List<Param> { tItem };
                tList = list;
            }
            else {
                paramList.Remove(oldItem);
                paramList.Add(tItem);
                tList = paramList;
            }

            var document = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("section", new XAttribute("section", "antd.config"),
                            from el in tList
                            select new XElement("param",
                            new XAttribute("key", el.Key),
                            new XAttribute("Value", el.Value)
            )));
            foreach (var p in _path) {
                document.Save(p);
            }
        }

        public List<Section> ReadAll() {
            if (!File.Exists(_path[0])) {
                return null;
            }
            var xelement = XElement.Load(_path[0]);
            var parameters = xelement.Elements();
            var list =
                (from elem in parameters
                 select new Section {
                     SectionName = xelement.Attribute("section").Value,
                     Param = new Param {
                         Key = elem.Attribute("key").Value,
                         Value = elem.Attribute("Value").Value
                     },
                 }
                ).ToList();

            return list;
        }

        public string ReadValue(string key) {
            var list = ReadAll();
            if (list == null) {
                return null;
            }
            var parList = (from p in list
                           where p.Param != null
                           select p.Param).ToList();
            var param = (from v in parList
                         where v.Key == key
                         select v).FirstOrDefault();
            return param?.Value;
        }

        public bool CheckValue(string key) {
            var getValue = ReadValue(key);
            return getValue != null;
        }
    }
}