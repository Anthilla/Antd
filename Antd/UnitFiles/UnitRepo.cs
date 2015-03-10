using System.Linq;

namespace Antd.UnitFiles {
    public class UnitModel {
        public string _Id { get; set; }
        public string description { get; set; }
        public string timeOutStartSec { get; set; }
        public string execStart { get; set; }
        public string execStop { get; set; }
        public string wantedBy { get; set; }
        public string alias { get; set; }
    }

    public class UnitRepo { 
        public static UnitModel GetInfo(string name){
            return DeNSo.Session.New.Get<UnitModel>(u => u.description == name).FirstOrDefault();
        }

        public static void SetInfo(string guid, string[] args) {
            UnitModel unit = new UnitModel() {
                _Id = guid,
                description = args[0],
                timeOutStartSec = "0",
                execStart = args[1],
                alias = args[0],
            };
            DeNSo.Session.New.Set(unit);
        }
    }
}
