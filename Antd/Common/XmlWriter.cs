﻿///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Antd {

    public class param {

        public string key { get; set; }

        public string value { get; set; }
    }

    public class section {

        public string sectionName { get; set; }

        public param param { get; set; }
    }

    public class XmlWriter {
        public string[] path;

        public XmlWriter(string[] fileNames) {
            var applicationRoot = Folder.Config;
            List<string> tmplist = new List<string>() { };
            foreach (string fileName in fileNames) {
                var p = Path.Combine(applicationRoot, fileName + ".xml");
                tmplist.Add(Path.Combine(applicationRoot, fileName + ".xml"));
            }
            path = tmplist.ToArray();
        }

        public void Write(string key, string value) {
            string sectionName = "antd.config";
            List<param> tList;
            param tItem;
            List<section> readList = ReadAll();
            List<param> paramList = new List<param>();
            if (readList != null) {
                paramList.AddRange(from sect in readList 
                                   where sect.param != null 
                                   select sect.param);
            }

            var oldItem = (from i in paramList
                           where i.key == key
                           select i).FirstOrDefault();
            if (oldItem == null) {
                param item = new param {
                    key = key,
                    value = value
                };
                tItem = item;
            }
            else {
                tItem = oldItem;
                tItem.value = value;
            }
            if (paramList.ToArray().Length < 1) {
                List<param> list = new List<param> {tItem};
                tList = list;
            }
            else {
                paramList.Remove(oldItem);
                paramList.Add(tItem);
                tList = paramList;
            }

            XDocument document = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("section", new XAttribute("section", sectionName),
                            from el in tList
                            select new XElement("param",
                            new XAttribute("key", el.key),
                            new XAttribute("value", el.value)
            )));
            foreach (string p in path) {
                document.Save(p);
            }
        }

        public List<section> ReadAll() {
            if (!File.Exists(path[0])) {
                return null;
            }
            XElement xelement = XElement.Load(path[0]);
            IEnumerable<XElement> parameters = xelement.Elements();

            List<section> list =
                (from elem in parameters
                 select new section {
                     sectionName = xelement.Attribute("section").Value.ToString(),
                     param = new param { key = elem.Attribute("key").Value.ToString()
                                       , value = elem.Attribute("value").Value.ToString() },
                 }
                ).ToList();

            return list;
        }

        public string ReadValue(string key) {
            List<section> list = ReadAll();
            if (list == null) {
                return null;
            }
            List<param> parList = (from p in list 
                                   where p.param != null 
                                   select p.param).ToList();
            param param = (from v in parList
                           where v.key == key
                           select v).FirstOrDefault();
            if (param == null) {
                return null;
            }
            return param.value;
        }

        public bool CheckValue(string key) {
            string getValue = ReadValue(key);
            return getValue != null;
        }
    }
}