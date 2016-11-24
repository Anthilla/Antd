using System.Collections.Generic;
using System.Xml.Serialization;

namespace Antd.Discovery {
    [XmlRoot(ElementName = "name")]
    public class Name {
        [XmlAttribute(AttributeName = "replace-wildcards")]
        public string Replacewildcards { get; set; }
        [XmlText]
        public string Text { get; set; }
    }

    [XmlRoot(ElementName = "service")]
    public class Service {
        [XmlElement(ElementName = "type")]
        public string Type { get; set; }
        [XmlElement(ElementName = "port")]
        public string Port { get; set; }
    }

    [XmlRoot(ElementName = "service-group")]
    public class Servicegroup {
        [XmlElement(ElementName = "name")]
        public Name Name { get; set; }
        [XmlElement(ElementName = "service")]
        public Service Service { get; set; }
    }

    public class AvahiCustomXml {
        public static IEnumerable<string> Generate(string port) {
            return new List<string> {
                "<?xml version=\"1.0\" standalone='no'?>" ,
                "<!DOCTYPE service-group SYSTEM \"avahi-service.dtd\"> " ,
                "<service-group> " ,
                "<name replace-wildcards=\"yes\">antd on %h</name> " ,
                "<service> " ,
                "<type>_http._tcp</type> " ,
                $"<port>{port}</port> " ,
                "<txt-record>antd_service</txt-record> " ,
                "</service> " ,
                "</service-group >"
            };
        }
    }
}
