using DeNSo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace antdlib.Database {
    public class SecureDb {
        public static IEnumerable<T> Get<T>() where T : SecureEntity, new() {
            using (var session = Session.New) {
                try {
                    var encryptedResult = session.Get<T>(t => t.Status != EntityStatus.Delete);
                    session.Dispose();
                    return encryptedResult.Select(encrypted => Encryption.XDecrypt<T>(encrypted.Dump, encrypted.EntityKey, encrypted.EntityVector)).ToList();
                }
                catch (Exception) {
                    return null;
                }
            }
        }

        public static IEnumerable<T> Get<T>(Func<T, bool> predicate) where T : SecureEntity, new() {
            using (var session = Session.New) {
                try {
                    var encryptedResult = session.Get<T>(t => t.Status != EntityStatus.Delete).Where(predicate);
                    session.Dispose();
                    return encryptedResult.Select(encrypted => Encryption.XDecrypt<T>(encrypted.Dump, encrypted.EntityKey, encrypted.EntityVector)).ToList();
                }
                catch (Exception) {
                    return null;
                }
            }
        }

        public static T GetBy<T>(string id) where T : SecureEntity, new() {
            using (var session = Session.New) {
                try {
                    var result = session.Get<T>(t => t.Status != EntityStatus.Delete && t._Id == id).FirstOrDefault();
                    session.Dispose();
                    return result != null ? Encryption.XDecrypt<T>(result.Dump, result.EntityKey, result.EntityVector) : null;
                }
                catch (Exception) {
                    return null;
                }
            }
        }

        public static T GetBy<T>(Func<T, bool> predicate) where T : SecureEntity, new() {
            using (var session = Session.New) {
                try {
                    var result = session.Get<T>(t => t.Status != EntityStatus.Delete).Where(predicate).FirstOrDefault();
                    session.Dispose();
                    return result != null ? Encryption.XDecrypt<T>(result.Dump, result.EntityKey, result.EntityVector) : null;
                }
                catch (Exception) {
                    return null;
                }
            }
        }

        public static void Create<T>(T type) where T : SecureEntity, new() {
            using (var session = Session.New) {
                T check;
                try {
                    check = session.Get<T>(t => t.Status != EntityStatus.Delete && t == type).FirstOrDefault();
                }
                catch (Exception) {
                    check = null;
                }
                if (type != null && check == null) {
                    var secureType = new T {
                        EntityKey = Encryption.RandomKey,
                        EntityVector = Encryption.RandomVector
                    };
                    secureType.Dump = Encryption.XEncrypt(type, secureType.EntityKey, secureType.EntityVector);
                    session.Set(secureType);
                }
                session.Dispose();
            }
        }

        public static void Create<T>(IEnumerable<T> types) where T : SecureEntity, new() {
            foreach (var type in types) {
                Create(type);
            }
        }

        public static void Edit<T>(T type) where T : SecureEntity, new() {
            if (type == null)
                return;
            using (var session = Session.New) {
                var existing = session.Get<T>(t => t.Status != EntityStatus.Delete && t._Id == type._Id).FirstOrDefault();
                if (existing != null) {
                    existing._Id = EntityStatus.Delete.ToString();
                    existing.Timestamp = EntityStatus.Delete.ToString();
                    existing.Status = EntityStatus.Delete;
                    session.Set(existing);
                }
                type.Timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                type.Status = EntityStatus.New;
                var secureType = new T {
                    EntityKey = Encryption.RandomKey,
                    EntityVector = Encryption.RandomVector
                };
                secureType.Dump = Encryption.XEncrypt(type, secureType.EntityKey, secureType.EntityVector);
                session.Set(secureType);
                session.Dispose();
            }
        }

        public static void Edit<T>(IEnumerable<T> types) where T : SecureEntity, new() {
            foreach (var type in types) {
                Edit(type);
            }
        }

        public static void Delete<T>(string id) where T : SecureEntity, new() {
            using (var session = Session.New) {
                var existing = session.Get<T>(t => t.Status != EntityStatus.Delete && t._Id == id).FirstOrDefault();
                if (existing == null)
                    return;
                existing._Id = EntityStatus.Delete.ToString();
                existing.Timestamp = EntityStatus.Delete.ToString();
                existing.Status = EntityStatus.Delete;
                session.Set(existing);
                session.Dispose();
            }
        }

        public static void Delete<T>(IEnumerable<string> ids) where T : SecureEntity, new() {
            foreach (var id in ids) {
                Delete<T>(id);
            }
        }
    }
}
