using System.IO;

namespace Antd2.cmds {
    public class Ssh {

        private const string sshCommand = "ssh";
        private const string sshkeygenCommand = "ssh-keygen";
        private const string sshkeygenArg = "-t rsa -N '' -f ";

        private const string rootPublicKeyPath = "/root/.ssh/id_rsa.pub";
        private const string rootPrivateKeyPath = "/root/.ssh/id_rsa";

        public static string GetRootPublicKey() {
            if (!File.Exists(rootPublicKeyPath)) {
                return string.Empty;
            }
            return File.ReadAllText(rootPublicKeyPath);
        }

        public static string GetRootPrivateKey() {
            if (!File.Exists(rootPrivateKeyPath)) {
                return string.Empty;
            }
            return File.ReadAllText(rootPrivateKeyPath);
        }

        public static bool CreateRootKeys() {
            Bash.Do($"{sshkeygenCommand} {sshkeygenArg} {rootPrivateKeyPath}");
            return true;
        }

        public static bool RegenRootKeys() {
            File.Delete(rootPublicKeyPath);
            File.Delete(rootPrivateKeyPath);
            Bash.Do($"{sshkeygenCommand} {sshkeygenArg} {rootPrivateKeyPath}");
            return true;
        }

        //private const string authorizedKeysFile = "/root/.ssh/authorized_keys";
        //private const string sshKeyType = "ssh-rsa";

        //public static AuthorizedKey[] GetAuthorizedKey() {
        //    if(!File.Exists(authorizedKeysFile)) {
        //        return new AuthorizedKey[0];
        //    }
        //    var result = File.ReadAllLines(authorizedKeysFile).Where(_ => !string.IsNullOrEmpty(_) && _.StartsWith(sshKeyType) && _.Contains("@")).ToArray();
        //    var keys = new AuthorizedKey[result.Length];
        //    for(var i = 0; i < result.Length; i++) {
        //        var lineData = result[i].Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
        //        var userData = lineData[2].Split(new[] { '@' }, 2, StringSplitOptions.RemoveEmptyEntries);
        //        keys[i] = new AuthorizedKey() {
        //            User = userData[0],
        //            Host = userData[1],
        //            Key = CommonString.Append(lineData[0], " ", lineData[1])
        //        };
        //    }
        //    return keys;
        //}

        public static bool SetAuthorizedKey() {
            //if(Application.CurrentConfiguration.Services.Ssh.AuthorizedKey == null) {
            //    return false;
            //}
            //var keys = Application.CurrentConfiguration.Services.Ssh.AuthorizedKey;
            //var lines = new string[keys.Length];
            //for(var i = 0; i < keys.Length; i++) {
            //    var ak = keys[i];
            //    lines[i] = CommonString.Append(ak.Key, " ", ak.User, "@", ak.Host);
            //}
            //File.WriteAllLines(authorizedKeysFile, lines);
            return true;
        }
    }
}
