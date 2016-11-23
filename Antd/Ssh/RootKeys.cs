using System.IO;
using antdlib.common.Tool;

namespace Antd.Ssh {
    public class RootKeys {
        public string PathToPublic { get; }
        public string PathToPrivate { get; }
        public bool Exists => File.Exists(PathToPublic) && File.Exists(PathToPrivate);

        private readonly Bash _bash = new Bash();

        public RootKeys() {
            PathToPublic = "/root/.ssh/id_rsa.pub";
            PathToPrivate = "/root/.ssh/id_rsa";
        }

        public void Create() {
            _bash.Execute("ssh-keygen -t rsa -N '' -f /root/.ssh/antd-root-key", false);
        }
    }
}
