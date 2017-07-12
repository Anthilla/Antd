using System.Collections.Generic;
using System.Xml.Serialization;

namespace antdlib.models {
    [XmlRoot(ElementName = "specVersion")]
    public class SpecVersion {
        [XmlElement(ElementName = "major")]
        public string Major { get; set; }
        [XmlElement(ElementName = "minor")]
        public string Minor { get; set; }
    }

    [XmlRoot(ElementName = "service")]
    public class Service {
        [XmlElement(ElementName = "serviceType")]
        public string ServiceType { get; set; }
        [XmlElement(ElementName = "serviceId")]
        public string ServiceId { get; set; }
        [XmlElement(ElementName = "controlURL")]
        public string ControlURL { get; set; }
        [XmlElement(ElementName = "eventSubURL")]
        public string EventSubURL { get; set; }
        [XmlElement(ElementName = "SCPDURL")]
        public string SCPDURL { get; set; }
    }

    [XmlRoot(ElementName = "serviceList")]
    public class ServiceList {
        [XmlElement(ElementName = "service")]
        public List<Service> Service { get; set; }
    }

    [XmlRoot(ElementName = "device")]
    public class Device {
        [XmlElement(ElementName = "deviceType")]
        public string DeviceType { get; set; }
        [XmlElement(ElementName = "friendlyName")]
        public string FriendlyName { get; set; }
        [XmlElement(ElementName = "manufacturer")]
        public string Manufacturer { get; set; }
        [XmlElement(ElementName = "manufacturerURL")]
        public string ManufacturerURL { get; set; }
        [XmlElement(ElementName = "modelDescription")]
        public string ModelDescription { get; set; }
        [XmlElement(ElementName = "modelName")]
        public string ModelName { get; set; }
        [XmlElement(ElementName = "modelNumber")]
        public string ModelNumber { get; set; }
        [XmlElement(ElementName = "modelURL")]
        public string ModelURL { get; set; }
        [XmlElement(ElementName = "serialNumber")]
        public string SerialNumber { get; set; }
        [XmlElement(ElementName = "UDN")]
        public string UDN { get; set; }
        [XmlElement(ElementName = "UPC")]
        public string UPC { get; set; }
        [XmlElement(ElementName = "serviceList")]
        public ServiceList ServiceList { get; set; }
    }

    [XmlRoot(ElementName = "deviceList")]
    public class DeviceList {
        [XmlElement(ElementName = "device")]
        public Device Device { get; set; }
    }

    [XmlRoot(ElementName = "root")]
    public class ServiceDiscoveryModel {
        [XmlElement(ElementName = "specVersion")]
        public SpecVersion SpecVersion { get; set; }
        [XmlElement(ElementName = "device")]
        public Device Device { get; set; }
    }

    public class DeviceUidsModel {
        public string Uuid { get; set; }
        public string Upnp { get; set; }
        public string Pnp { get; set; }
        public string Urn { get; set; }
        public string Device { get; set; }
    }

    public class NodeModel {
        public string RawUid { get; set; }
        public string Upnp { get; set; }
        public string Pnp { get; set; }
        public string Urn { get; set; }
        public string Device { get; set; }
        public string MachineUid { get; set; }
        public string DescriptionLocation { get; set; }
        public string DeviceType { get; set; }
        public string Hostname { get; set; }
        public string PublicIp { get; set; }
        public string Manufacturer { get; set; }
        public string ModelName { get; set; }
        public string ModelDescription { get; set; }
        public string ModelNumber { get; set; }
        public string ModelUrl { get; set; }
        public string SerialNumber { get; set; }
        public List<RssdpServiceModel> Services { get; set; }
    }

    public class RssdpServiceModel {
        public string ServiceType { get; set; }
        public string ControlURL { get; set; }
        public string Name { get; set; }
        public string Port { get; set; }
    }
}
