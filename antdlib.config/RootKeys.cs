using antdlib.common;
using System.IO;

namespace antdlib.config {
    public class RootKeys {
        public string PathToPublic { get; }
        public string PathToPrivate { get; }
        public bool Exists => File.Exists(PathToPublic) && File.Exists(PathToPrivate);

        public RootKeys() {
            PathToPublic = "/root/.ssh/id_rsa.pub";
            PathToPrivate = "/root/.ssh/id_rsa";
        }

        public void Create() {
            Bash.Execute("ssh-keygen -t rsa -N '' -f /root/.ssh/id_rsa", false);
        }
    }
}
