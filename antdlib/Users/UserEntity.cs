using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using antdlib.Log;
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
                case "pin":
                    return ClaimType.UserPin;
                default:
                    return ClaimType.Other;
            }
        }

        public enum ClaimType : byte {
            UserIdentity = 1,
            UserPassword = 2,
            UserToken = 3,
            UserPin = 4,
            Other = 99
        }

        public static ClaimMode ConvertClaimMode(string claimString) {
            switch (claimString) {
                case "antd":
                    return ClaimMode.Antd;
                case "system":
                    return ClaimMode.System;
                case "activedirectory":
                    return ClaimMode.ActiveDirectory;
                case "anthillasp":
                    return ClaimMode.AnthillaSP;
                default:
                    return ClaimMode.Other;
            }
        }

        public enum ClaimMode : byte {
            Antd = 1,
            System = 2,
            ActiveDirectory = 3,
            AnthillaSP = 4,
            Other = 99
        }

        public class UserEntityModel {
            [Key]
            public string _Id { get; set; } = Guid.NewGuid().ToString();
            public Guid Guid => Guid.Parse(_Id);
            public string MasterGuid { get; set; }
            public string MasterUsername { get; set; }
            public string MasterAlias { get; set; }
            public bool IsEnabled { get; set; }
            public IEnumerable<Claim> Claims { get; set; }

            public string PublicKey { get; set; }
            public string PrivateKey { get; set; }

            public class Claim {
                public string ClaimGuid { get; set; }
                public string ClaimUserGuid { get; set; }
                public ClaimType Type { get; set; }
                public ClaimMode Mode { get; set; }
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

            public static string GenerateUserAlias(string identity) {
                var trySplit = identity.Split(' ');
                string stringAlias;
                if (trySplit.Length > 1) {
                    var last = trySplit[1].Length < 4 ? trySplit[1] : trySplit[1].Substring(0, 3);
                    var first = trySplit[0].Length < 4 ? trySplit[1] : trySplit[0].Substring(0, 3);
                    stringAlias = last + first;
                }
                else {
                    stringAlias = identity.Length < 7 ? identity : identity.Substring(0, 6);
                }
                var tryAlias = stringAlias + "01";
                var isUser = GetByUserIdentity(tryAlias);
                if (isUser == null) {
                    return tryAlias.ToLower();
                }
                var table = GetAll();
                var existingAlias = (from c in table
                                     where c.MasterAlias.Contains(stringAlias)
                                     orderby c.MasterAlias ascending
                                     select c.MasterAlias).ToArray();
                var lastAlias = existingAlias[existingAlias.Length - 1];
                var newNumber = (Convert.ToInt32(lastAlias.Substring(6, 2)) + 1).ToString("D2");
                return (stringAlias + newNumber).ToLower();
            }

            public static void Create(string guid, string username, string alias) {
                var user = new UserEntityModel {
                    MasterGuid = guid,
                    IsEnabled = true,
                    MasterUsername = username,
                    MasterAlias = alias,
                    Claims = new List<UserEntityModel.Claim>()
                };
                var keys = GenerateUsersKeys(guid, "rsa", "20048");
                user.PublicKey = keys.Item1;
                user.PrivateKey = keys.Item2;
                Session.New.Set(user);
            }

            public static void Create(string guid, string username, string alias, IEnumerable<UserEntityModel.Claim> claims) {
                var user = new UserEntityModel {
                    MasterGuid = guid,
                    IsEnabled = true,
                    MasterUsername = username,
                    MasterAlias = alias,
                    Claims = claims
                };
                var keys = GenerateUsersKeys(guid, "rsa", "20048");
                user.PublicKey = keys.Item1;
                user.PrivateKey = keys.Item2;
                Session.New.Set(user);
            }

            public static void AddClaim(string guid, ClaimType type, ClaimMode mode, string key, string value) {
                var user = Session.New.Get<UserEntityModel>().FirstOrDefault(_ => _.MasterGuid == guid);
                if (user == null)
                    return;
                var secureValue = type == ClaimType.UserPassword ? Cryptography.Hash256ToString(value) : value;
                var claim = new UserEntityModel.Claim {
                    ClaimGuid = Guid.NewGuid().ToString(),
                    ClaimUserGuid = guid,
                    Type = type,
                    Mode = mode,
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

            private static Tuple<string, string> GenerateUsersKeys(string userGuid, string type, string keyLenght) {
                try {
                    var keyrepo = Parameter.AntdCfgKeys;
                    Directory.CreateDirectory(keyrepo);
                    var userkeyrepo = keyrepo;
                    //var userkeyrepo = $"{keyrepo}/{userGuid}";
                    //Directory.CreateDirectory(userkeyrepo);
                    var c = $"ssh-keygen -t {type} -b {keyLenght} -P antd{userGuid} -C \"antd_{userGuid}_key\" -f {userkeyrepo}/key_{userGuid}";
                    ConsoleLogger.Warn(c);
                    Terminal.Terminal.Execute(c);
                    ConsoleLogger.Log($"keys for {userGuid} created");
                    var publicFile = $"{userkeyrepo}/key_{userGuid}.pub";
                    var privateFile = $"{userkeyrepo}/key_{userGuid}";
                    var publicBytes = File.ReadAllText(publicFile);
                    var privateBytes = File.ReadAllText(privateFile);
                    return new Tuple<string, string>(publicBytes, privateBytes);
                }
                catch (Exception ex) {
                    ConsoleLogger.Warn(ex.Message);
                    return new Tuple<string, string>("", "");
                }
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