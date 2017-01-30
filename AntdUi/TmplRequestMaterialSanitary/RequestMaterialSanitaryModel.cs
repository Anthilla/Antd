using System;
using System.Collections.Generic;

namespace AntdUi.TmplRequestMaterialSanitary {
    public class RequestMaterialSanitaryModel : Database.BaseModel {
        public string FileName { get; set; }
        public string Ward { get; set; }
        public string Nurse { get; set; }
        public DateTime Date { get; set; }
        public RequestStatus Status { get; set; }
        public IEnumerable<MaterialSanitary> Materials { get; set; } = new List<MaterialSanitary>();
    }

    public class MaterialSanitary {
        public string MaterialName { get; set; }
        public int MaterialQty { get; set; }
        public string MaterialFormat { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; } = "";
    }
}