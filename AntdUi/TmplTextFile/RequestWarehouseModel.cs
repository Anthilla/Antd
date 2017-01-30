using System;

namespace AntdUi.TmplTextFile {
    public class TextFileModel : Database.BaseModel {
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }
}