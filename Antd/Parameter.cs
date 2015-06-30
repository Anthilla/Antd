using System;
using System.IO;

namespace Antd {

    public class Folder {
        public static string Root {get{return AppDomain.CurrentDomain.BaseDirectory;}}
        public static string Config { get { return Path.Combine(Folder.Root, "config"); } }
        public static string Database { get { return Path.Combine(Folder.Root, "database"); } }
        public static string Networkd { get { return Path.Combine(Folder.Root, "networkd"); } }
        public static string Apps { get { return "/mnt/cdrom/Apps"; } }
        public static string AppsUnits { get { return "/mnt/cdrom/Units/applicative.target.wants"; } }
    }
}
