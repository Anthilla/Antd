using System.ComponentModel.DataAnnotations;
namespace Antd.Log {
    public class LogModel {
        [Key]
        public string _Id { get; set; }
        public string time { get; set; }
        public string mode { get; set; }
        public string file { get; set; }
        public string oldfile { get; set; }
    }
}
