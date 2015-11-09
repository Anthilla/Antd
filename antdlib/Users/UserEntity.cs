using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using antdlib.Security;
using DeNSo;

namespace antdlib.Users {
    public class UserEntity {

        public static ClaimType ConvertClaimType(string claimString) {
            switch (claimString) {
                case "identity":
                    return ClaimType.UserIdentity;
                case "password":
                    return ClaimType.UserPassword;
                case "token":
                    return ClaimType.UserToken;
                default:
                    return ClaimType.Other;
            }
        }

        public enum ClaimType : byte {
            UserIdentity = 1,
            UserPassword = 2,
            UserToken = 3,
            Other = 99
        }

        public class UserEntityModel {
            [Key]
            public string _Id { get; set; } = Guid.NewGuid().ToString();
            public Guid Guid { get { return Guid.Parse(_Id); } }
            public string MasterGuid { get; set; }
            public string MasterUsername { get; set; }
            public bool IsEnabled { get; set; }
            public IEnumerable<Claim> Claims { get; set; }

            public class Claim {
                public string ClaimGuid { get; set; }
                public ClaimType Type { get; set; }
                public string Key { get; set; }
                public string Value { get; set; }
            }
        }

        public class Repository {
            public static IEnumerable<UserEntityModel> GetAll() {
                return Session.New.Get<UserEntityModel>().OrderBy(_ => _.MasterUsername);
            }

            public static IEnumerable<UserEntityModel> GetAllEnabled() {
                return Session.New.Get<UserEntityModel>().OrderBy(_ => _.MasterUsername).Where(_ => _.IsEnabled);
            }

            public static IEnumerable<UserEntityModel> GetAllDisabled() {
                return Session.New.Get<UserEntityModel>().OrderBy(_ => _.MasterUsername).Where(_ => _.IsEnabled == false);
            }

            public static UserEntityModel GetByUserIdentity(string userIdentity) {
                return Session.New.Get<UserEntityModel>().FirstOrDefault(_ => _.Claims.Any(c => c.Type == ClaimType.UserIdentity && c.Value == userIdentity));
            }

            public static string GenerateGuid() {
                return Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10).ToUpper();
            }

            public static void Create(string guid, string username) {
                var user = new UserEntityModel {
                    MasterGuid = guid,
                    IsEnabled = true,
                    MasterUsername = username,
                    Claims = new List<UserEntityModel.Claim>()
                };
                Session.New.Set(user);
            }

            public static void AddClaim(string guid, ClaimType type, string key, string value) {
                var user = Session.New.Get<UserEntityModel>().FirstOrDefault(_ => _.MasterGuid == guid);
                if (user == null)
                    return;
                var secureValue = (type == ClaimType.UserPassword) ? Cryptography.Hash256ToString(value) : value;
                var claim = new UserEntityModel.Claim {
                    ClaimGuid = Guid.NewGuid().ToString(),
                    Type = type,
                    Key = key,
                    Value = secureValue
                };
                var list = user.Claims.ToList();
                list.Add(claim);
                user.Claims = list;
                Session.New.Set(user);
            }

            public static void RemoveClaim(string guid, string claimGuid) {
                var user = Session.New.Get<UserEntityModel>().FirstOrDefault(_ => _.MasterGuid == guid);
                var claim = user?.Claims.First(_ => _.ClaimGuid == claimGuid);
                if (claim == null)
                    return;
                var list = user.Claims.ToList();
                list.Remove(claim);
                user.Claims = list;
                Session.New.Set(user);
            }

            public static void Enable(string guid) {
                var user = Session.New.Get<UserEntityModel>().FirstOrDefault(_ => _.MasterGuid == guid);
                if (user == null)
                    return;
                user.IsEnabled = true;
                Session.New.Set(user);
            }

            public static void Disable(string guid) {
                var user = Session.New.Get<UserEntityModel>().FirstOrDefault(_ => _.MasterGuid == guid);
                if (user == null)
                    return;
                user.IsEnabled = false;
                Session.New.Set(user);
            }

            public static void Delete(string guid) {
                var user = Session.New.Get<UserEntityModel>().FirstOrDefault(_ => _.MasterGuid == guid);
                Session.New.Delete(user);
            }
        }

        public enum AuthenticationStatus : byte {
            Ok = 0,
            UserDoesNotExists = 1,
            UserNotEnabled = 2,
            WrongCredential = 3,
            WrongPassword = 4,
            Error = 99
        }

        public class Manage {
            public static KeyValuePair<AuthenticationStatus, Guid?> AuthenticatePassword(string userIdentity, string password) {
                var user = Repository.GetByUserIdentity(userIdentity);
                if (user == null)
                    return new KeyValuePair<AuthenticationStatus, Guid?>(AuthenticationStatus.UserDoesNotExists, null);
                if (user.IsEnabled == false)
                    return new KeyValuePair<AuthenticationStatus, Guid?>(AuthenticationStatus.UserNotEnabled, null);
                return !user.Claims.Where(_ => _.Type == ClaimType.UserPassword).Select(_ => _.Value).ToList().Contains(Cryptography.Hash256ToString(password)) ? new KeyValuePair<AuthenticationStatus, Guid?>(AuthenticationStatus.WrongPassword, null) : new KeyValuePair<AuthenticationStatus, Guid?>(AuthenticationStatus.Ok, user.Guid);
            }

            public static KeyValuePair<AuthenticationStatus, Guid?> Authenticate(string userIdentity, string claimToMatch) {
                var user = Repository.GetByUserIdentity(userIdentity);
                if (user == null)
                    return new KeyValuePair<AuthenticationStatus, Guid?>(AuthenticationStatus.UserDoesNotExists, null);
                if (user.IsEnabled == false)
                    return new KeyValuePair<AuthenticationStatus, Guid?>(AuthenticationStatus.UserNotEnabled, null);
                return !user.Claims.Select(_ => _.Value).ToList().Contains(claimToMatch) ? new KeyValuePair<AuthenticationStatus, Guid?>(AuthenticationStatus.WrongCredential, null) : new KeyValuePair<AuthenticationStatus, Guid?>(AuthenticationStatus.Ok, user.Guid);
            }

            public static KeyValuePair<AuthenticationStatus, Guid?> Authenticate(string userIdentity, IEnumerable<string> claimsToMatch) {
                var user = Repository.GetByUserIdentity(userIdentity);
                if (user == null)
                    return new KeyValuePair<AuthenticationStatus, Guid?>(AuthenticationStatus.UserDoesNotExists, null);
                if (user.IsEnabled == false)
                    return new KeyValuePair<AuthenticationStatus, Guid?>(AuthenticationStatus.UserNotEnabled, null);
                return !user.Claims.Select(_ => _.Value).ToList().Intersect(claimsToMatch).Any() ? new KeyValuePair<AuthenticationStatus, Guid?>(AuthenticationStatus.WrongCredential, null) : new KeyValuePair<AuthenticationStatus, Guid?>(AuthenticationStatus.Ok, user.Guid);
            }
        }
    }
}