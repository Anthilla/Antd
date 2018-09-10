using Antd;
using anthilla.core;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AntdUi.Auth {
    public class UserDatabase : IUserMapper {

        private static IEnumerable<UserIdentity> Users() {
            var appusers = ApiConsumer.Get<ApplicativeUser[]>($"{Application.ServerUrl}/user/get/applicative");
            var users = new UserIdentity[appusers.Length];
            for(var i = 0; i < appusers.Length; i++) {
                users[i] = new UserIdentity() {
                    UserGuid = Guid.Parse("D059C902-C663-4D05-A2F5-4F09059695F8"),
                    UserIdentifier = Encryption.XHash(appusers[i].Id),
                    UserName = appusers[i].Id,
                    Claims = new string[0]
                };
            }
            return users;
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context) {
            var userRecord = Users().FirstOrDefault(u => u.UserGuid == identifier);
            return userRecord ?? new UserIdentity { UserName = userRecord.UserName };
        }

        public static Guid? ValidateUser(string userIdentity, string password) {
            var authResult = Help.Authenticate(userIdentity, password);
            return authResult == HttpStatusCode.OK ? (Guid?)Guid.Parse("D059C902-C663-4D05-A2F5-4F09059695F8") : null;
        }
    }
}