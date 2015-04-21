using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UbearCore.Scheduler {
    public class TaskModel {
        [Key]
        public string _Id { get; set; }

        public string Guid { get; set; }

        public string Data0 { get; set; }

        public string Data1 { get; set; }

        public int Interval { get; set; }
    }
}
