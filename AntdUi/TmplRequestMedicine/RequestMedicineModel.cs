using System;
using System.Collections.Generic;

namespace AntdUi.TmplRequestMedicine {
    public class RequestMedicineModel : Database.BaseModel {
        public string FileName { get; set; }
        public string Ward { get; set; }
        public string Nurse { get; set; }
        public DateTime Date { get; set; }
        public RequestStatus Status { get; set; }
        public IEnumerable<MedicineModel> Medicines { get; set; } = new List<MedicineModel>();
    }

    public class MedicineModel {
        public string MedicineName { get; set; }
        public int MedicineQty { get; set; }
        public string MedicineFormat { get; set; }
        public bool Status { get; set; }
        public string Note { get; set; } = "";
    }
}