using System.IO;
using antdlib.common;

namespace Antd.Ssh {
    public class RootKeys {
        public string PathToPublic { get; private set; }
        public string PathToPrivate { get; private set; }
        public bool Exist { get; private set; } = false;

        public RootKeys() {
            PathToPublic = "/root/.ssh/antd-root-key.pub";
            PathToPrivate = "/root/.ssh/antd-root-key";
        }

        public void CheckKeys() {
            Exist = File.Exists(PathToPublic) && File.Exists(PathToPrivate);
        }

        public void Create() {
            Bash.Execute("ssh-keygen -t rsa -N '' -f /root/.ssh/antd-root-key");
        }
    }
}
