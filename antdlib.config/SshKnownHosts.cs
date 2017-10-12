using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using anthilla.core;

namespace antdlib.config {
    public class SshKnownHosts {

        private readonly string _filePath = $"{Parameter.AntdCfg}/known_hosts";

        public List<string> Hosts { get; }

        public SshKnownHosts() {
            Hosts = !File.Exists(_filePath) ? new List<string>() : JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(_filePath));
        }

        public void Add(string host) {
            if(Hosts.Contains(host)) { return; }
            Hosts.Add(host);
            FileWithAcl.WriteAllText(_filePath, JsonConvert.SerializeObject(Hosts, Formatting.Indented), "644", "root", "wheel");
        }

        public void Remove(string host) {
            if(!Hosts.Contains(host)) { return; }
            Hosts.Remove(host);
            FileWithAcl.WriteAllText(_filePath, JsonConvert.SerializeObject(Hosts, Formatting.Indented), "644", "root", "wheel");
        }
    }
}
