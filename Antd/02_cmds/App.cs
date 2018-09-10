using anthilla.core;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Antd.cmds {

    public class App {

        private const string appRepository = "/mnt/cdrom/Apps/";

        private const string app_anthillaCore = "AnthillaCore";
        private const string app_anthillaSp = "AnthillaSP";
        private const string app_anthillaDemo = "AnthillaDemo";
        private const string app_hoplite = "Hoplite";
        private const string app_sanist = "Sanist";
        private const string app_scighera = "Scighera";

        private static string[] APPS = new string[] {
            "AnthillaCore",
            "AnthillaSP",
            "AnthillaDemo",
            "Hoplite",
            "Sanist",
            "Scighera"
        };

        private const string unitPrefix = "app-";
        private const string unitSuffix01 = "-01-prepare";
        private const string unitSuffix02 = "-02-mount";
        private const string unitSuffix03 = "-03-launcher";

        /// <summary>
        /// Ottengo la lista delle applicazioni presenti
        /// </summary>
        /// <returns></returns>
        public static LocalApplication[] GetLocal() {
            var localApps = new List<LocalApplication>();
            for(var i = 0; i < APPS.Length; i++) {
                var appDir = CommonString.Append(appRepository, APPS[i]);
                if(Directory.Exists(appDir)) {
                    localApps.Add(new LocalApplication() { Name = APPS[i].ToLowerInvariant() });
                }
            }
            var apps = localApps.ToArray();
            for(var i = 0; i < apps.Length; i++) {
                apps[i].Prepared = Systemctl.IsActive(CommonString.Append(unitPrefix, apps[i].Name, unitSuffix01));
                apps[i].Mounted = Systemctl.IsActive(CommonString.Append(unitPrefix, apps[i].Name, unitSuffix02));
                apps[i].Launched = Systemctl.IsActive(CommonString.Append(unitPrefix, apps[i].Name, unitSuffix03));
                apps[i].LauncherUnit = CommonString.Append(unitPrefix, apps[i].Name, unitSuffix03);
                apps[i].Status = Systemctl.Status(CommonString.Append(unitPrefix, apps[i].Name, unitSuffix03)).ToArray();
            }
            return apps;
        }
    }
}
