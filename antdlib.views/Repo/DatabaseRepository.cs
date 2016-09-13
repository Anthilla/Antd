using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;

namespace antdlib.views.Repo {
    public class DatabaseRepository {

        public static IEnumerable<T> Query<T>(RaptorDB.RaptorDB rdb, string viewName) {
            var result = rdb.Query(viewName);
            var list = result.Rows.Select(_ => (T)_);
            return list;
        }

        public static IEnumerable<T> Query<T>(RaptorDB.RaptorDB rdb, string viewName, Func<T, bool> predicate) {
            var result = rdb.Query(viewName);
            var list = result.Rows.Select(_ => (T)_);
            return list.Where(predicate);
        }

        public static bool Save<T>(RaptorDB.RaptorDB rdb, T model, bool useEncryption = false) where T : EntityModel, new() {
            var modelToSave = model;
            modelToSave.IsEncrypted = false;
            if (!useEncryption) {
                return rdb.Save(modelToSave.Id, modelToSave);
            }
            modelToSave = new T {
                Id = model.Id,
                Guid = model.Guid,
                Status = model.Status,
                Tags = new List<string>(),
                Key = model.Key,
                Vector = model.Vector,
                IsEncrypted = true
            };
            var dumpData = Encryption.DbEncrypt(model, model.Key.ToKey(), model.Vector.ToVector());
            modelToSave.Dump = dumpData;
            return rdb.Save(modelToSave.Id, modelToSave);
        }

        public static bool Delete<T>(RaptorDB.RaptorDB rdb, Guid modelGuid) where T : EntityModel, new() {
            var model = (T)rdb.Fetch(modelGuid);
            model.Status = EntityStatus.Delete;
            return rdb.Save(model.Id, model);
        }

        public static bool Edit<T>(RaptorDB.RaptorDB rdb, T modelUpdate, bool useEncryption = false) where T : EntityModel, new() {
            var model = (T)rdb.Fetch(modelUpdate.Id);
            if (model == null) {
                return Save(rdb, modelUpdate, useEncryption);
            }
            if (model.IsEncrypted) {
                var k = model.Key.ToKey();
                var v = model.Vector.ToVector();
                var encryptedModel = model.Dump;
                model = Encryption.DbDecrypt<T>(encryptedModel, k, v);
            }
            model.Status = EntityStatus.Delete;
            var oldSave = Save(rdb, model, useEncryption);
            var newModel = modelUpdate.UpdatePropertiesOf(model);
            newModel.Id = Guid.NewGuid();
            newModel.Guid = model.Guid;
            newModel.Status = EntityStatus.New;
            var newSave = Save(rdb, newModel, useEncryption);
            return oldSave && newSave;
        }

        public static bool SetFastObject(RaptorDB.RaptorDB rdb, string key, object obj) {
            var kv = rdb.GetKVHF();
            return kv.SetObjectHF(key, obj);
        }

        public static object GetFastObject(RaptorDB.RaptorDB rdb, string key) {
            var kv = rdb.GetKVHF();
            return kv.GetObjectHF(key);
        }

        public static T GetFastObject<T>(RaptorDB.RaptorDB rdb, string key) {
            var kv = rdb.GetKVHF();
            return (T)kv.GetObjectHF(key);
        }
    }
}
