using System.IO;

namespace Antd.UnitFiles {
    public class UnitFile {
        public static void Write(string name) {
            UnitModel unitModel = UnitRepo.GetInfo(name);
            string folder = "/cfg/anthilla.units.d/";
            System.IO.Directory.CreateDirectory(folder);
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
                    sw.WriteLine("Alias=" + unitModel.alias);
                    sw.WriteLine("");
                }
            }
            Command.Launch("chmod", "777 " + path);
        }

        public static void WriteForSelf() {
            string folder = "/cfg/anthilla.units.d/";
            Directory.CreateDirectory(folder);
            string file = "antd.service";
            string path = Path.Combine(folder, file);
            if (!File.Exists(path)) {
                using (StreamWriter sw = File.CreateText(path)) {
                    sw.WriteLine("[Unit]");
                    sw.WriteLine("Description=antd.service");
                    sw.WriteLine("");
                    sw.WriteLine("[Service]");
                    sw.WriteLine("TimeoutStartSec=0");
                    sw.WriteLine("ExecStartPre=mount /mnt/cdrom/DIR_usr_lib64_mono.squash.xz /usr/lib64/mono");
                    sw.WriteLine("ExecStartPre=mkdir /antd");
                    sw.WriteLine("ExecStartPre=mount mount /mnt/cdrom/DIR_antd_2015.squash.xz /antd");
                    sw.WriteLine("ExecStartPre=#mount -t tmpfs none /antd/cfg");
                    sw.WriteLine("ExecStartPre=mono /antd/Antd/Antd.exe");
                    sw.WriteLine("ExecStart=/usr/bin/mono /antd/Antd/start.antd.sh");
                    sw.WriteLine("");
                    sw.WriteLine("[Install]");
                    sw.WriteLine("Alias=antd.service");
                    sw.WriteLine("");
                }
            }
            Command.Launch("chmod", "777 " + path);
        }
    }
}