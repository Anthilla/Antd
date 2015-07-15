using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antdlib {
    public class antdconst {
        public class Folder {
            public static string Root { get { return "/cfg/antd"; } }
            public static string Config { get { return Folder.Root + "/config"; } }
            public static string Database { get { return Folder.Root + "/database"; } }
            public static string FileRepository { get { return Folder.Root + "/files"; } }
            public static string Networkd { get { return Folder.Root + "/networkd"; } }
            public static string Apps { get { return "/mnt/cdrom/Apps"; } }
            public static string AppsUnits { get { return "/mnt/cdrom/Units/applicative.target.wants"; } }
        }
    }
}
