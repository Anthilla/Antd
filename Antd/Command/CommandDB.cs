using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antd.CommandManagement {
    public class CommandDB {
        public class CommandInputModel {
            public string _Id { get; set; }

            public string Guid { get; set; }

            public DateTime Date { get; set; }

            public string File { get; set; }

            public string Arguments { get; set; }

            public string Notes {get; set;}
        }

        public List<CommandInputModel> GetAll() {
            return DeNSo.Session.New.Get<CommandInputModel>(m => m != null).ToList();
        }

        public List<CommandInputModel> GetByString(string q) {
            return DeNSo.Session.New.Get<CommandInputModel>(m => m.File.Contains(q) || m.Arguments.Contains(q)).ToList();
        }

        public CommandInputModel GetByGuid(string g) { 
            return DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == g).FirstOrDefault();
        }

        public void Create(string command, string notes) {
            var model = new CommandInputModel();
            model._Id = Guid.NewGuid().ToString();
            model.Guid = model._Id;
            model.Date = DateTime.Now;
            model.File = "";
            model.Arguments = "";
            model.Notes = notes;
            DeNSo.Session.New.Set(model);
        }

        public void Delete(string g) {
            var model = DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == g).FirstOrDefault();
            DeNSo.Session.New.Delete(model);
        }

        public void Launch(string guid) {
            var command = DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == guid).FirstOrDefault();
        }
    }
}
