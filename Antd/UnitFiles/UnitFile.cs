using System.IO;

namespace Antd.UnitFiles {
    public class UnitFile {
        public static void Write(string name) {
            UnitModel unitModel = UnitRepo.GetInfo(name);
            string folder = "/cfg/anthilla.units/";
            Directory.CreateDirectory(folder);
            string file = unitModel.description;
            string path = Path.Combine(folder, file);
            if (!File.Exists(path)) {
                using (StreamWriter sw = File.CreateText(path)) {
                    sw.WriteLine("[Unit]");
                    sw.WriteLine("Description=" + unitModel.description);
                    sw.WriteLine("");
                    sw.WriteLine("[Service]");
                    sw.WriteLine("TimeoutStartSec=" + unitModel.timeOutStartSec);
                    sw.WriteLine("ExecStart=" + unitModel.execStart);
                    sw.WriteLine("");
                    sw.WriteLine("[Install]");
                    sw.WriteLine("Alias=" + unitModel.wantedBy);
                    sw.WriteLine("");
                }
            }
            Command.Launch("chmod", "777 " + path);
        }
    }
}
