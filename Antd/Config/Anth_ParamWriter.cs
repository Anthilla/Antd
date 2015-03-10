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

    public class Anth_ParamWriter {
        public static string root = "/cfg";
        public string folder;
        public string[] path;

        public Anth_ParamWriter(string[] fileNames, string newFolder) {
            folder = Path.Combine(root, newFolder); ;
            Directory.CreateDirectory(folder);
            List<string> tmplist = new List<string>() { };
            foreach (string fileName in fileNames) {
                var p = Path.Combine(folder, fileName + ".xml");
                tmplist.Add(p);
            }
            path = tmplist.ToArray();
        }

        public void Write(string key, string value) {
            string sectionName = "antd";
            List<param> tList;
            param tItem;
            List<section> readList = ReadAll();
            List<param> paramList = new List<param>();
            if (readList != null) {
                foreach (var sect in readList) {
                    if (sect.param != null) {
                        paramList.Add(sect.param);
                    }
                }
            }

            var oldItem = (from i in paramList
                           where i.key == key
                           select i).FirstOrDefault();
            if (oldItem == null) {
                param item = new param();
                item.key = key;
                item.value = value;
                tItem = item;
            }
            else {
                tItem = oldItem;
                tItem.value = value;
            }
            if (paramList == null || paramList.ToArray().Length < 1) {
                List<param> list = new List<param>() { };
                list.Add(tItem);
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
                     param = new param { key = elem.Attribute("key").Value.ToString(), value = elem.Attribute("value").Value.ToString() },
                 }
                ).ToList();

            return list;
        }

        public string ReadValue(string key) {
            List<section> list = ReadAll();
            if (list == null) {
                return null;
            }
            List<param> parList = new List<param>();
            foreach (var p in list) {
                if (p.param != null) {
                    parList.Add(p.param);
                }
            }
            param param = (from v in parList
                           where v.key == key
                           select v).FirstOrDefault();
            if (param == null) {
                return null;
            }
            string value = param.value;
            return value;
        }

        public bool CheckValue(string key) {
            string getValue = ReadValue(key);
            if (getValue == null) {
                return false;
            }
            return true;
        }
    }
}