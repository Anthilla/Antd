using System.Collections.Generic;

namespace antdlib.ProcFs {
    public struct NetArpEntry {
        private const string NetArpPath = ProcFs.RootPath + "/net/arp";

        public NetAddress Address { get; }
        public NetHardwareAddress HardwareAddress { get; }
        public string Mask { get; }
        public string Device { get; }

        public NetArpEntry(in NetAddress address, in NetHardwareAddress hardwareAddress, string mask, string device) {
            Address = address;
            HardwareAddress = hardwareAddress;
            Mask = mask;
            Device = device;
        }

        public override string ToString() => $"{Address.ToString()} {HardwareAddress.ToString()} {Mask} {Device}";

        internal static IEnumerable<NetArpEntry> Get() => Get(NetArpPath);

        internal static IEnumerable<NetArpEntry> Get(string netArpPath) {
            var statReader = new Utf8FileReader<X1024>(netArpPath);
            try {
                statReader.SkipLine();
                while (!statReader.EndOfStream) {
                    statReader.SkipWhiteSpaces();
                    var address = NetAddress.Parse(statReader.ReadWord(), NetAddressFormat.Human);
                    statReader.SkipWord();
                    statReader.SkipWord();
                    var hardwareAddress = NetHardwareAddress.Parse(statReader.ReadWord());
                    var maskBytes = statReader.ReadWord();
                    var mask = maskBytes.Length == 1 && maskBytes[0] == '*' ? "*" : maskBytes.ToUtf8String();
                    var device = statReader.ReadStringWord();
                    yield return new NetArpEntry(address, hardwareAddress, mask, device);
                }
            }
            finally {
                statReader.Dispose();
            }
        }
    }
}