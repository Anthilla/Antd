using System;
using antdlib.common;

namespace Antd.Users {
    public class SystemUser {

        public static void Create(string user) {
            Terminal.Execute($"useradd {user}");
        }

        public static void ResetPassword(string user, string password) {
            var hp = Terminal.Execute($"mkpasswd -m sha-512 {password}");
            Terminal.Execute($"usermod -p '{hp.Trim()}' {user}");
        }
    }
}