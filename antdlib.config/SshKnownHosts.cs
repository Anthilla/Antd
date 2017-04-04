using antdlib.common;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace antdlib.config {
    public class SshKnownHosts {

        private readonly string _filePath = $"{Parameter.AntdCfg}/known_hosts";
        private readonly string _filePathBackup = $"{Parameter.AntdCfg}/known_hosts.bck";

        public List<string> Hosts { get; }

        public SshKnownHosts() {
            Hosts = !File.Exists(_filePath) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(_filePath));
        }

        public void Add(string host) {
            if(Hosts.Contains(host)) { return; }
            Hosts.Add(host);
            if(File.Exists(_filePath)) {
                File.Copy(_filePath, _filePathBackup, true);
            }
            File.WriteAllText(_filePath, JsonConvert.SerializeObject(Hosts, Formatting.Indented));
        }

        public void Remove(string host) {
            if(!Hosts.Contains(host)) { return; }
            Hosts.Remove(host);
            if(File.Exists(_filePath)) {
                File.Copy(_filePath, _filePathBackup, true);
            }
            File.WriteAllText(_filePath, JsonConvert.SerializeObject(Hosts, Formatting.Indented));
        }
    }
}
