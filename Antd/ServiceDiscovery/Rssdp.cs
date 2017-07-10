using antdlib.config;
using antdlib.config.shared;
using antdlib.models;
using anthilla.core;
using Rssdp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Antd.ServiceDiscovery {
    public class Rssdp {

        private static SsdpDevicePublisher _Publisher;
        private static string Hostname = Host2Configuration.Host.HostName;
        private static string PartNumber = Machine.MachineIds.Get.PartNumber;
        private static string SerialNumber = Machine.MachineIds.Get.SerialNumber.Replace("-", "").Substring(0, 16);
        private static string MachineUid = Machine.MachineIds.Get.MachineUid;

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
            foreach(var ip in IPv4.GetAllLocalAddress()) {
                var deviceDefinition = new SsdpRootDevice() {
                    CacheLifetime = TimeSpan.FromMinutes(60),
                    Uuid = MachineUid,
                    Location = new Uri($"http://{ip}:{appPort}/device/description"),
                    DeviceTypeNamespace = "antd",
                    DeviceType = PartNumber,
                    FriendlyName = Hostname,
                    Manufacturer = "Anthilla SRL",
                    ModelName = SerialNumber
                };
                _Publisher = new SsdpDevicePublisher();
                _Publisher.AddDevice(deviceDefinition);
            }
        }

        public static async Task<List<RssdpDeviceModel>> Discover() {
            var list = new List<RssdpDeviceModel>();
            using(var deviceLocator = new SsdpDeviceLocator()) {
                var foundDevices = await deviceLocator.SearchAsync();
                foreach(var foundDevice in foundDevices) {
                    try {
                        var device = new RssdpDeviceModel();
                        device.MachineUid = foundDevice.Usn;
                        device.DescriptionLocation = foundDevice.DescriptionLocation.ToString();
                        var fullDevice = await foundDevice.GetDeviceInfo();
                        device.DeviceType = fullDevice.DeviceType;
                        device.FriendlyName = fullDevice.FriendlyName;
                        device.Manufacturer = fullDevice.Manufacturer;
                        device.ModelName = fullDevice.ModelName;
                        device.ModelDescription = fullDevice.ModelDescription;
                        device.ModelNumber = fullDevice.ModelNumber;
                        device.SerialNumber = fullDevice.SerialNumber;
                        var services = new List<RssdpServiceModel>();
                        foreach(var foundService in fullDevice.Services) {
                            var svc = new RssdpServiceModel();
                            svc.ServiceType = foundService.ServiceType;
                            svc.ControlURL = foundService.ControlUrl.ToString();
                        }
                        device.Services = services;
                    }
                    catch(Exception ex) {
                        ConsoleLogger.Error(ex.Message);
                        continue;
                    }
                }
            }
            return list;
        }
    }
}
