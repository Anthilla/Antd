using antdlib.config;
using Rssdp;
using System;

namespace Antd.ServiceDiscovery {
    public class Rssdp {

        private static SsdpDevicePublisher _Publisher;
        private static string Hostname = Host2Configuration.Host.HostName;
        private static string PartNumber = Machine.MachineIds.Get.PartNumber;
        private static string SerialNumber = Machine.MachineIds.Get.SerialNumber.Replace("-", "").Substring(0, 16);
        private static string MachineUid = Machine.MachineIds.Get.MachineUid;

        public static void PublishThisDevice() {
            var deviceDefinition = new SsdpRootDevice() {
                CacheLifetime = TimeSpan.FromMinutes(60),
                Uuid = MachineUid,
                Location = new Uri("http://mydevice/descriptiondocument.xml"),
                DeviceTypeNamespace = "antd",
                DeviceType = PartNumber,
                FriendlyName = Hostname,
                Manufacturer = "Anthilla SRL",
                ModelName = SerialNumber
            };
            _Publisher = new SsdpDevicePublisher();
            _Publisher.AddDevice(deviceDefinition);
        }


        // Found uuid:a2061f6e-af29-4975-a65b-3b01409eee8c::upnp:rootdevice at http://mydevice/descriptiondocument.xml
        //Found uuid:a2061f6e-af29-4975-a65b-3b01409eee8c::pnp:rootdevice at http://mydevice/descriptiondocument.xml
        //Found uuid:a2061f6e-af29-4975-a65b-3b01409eee8c at http://mydevice/descriptiondocument.xml
        //Found uuid:a2061f6e-af29-4975-a65b-3b01409eee8c::urn:antd:device:08324d51-2d75-4db7-b59a-f08afb68252f:1 at http://mydevice/descriptiondocument.xml

        public static async void SearchForDevices() {
            using(var deviceLocator = new SsdpDeviceLocator()) {
                var foundDevices = await deviceLocator.SearchAsync();
                foreach(var foundDevice in foundDevices) {
                    Console.WriteLine("Found " + foundDevice.Usn + " at " + foundDevice.DescriptionLocation.ToString());
                    //var fullDevice = await foundDevice.GetDeviceInfo();
                    //Console.WriteLine(fullDevice.FriendlyName);
                }
            }
            Console.WriteLine("");
        }
    }
}
