using System;
using antdlib.common;

namespace Antd.Users {
    public class SystemUser {

        public static void Create(string user) {
            Terminal.Execute($"useradd {user}");
        }

        public static void SetPassword(string user, string password) {
            if (string.IsNullOrEmpty(user)) {
                return;
            }
            if (string.IsNullOrEmpty(password)) {
                return;
            }
            Terminal.Execute($"usermod -p '{password}' {user}");
        }
    }
}