using System.IO;
using antdlib.common.Tool;

namespace Antd.Ssh {
    public class RootKeys {
        public string PathToPublic { get; }
        public string PathToPrivate { get; }
        public bool Exist { get; private set; }

        private readonly Bash _bash = new Bash();

        public RootKeys() {
            PathToPublic = "/root/.ssh/antd-root-key.pub";
            PathToPrivate = "/root/.ssh/antd-root-key";
        }

        public void CheckKeys() {
            Exist = File.Exists(PathToPublic) && File.Exists(PathToPrivate);
        }

        public void Create() {
            _bash.Execute("ssh-keygen -t rsa -N '' -f /root/.ssh/antd-root-key", false);
        }
    }
}
