using DeNSo;
//using DeNSo.P2P;
using System;
using System.Collections.Generic;
using System.Linq;
//using System.Net.PeerToPeer;

namespace antdlib.Database {
    public class Db {
        public static void Start() {
            Configuration.BasePath = new[] { "/test" };
            Session.DefaultDataBase = "db";

            //todo test p2p
            //EventP2PDispatcher.EnableP2PEventMesh();
            //EventP2PDispatcher.MakeNodeAvaiableToPNRP(Cloud.Available);

            Session.Start();
        }

        public static IEnumerable<T> Get<T>() where T : Entity, new() {
            using (var session = Session.New) {
                try {
                    var result = session.Get<T>(t => t.Status != EntityStatus.Delete);
                    session.Dispose();
                    return result;
                }
                catch (Exception) {
                    return null;
                }
            }
        }

        public static IEnumerable<T> Get<T>(Func<T, bool> predicate) where T : Entity, new() {
            using (var session = Session.New) {
                try {
                    var result = session.Get<T>(t => t.Status != EntityStatus.Delete).Where(predicate);
                    session.Dispose();
                    return result;
                }
                catch (Exception) {
                    return null;
                }
            }
        }

        public static T GetBy<T>(string id) where T : Entity, new() {
            using (var session = Session.New) {
                try {
                    var result = session.Get<T>(t => t.Status != EntityStatus.Delete && t._Id == id).FirstOrDefault();
                    session.Dispose();
                    return result;
                }
                catch (Exception) {
                    return null;
                }
            }
        }

        public static T GetBy<T>(Func<T, bool> predicate) where T : Entity, new() {
            using (var session = Session.New) {
                try {
                    var result = session.Get<T>(t => t.Status != EntityStatus.Delete).Where(predicate).FirstOrDefault();
                    session.Dispose();
                    return result;
                }
                catch (Exception) {
                    return null;
                }
            }
        }

        public static void Create<T>(T type) where T : Entity, new() {
            using (var session = Session.New) {
                T check;
                try {
                    check = session.Get<T>(t => t.Status != EntityStatus.Delete && t == type).FirstOrDefault();
                }
                catch (Exception) {
                    check = null;
                }
                if (type != null && check == null) {
                    session.Set(type);
                }
                session.Dispose();
            }
        }

        public static void Create<T>(IEnumerable<T> types) where T : Entity, new() {
            using (var session = Session.New) {
                foreach (var type in types) {
                    var check = session.Get<T>(t => t.Status != EntityStatus.Delete && t == type).FirstOrDefault();
                    if (type != null && check == null) {
                        session.Set(type);
                    }
                    session.Dispose();
                }
            }
        }

        public static void Edit<T>(T type) where T : Entity, new() {
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
                    session.Set(type);
                    session.Dispose();
                }
            }
        }

        public static void Edit<T>(IEnumerable<T> types) where T : Entity, new() {
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
                        session.Set(type);
                        session.Dispose();
                    }
                }
            }
        }

        public static void Delete<T>(string id) where T : Entity, new() {
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

        public static void Delete<T>(IEnumerable<string> ids) where T : Entity, new() {
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
