using Antd.Common;
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

            public string Layout { get; set; }

            public string Notes { get; set; }
        }

        public static List<CommandInputModel> GetAll() {
            var list = DeNSo.Session.New.Get<CommandInputModel>(m => m != null).ToList();
            if (list == null) {
                return new List<CommandInputModel>() { };
            }
            else {
                return list;
            }
        }

        public static List<CommandInputModel> GetByString(string q) {
            return DeNSo.Session.New.Get<CommandInputModel>(m => m.File.Contains(q) || m.Arguments.Contains(q)).ToList();
        }

        public static CommandInputModel GetByGuid(string g) {
            return DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == g).FirstOrDefault();
        }

        public static void Create(string command, string layout, string notes) {
            var model = new CommandInputModel();
            model._Id = Guid.NewGuid().ToString();
            model.Guid = model._Id;
            model.Date = DateTime.Now;
            model.File = command.GetFirstString();
            model.Arguments = command.GetAllStringsButFirst();
            model.Layout = layout;
            model.Notes = notes;
            DeNSo.Session.New.Set(model);
        }

        public static void Delete(string g) {
            var model = DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == g).FirstOrDefault();
            DeNSo.Session.New.Delete(model);
        }

        public static void Launch(string guid) {
            var command = DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == guid).FirstOrDefault();
            Command.Launch(command.File, command.Arguments);
        }

        public static string LaunchAndGetOutput(string guid) {
            var command = DeNSo.Session.New.Get<CommandInputModel>(m => m.Guid == guid).FirstOrDefault();
            return Command.Launch(command.File, command.Arguments).output;
        }
    }
}
