using System;
using System.Collections.Generic;

namespace AntdUi.TmplRequestMaterialCleaning {
    public class RequestMaterialCleaningModel : Database.BaseModel {
        public string FileName { get; set; }
        public string Ward { get; set; }
        public string Nurse { get; set; }
        public DateTime Date { get; set; }
        public RequestStatus Status { get; set; }
        public IEnumerable<MaterialCleaning> Materials { get; set; } = new List<MaterialCleaning>();
    }

    public class MaterialCleaning {
        public string MaterialName { get; set; }
        public int MaterialQty { get; set; }
        public string MaterialFormat { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; } = "";
    }
}