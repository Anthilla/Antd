using DeviceId;
using DeviceId.Encoders;
using DeviceId.Formatters;

namespace Antd2.cmds {
    public class Device {

        public static string LocalId { get { return GetLocalId(); } }
        public static string LocalName { get { return System.Environment.MachineName; } }

        private static string GetLocalId() {
            return new DeviceIdBuilder()
                //.AddMacAddress()
                //.AddProcessorId()
                //.AddMotherboardSerialNumber()
                .ToString()
                .ToUpperInvariant();
        }
    }
}
