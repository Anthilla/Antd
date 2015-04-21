using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbearCore.Scheduler {
    public class TaskRepository {
        public static List<TaskModel> GetAll() {
            List<TaskModel> list = DeNSo.Session.New.Get<TaskModel>().ToList();
            return list;
        }

        public static void Create(string guid, string data0, string data1, int interval) {
            TaskModel task = new TaskModel();
            task._Id = Guid.NewGuid().ToString();
            task.Guid = guid;
            task.Data0 = data0;
            task.Data1 = data1;
            task.Interval = interval;
            DeNSo.Session.New.Set(task);
        }
    }
}
