using DeviceId;
using System;

namespace Antd2.cmds {
    public class Device {

        public static string LocalId { get { return GetLocalId(); } }
        public static string LocalName { get { return System.Environment.MachineName; } }

        private static string GetLocalId() {
            try {
                return new DeviceIdBuilder()
                   .AddProcessorId()
                   .AddMotherboardSerialNumber()
                   .ToString()
                   .FormatId();
            }
            catch (Exception) {
                if (System.IO.File.Exists("/etc/machine-id"))
                    return System.IO.File.ReadAllText("/etc/machine-id").FormatId();
                else
                    return Guid.Empty.ToString().FormatId();
            }
        }
    }
    public static class DeviceExtensions {

        public static string FormatId(this string deviceId) {
            var formatDeviceId = deviceId
                 .Trim()
                 .Replace(" ", "")
                 .Replace("-", "")
                 .Replace("_", "")
                 .ToUpperInvariant();
            formatDeviceId = formatDeviceId
                 .Substring(0, Math.Min(formatDeviceId.Length, 40));
            if (formatDeviceId.Length < 40)
                for (var i = 0; i < 40 - formatDeviceId.Length; i++)
                    formatDeviceId += "0";
            return formatDeviceId;
        }

    }
}
