using DeNSo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace antdlib.Database {
    public class SecureDb {
        /// <summary>
        /// Usage:
        /// in your database method use this line
        ///     SecureDB.*Action*<*type*>(*model*);
        /// where *Action* could be any of the "CRUD" methods listed below in this class (eg Get, GetBy, Create, Edit, Delete)
        /// where *type* is the type of *model*, this will define the "table" of database where your info will be saved
        /// where *model* could be any of your custom model, altready initialized, with the info you want to save
        /// </summary>

        public static IEnumerable<T> Get<T>() where T : SecureEntity, new() {
            using (var session = Session.New) {
                try {
                    var encryptedResult = session.Get<T>(t => t.Status != EntityStatus.Delete);
                    session.Dispose();
                    var result = new List<T>();
                    foreach (var encrypted in encryptedResult) {
                        T item = Encryption.XDecrypt<T>(encrypted.Dump, encrypted.EntityKey, encrypted.EntityVector);
                        result.Add(item);
                    }
                    return result;
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
                    var result = new List<T>();
                    foreach (var encrypted in encryptedResult) {
                        T item = Encryption.XDecrypt<T>(encrypted.Dump, encrypted.EntityKey, encrypted.EntityVector);
                        result.Add(item);
                    }
                    return result;
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
                    return Encryption.XDecrypt<T>(result.Dump, result.EntityKey, result.EntityVector);
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
                    return Encryption.XDecrypt<T>(result.Dump, result.EntityKey, result.EntityVector);
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
                    var secureType = new T();
                    secureType.EntityKey = Encryption.RandomKey;
                    secureType.EntityVector = Encryption.RandomVector;
                    secureType.Dump = Encryption.XEncrypt(type, secureType.EntityKey, secureType.EntityVector);
                    session.Set(secureType);
                }
                session.Dispose();
            }
        }

        public static void Create<T>(IEnumerable<T> types) where T : SecureEntity, new() {
            using (var session = Session.New) {
                foreach (T type in types) {
                    var check = session.Get<T>(t => t.Status != EntityStatus.Delete && t == type).FirstOrDefault();
                    if (type != null && check == null) {
                        var secureType = new T();
                        secureType.EntityKey = Encryption.RandomKey;
                        secureType.EntityVector = Encryption.RandomVector;
                        secureType.Dump = Encryption.XEncrypt(type, secureType.EntityKey, secureType.EntityVector);
                        session.Set(secureType);
                    }
                    session.Dispose();
                }
            }
        }

        public static void Edit<T>(T type) where T : SecureEntity, new() {
            if (type != null) {
                using (var session = Session.New) {
                    var existing = session.Get<T>(t => t.Status != EntityStatus.Delete && t._Id == type._Id).FirstOrDefault();
                    existing._Id = EntityStatus.Delete.ToString();
                    existing.Timestamp = EntityStatus.Delete.ToString();
                    existing.Status = EntityStatus.Delete;
                    session.Set(existing);
                    type.Timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                    type.Status = EntityStatus.New;
                    var secureType = new T();
                    secureType.EntityKey = Encryption.RandomKey;
                    secureType.EntityVector = Encryption.RandomVector;
                    secureType.Dump = Encryption.XEncrypt(type, secureType.EntityKey, secureType.EntityVector);
                    session.Set(secureType);
                    session.Dispose();
                }
            }
        }

        public static void Edit<T>(IEnumerable<T> types) where T : SecureEntity, new() {
            foreach (T type in types) {
                if (type != null) {
                    using (var session = Session.New) {
                        var existing = session.Get<T>(t => t.Status != EntityStatus.Delete && t._Id == type._Id).FirstOrDefault();
                        if (existing == null) {
                            throw new ArgumentNullException();
                        }
                        existing._Id = EntityStatus.Delete.ToString();
                        existing.Timestamp = EntityStatus.Delete.ToString();
                        existing.Status = EntityStatus.Delete;
                        session.Set(existing);
                        type.Timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                        type.Status = EntityStatus.New;
                        var secureType = new T() {
                            EntityKey = Encryption.RandomKey,
                            EntityVector = Encryption.RandomVector,
                        };
                        secureType.Dump = Encryption.XEncrypt(type, secureType.EntityKey, secureType.EntityVector);
                        session.Set(secureType);
                        session.Dispose();
                    }
                }
            }
        }

        public static void Delete<T>(string id) where T : SecureEntity, new() {
            using (var session = Session.New) {
                var existing = session.Get<T>(t => t.Status != EntityStatus.Delete && t._Id == id).FirstOrDefault();
                if (existing == null) {
                    throw new ArgumentNullException();
                }
                existing._Id = EntityStatus.Delete.ToString();
                existing.Timestamp = EntityStatus.Delete.ToString();
                existing.Status = EntityStatus.Delete;
                session.Set(existing);
                session.Dispose();
            }
        }

        public static void Delete<T>(IEnumerable<string> ids) where T : SecureEntity, new() {
            foreach (var id in ids) {
                using (var session = Session.New) {
                    var existing = session.Get<T>(t => t.Status != EntityStatus.Delete && t._Id == id).FirstOrDefault();
                    if (existing == null) {
                        throw new ArgumentNullException();
                    }
                    existing._Id = EntityStatus.Delete.ToString();
                    existing.Timestamp = EntityStatus.Delete.ToString();
                    existing.Status = EntityStatus.Delete;
                    session.Set(existing);
                    session.Dispose();
                }
            }
        }
    }
}
