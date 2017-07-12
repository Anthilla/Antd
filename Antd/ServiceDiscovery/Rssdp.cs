using antdlib.config;
using antdlib.config.shared;
using antdlib.models;
using anthilla.core;
using Rssdp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Antd.ServiceDiscovery {
    public class Rssdp {

        private static SsdpDevicePublisher _Publisher;
        private static string Hostname = Host2Configuration.Host.HostName;
        private static string PartNumber = Machine.MachineIds.Get.PartNumber;
        private static string SerialNumber = Machine.MachineIds.Get.SerialNumber.Replace("-", "").Substring(0, 16);
        private static string MachineUid = Machine.MachineIds.Get.MachineUid;

        private static string CacheFile = $"{antdlib.common.Parameter.AntdCfgRssdp}/cache.json";

        public static ServiceDiscoveryModel GetDeviceDescription() {
            var antdPort = new AppConfiguration().Get().AntdUiPort;
            var specVersion = new SpecVersion { Major = "0", Minor = "0" };
            var services = new List<Service> {
                new Service {
                    ServiceType = $"antd_{antdPort}",
                    ServiceId = "0",
                    ControlURL = "",
                    EventSubURL = "",
                    SCPDURL = ""
                },
                  new Service {
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
                FriendlyName = Hostname,
                Manufacturer = "Anthilla SRL",
                ManufacturerURL = "www.anthilla.com",
                ModelDescription = "anthilla os device",
                ModelName = "anthilla os device",
                ModelNumber = PartNumber,
                ModelURL = "www.anthilla.com",
                SerialNumber = SerialNumber,
                UDN = MachineUid,
                UPC = MachineUid,
                ServiceList = serviceList,
            };
            var descr = new ServiceDiscoveryModel {
                SpecVersion = specVersion,
                Device = device
            };
            return descr;
        }

        public static void PublishThisDevice() {
            var appPort = new AppConfiguration().Get().AntdUiPort;
            var localIps = IPv4.GetAllLocalAddress().Where(_ => _ != "127.0.0.1").OrderBy(_ => _).ToList();
            foreach(var ip in localIps) {
                var deviceDefinition = new SsdpRootDevice() {
                    CacheLifetime = TimeSpan.FromMinutes(60),
                    Uuid = MachineUid,
                    Location = new Uri($"http://{ip}:{appPort}/device/description"),
                    DeviceTypeNamespace = "antd",
                    ModelUrl = new Uri($"http://{ip}:{appPort}/"),
                    DeviceType = PartNumber,
                    FriendlyName = $"{ip} as {Hostname}",
                    Manufacturer = "Anthilla SRL",
                    ModelName = SerialNumber
                };
                _Publisher = new SsdpDevicePublisher();
                _Publisher.AddDevice(deviceDefinition);
            }
        }

        private SsdpDeviceLocator _DeviceLocator;

        // Call this method from somewhere in your code to start the search.
        public void BeginSearch() {
            _DeviceLocator = new SsdpDeviceLocator();
            // (Optional) Set the filter so we only see notifications for devices we care about 
            // (can be any search target value i.e device type, uuid value etc - any value that appears in the 
            // DiscoverdSsdpDevice.NotificationType property or that is used with the searchTarget parameter of the Search method).
            //_DeviceLocator.NotificationFilter = "upnp:rootdevice";
            // Connect our event handler so we process devices as they are found
            _DeviceLocator.DeviceAvailable += deviceLocator_DeviceAvailable;
            // Enable listening for notifications (optional)
            _DeviceLocator.StartListeningForNotifications();
            // Perform a search so we don't have to wait for devices to broadcast notifications 
            // again to get any results right away (notifications are broadcast periodically).
            _DeviceLocator.SearchAsync();
        }

        // Process each found device in the event handler
        public async static void deviceLocator_DeviceAvailable(object sender, DeviceAvailableEventArgs e) {
            try {
                //Device data returned only contains basic device details and location of full device description.
                Console.WriteLine("Found " + e.DiscoveredDevice.Usn + " at " + e.DiscoveredDevice.DescriptionLocation.ToString());

                //Can retrieve the full device description easily though.
                var fullDevice = await e.DiscoveredDevice.GetDeviceInfo();
                Console.WriteLine(fullDevice.FriendlyName);
                Console.WriteLine();
            }
            catch(Exception ex) {
                ConsoleLogger.Error(ex.Message);
                //continue;
            }
        }

        public static async Task<List<NodeModel>> Discover() {
            var list = new List<NodeModel>();

            using(var deviceLocator = new SsdpDeviceLocator()) {
                var foundDevices = await deviceLocator.SearchAsync();
                var uidRegex = new Regex("uuid\\:([a-zA-Z0-9\\-]+)\\:");
                var ipRegex = new Regex("([0-9]{0,3}\\.[0-9]{0,3}\\.[0-9]{0,3}\\.[0-9]{0,3})");
                foreach(var foundDevice in foundDevices) {
                    try {
                        var uid = foundDevice.Usn;
                        var device = new NodeModel();
                        device.RawUid = uid;
                        device.DescriptionLocation = foundDevice.DescriptionLocation.ToString();
                        device.PublicIp = ipRegex.Match(device.DescriptionLocation).Groups[1].Value;
                        var fullDevice = await foundDevice.GetDeviceInfo();
                        device.Hostname = fullDevice.FriendlyName;
                        device.DeviceType = fullDevice.DeviceType;
                        device.Manufacturer = fullDevice.Manufacturer;
                        device.ModelName = fullDevice.ModelName;
                        device.ModelDescription = fullDevice.ModelDescription;
                        device.ModelNumber = fullDevice.ModelNumber;
                        device.ModelUrl = device.DescriptionLocation.Replace("device/description", "");
                        device.SerialNumber = fullDevice.SerialNumber;
                        var services = new List<RssdpServiceModel>();
                        foreach(var foundService in fullDevice.Services) {
                            var svc = new RssdpServiceModel();
                            svc.ServiceType = foundService.ServiceType;
                            svc.ControlURL = foundService.ControlUrl?.ToString();
                        }
                        device.Services = services;
                        list.Add(device);
                    }
                    catch(Exception ex) {
                        ConsoleLogger.Error(ex.Message);
                        continue;
                    }
                }
            }
            var mergedList = new List<NodeModel>();
            if(!System.IO.File.Exists(CacheFile)) {
                mergedList = new List<NodeModel>();
            }
            else {
                var cachedText = System.IO.File.ReadAllText(CacheFile);
                if(string.IsNullOrEmpty(cachedText)) {
                    mergedList = new List<NodeModel>();
                }
                else {
                    mergedList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NodeModel>>(cachedText);
                }
            }
            var groupedList = list.GroupBy(_ => _.PublicIp).ToList();
            foreach(var group in groupedList) {
                var mergedNode = MergeUidInformation(group.ToList());
                var tryget = mergedList.FirstOrDefault(_ => _.MachineUid == mergedNode.MachineUid);
                if(tryget == null) {
                    mergedList.Add(mergedNode);
                }
            }
            System.IO.File.WriteAllText(CacheFile, Newtonsoft.Json.JsonConvert.SerializeObject(mergedList, Newtonsoft.Json.Formatting.Indented));
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

        public static List<RssdpServiceModel> GetServices() {
            var list = new List<RssdpServiceModel>();
            var appConfig = new AppConfiguration().Get();
            list.Add(new RssdpServiceModel { Name = "antd", Port = appConfig.AntdPort.ToString() });
            list.Add(new RssdpServiceModel { Name = "antdui", Port = appConfig.AntdUiPort.ToString() });
            list.Add(new RssdpServiceModel { Name = "antdfs", Port = "" });
            return list;
        }
    }
}
