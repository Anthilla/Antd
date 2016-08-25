using System.Xml.Serialization;

namespace Antd.Avahi {
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
}
