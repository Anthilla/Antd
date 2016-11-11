using antdlib.common.Tool;

namespace Antd.Users {
    public class SystemUser {

        public void Create(string user) {
            var bash = new Bash();
            bash.Execute($"useradd {user}", false);
        }

        public void SetPassword(string user, string password) {
            if(string.IsNullOrEmpty(user)) {
                return;
            }
            if(string.IsNullOrEmpty(password)) {
                return;
            }
            var bash = new Bash();
            bash.Execute($"usermod -p '{password}' {user}", false);
        }
    }
}