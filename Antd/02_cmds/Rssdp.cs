using Antd.models;
using anthilla.core;
using Rssdp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Antd.cmds {
    public class Rssdp {

        private static SsdpDevicePublisher _Publisher;

        public static ServiceDiscoveryModel GetDeviceDescription() {
            var antdPort = Application.CurrentConfiguration.WebService.GuiWebServicePort;
            var specVersion = new SpecVersion { Major = "0", Minor = "0" };
            var services = new List<models.Service> {
                new models.Service {
                    ServiceType = $"antd_{antdPort}",
                    ServiceId = "0",
                    ControlURL = "",
                    EventSubURL = "",
                    SCPDURL = ""
                },
                new models.Service {
                    ServiceType = "other service",
                    ServiceId = "1",
                    ControlURL = "",
                    EventSubURL = "",
                    SCPDURL = ""
                },
            };
            var serviceList = new ServiceList { Service = services };
            var device = new Device {
                DeviceType = "anthilla os device",
                FriendlyName = Application.CurrentConfiguration.Host.HostName,
                Manufacturer = "Anthilla SRL",
                ManufacturerURL = "www.anthilla.com",
                ModelDescription = "anthilla os device",
                ModelName = "anthilla os device",
                ModelNumber = Application.CurrentConfiguration.Host.PartNumber.ToString(),
                ModelURL = "www.anthilla.com",
                SerialNumber = Application.CurrentConfiguration.Host.SerialNumber.ToString(),
                UDN = Application.CurrentConfiguration.Host.MachineUid.ToString(),
                UPC = Application.CurrentConfiguration.Host.MachineUid.ToString(),
                ServiceList = serviceList,
            };
            var descr = new ServiceDiscoveryModel {
                SpecVersion = specVersion,
                Device = device
            };
            return descr;
        }

        public static void PublishThisDevice() {
            var appPort = Application.CurrentConfiguration.WebService.GuiWebServicePort;
            var localIp = Application.CurrentConfiguration.Network.InternalNetwork.IpAddress;
            if(string.IsNullOrEmpty(localIp)) {
                ConsoleLogger.Log("[rssdp] cannot publish device: missing ip address");
                return;
            }
            PublishThisDevice(localIp);
        }

        public static void PublishThisDevice(string ip) {
            if(string.IsNullOrEmpty(ip)) {
                ConsoleLogger.Log("[rssdp] cannot publish device: ip address not valid");
                return;
            }
            var appPort = Application.CurrentConfiguration.WebService.GuiWebServicePort;
            var deviceDefinition = new SsdpRootDevice() {
                CacheLifetime = TimeSpan.FromMinutes(30),
                Uuid = Application.CurrentConfiguration.Host.MachineUid.ToString(),
                Location = new Uri($"http://{ip}:{appPort}/device/description"),
                DeviceTypeNamespace = "antd",
                ModelUrl = new Uri($"http://{ip}:{appPort}/"),
                DeviceType = Application.CurrentConfiguration.Host.PartNumber.ToString(),
                FriendlyName = $"{ip} as {Application.CurrentConfiguration.Host.HostName}",
                Manufacturer = "Anthilla SRL",
                ModelName = Application.CurrentConfiguration.Host.SerialNumber.ToString().Replace("-", ""),
                SerialNumber = Application.CurrentConfiguration.Host.SerialNumber.ToString()
            };
            _Publisher = new SsdpDevicePublisher();
            _Publisher.AddDevice(deviceDefinition);
            ConsoleLogger.Log($"[rssdp] publishing this device on '{ip}'");
        }

        public static async Task<List<NodeModel>> Discover() {
            ConsoleLogger.Log("[rssdp] Start scanning");
            var uidRegex = new Regex("uuid\\:([a-zA-Z0-9\\-]+)\\:");
            var ipRegex = new Regex("([0-9]{0,3}\\.[0-9]{0,3}\\.[0-9]{0,3}\\.[0-9]{0,3})");
            var list = new List<NodeModel>();

            using(var deviceLocator = new SsdpDeviceLocator()) {
                var foundDevices = await deviceLocator.SearchAsync();
                foreach(var foundDevice in foundDevices) {
                    var device = new NodeModel();
                    device.RawUid = foundDevice.Usn;
                    device.DescriptionLocation = foundDevice.DescriptionLocation.ToString();
                    device.PublicIp = ipRegex.Match(device.DescriptionLocation).Groups[1].Value;
                    try {
                        var fullDevice = await foundDevice.GetDeviceInfo();
                        device.Hostname = fullDevice.FriendlyName;
                        device.DeviceType = fullDevice.DeviceType;
                        device.Manufacturer = fullDevice.Manufacturer;
                        device.ModelName = fullDevice.ModelName;
                        device.ModelDescription = fullDevice.ModelDescription;
                        device.ModelNumber = fullDevice.ModelNumber;
                        device.ModelUrl = device.DescriptionLocation.Replace("device/description", "");
                        device.SerialNumber = fullDevice.SerialNumber;
                        ConsoleLogger.Log($"[rssdp] Found device: {device.Hostname}");
                    }
                    catch(Exception) {
                        //
                    }
                    list.Add(device);
                }
            }

            var mergedList = new List<NodeModel>();
            var groupedList = list.GroupBy(_ => _.PublicIp).ToList();
            foreach(var group in groupedList) {
                var mergedNode = MergeUidInformation(group.ToList());
                var tryget = mergedList.FirstOrDefault(_ => _.MachineUid == mergedNode.MachineUid);
                if(tryget == null) {
                    mergedList.Add(mergedNode);
                }
            }
            return mergedList;
        }

        //uuid:df5fb5c5-98c6-4103-8860-249747b8f5eb::upnp:rootdevice
        //uuid:df5fb5c5-98c6-4103-8860-249747b8f5eb::pnp:rootdevice
        //uuid:df5fb5c5-98c6-4103-8860-249747b8f5eb
        //uuid:df5fb5c5-98c6-4103-8860-249747b8f5eb::urn:antd:device:5a67b142-5139-4820-8fee-574d56fffc07:1

        private static NodeModel MergeUidInformation(List<NodeModel> rawNodes) {
            var node = rawNodes.FirstOrDefault();
            var uuidRegex = new Regex("uuid\\:([0-9a-zA-Z\\-]+)");
            var upnpRegex = new Regex("upnp\\:([0-9a-zA-Z\\-]+)");
            var pnpRegex = new Regex("pnp\\:([0-9a-zA-Z\\-]+)");
            var urnRegex = new Regex("urn\\:([0-9a-zA-Z\\-]+)");
            var deviceRegex = new Regex("device\\:([0-9a-zA-Z\\-]+)");
            foreach(var n in rawNodes) {
                if(string.IsNullOrEmpty(node.MachineUid)) {
                    node.MachineUid = uuidRegex.Match(n.RawUid).Groups[1].Value;
                }
                if(string.IsNullOrEmpty(node.Upnp)) {
                    node.Upnp = upnpRegex.Match(n.RawUid).Groups[1].Value;
                }
                if(string.IsNullOrEmpty(node.Pnp)) {
                    node.Pnp = pnpRegex.Match(n.RawUid).Groups[1].Value;
                }
                if(string.IsNullOrEmpty(node.Urn)) {
                    node.Urn = urnRegex.Match(n.RawUid).Groups[1].Value;
                }
                if(string.IsNullOrEmpty(node.Device)) {
                    node.Device = deviceRegex.Match(n.RawUid).Groups[1].Value;
                }
            }
            return node;
        }

        /// <summary>
        /// TODO
        /// Andranno aggiunti alla parte di configurazione
        /// </summary>
        /// <returns></returns>
        public static ClusterNodeService[] GetServices() {
            return new ClusterNodeService[] {
                new ClusterNodeService { Name = "antd", Port = Application.CurrentConfiguration.WebService.Port },
                new ClusterNodeService { Name = "antdui", Port = Application.CurrentConfiguration.WebService.GuiWebServicePort }
            };
        }
    }
}
