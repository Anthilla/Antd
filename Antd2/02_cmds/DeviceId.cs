using DeviceId;

namespace Antd2.cmds {
    public class DeviceId {

        public static string LocalId { get { return GetLocalId(); } }
        public static string LocalName { get { return System.Environment.MachineName; } }

        private static string GetLocalId() {
            return new DeviceIdBuilder()
                .AddProcessorId()
                .AddMotherboardSerialNumber()
                .ToString()
                .ToUpperInvariant();
        }
    }
}
