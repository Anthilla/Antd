//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;

//namespace Antd.Security {

//    public class KeyIdentity_Model {

//        [Key]
//        public string _Id { get; set; }

//        public string KeyGuid { get; set; }

//        public string UserGuid { get; set; }

//        public AnthillaRsaKeys UserKey { get; set; }
//    }

//    public class KeyIdentity_Repository {
//        public IEnumerable<KeyIdentity_Model> KeyIdentity_Table = DeNSo.Session.New.Get<KeyIdentity_Model>(model => model.IsDeleted == false).ToArray();

//        public IEnumerable<KeyIdentity_Model> GetAll() {
//            Anth_Log.TraceEvent("KeyIdentity_ Management", "Information", "tmp", "KeyIdentity_ getalldec");
//            return KeyIdentity_Table;
//        }

//        public KeyIdentity_Model GetById(string id) {
//            var item = DeNSo.Session.New.Get<KeyIdentity_Model>(c => c.KeyGuid == id).FirstOrDefault();
//            Anth_Log.TraceEvent("KeyIdentity_ Management", "Information", "tmp", "KeyIdentity_ getbyId");
//            return item;
//        }

//        public KeyIdentity_Model Create(string userGuid) {
//            var model = new KeyIdentity_Model();

//            model.ADt = DateTime.Now.ToString();
//            model.Aned = "n";
//            model.ARelGuid = Guid.NewGuid().ToString().Substring(0, 8);
//            model.IsDeleted = false;
//            model.StorIndexN2 = CoreSecurity.CreateRandomKey();
//            model.StorIndexN1 = CoreSecurity.CreateRandomVector();
//            model.KeyId = Guid.NewGuid().ToString();
//            model.KeyGuid = Guid.NewGuid().ToString();
//            model.UserGuid = userGuid;

//            model.UserKey = AnthillaRsaCore.GenerateKeys();

//            DeNSo.Session.New.Set(model);
//            Anth_Log.TraceEvent("KeyIdentity_ Management", "Information", "tmp", "KeyIdentity_ Created");
//            return model;
//        }

//        public AnthillaPublicKey GetPublicKey(string guid) {
//            var item = DeNSo.Session.New.Get<KeyIdentity_Model>(k => k.UserGuid == guid && k.IsDeleted == false).FirstOrDefault();
//            var key = item.UserKey.Public;
//            return key;
//        }

//        public AnthillaPrivateKey GetPrivateKey(string guid) {
//            var item = DeNSo.Session.New.Get<KeyIdentity_Model>(k => k.UserGuid == guid && k.IsDeleted == false).FirstOrDefault();
//            var key = item.UserKey.Private;
//            return key;
//        }

//        public KeyIdentity_Model Delete(string id) {
//            var deletedItem = DeNSo.Session.New.Get<KeyIdentity_Model>(model => model.KeyGuid == id && model.IsDeleted == false).FirstOrDefault();
//            if (deletedItem != null) {
//                deletedItem.ADt = DateTime.Now.ToString();
//                deletedItem.Aned = "d";
//                deletedItem.IsDeleted = true;
//                DeNSo.Session.New.Set(deletedItem);
//                Anth_Log.TraceEvent("KeyIdentity_ Management", "Information", "tmp", "KeyIdentity_ Deleted");
//            }
//            return deletedItem;
//        }
//    }
//}