using System;
using System.Collections.Generic;
using System.Linq;
using AntdUi.Models;
using Nancy;
using Newtonsoft.Json;

namespace AntdUi.TmplRequestMedicine {

    public class RequestMedicineModule : NancyModule {

        private readonly RequestMedicineRepository _requestMedicineRepository = new RequestMedicineRepository();
        private readonly SelectizeCacheRepository _cacheRepository = new SelectizeCacheRepository();

        public RequestMedicineModule() : base("/repo") {

            Get["/requestmedicine"] = _ => {
                var list = _requestMedicineRepository.Get();
                return JsonConvert.SerializeObject(list);
            };

            Get["/requestmedicine/{guid}"] = _ => {
                var guid = (Guid)_.guid;
                var list = _requestMedicineRepository.Get().FirstOrDefault(x => x.Guid == guid);
                return JsonConvert.SerializeObject(list);
            };

            Post["/requestmedicine"] = _ => {
                var ward = (string)Request.Form.Ward;
                var nurse = (string)Request.Form.Nurse;
                var model = new RequestMedicineModel {
                    Guid = Guid.NewGuid(),
                    Ward = ward.ToUpper(),
                    Nurse = nurse.ToTitleCase(),
                    Date = DateTime.Now,
                    Status = RequestStatus.Empty
                };
                _requestMedicineRepository.Save(model);
                _cacheRepository.Save("ward", ward.ToUpper());
                _cacheRepository.Save("nurse", nurse.ToTitleCase());
                return HttpStatusCode.OK;
            };

            Post["/requestmedicine/edit/status"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                var status = (string)Request.Form.Status;
                if (!Guid.TryParse(guid, out g)) return HttpStatusCode.InternalServerError;
                var model = _requestMedicineRepository.Get(g);
                model.Date = DateTime.Now;
                model.Status = (RequestStatus)Enum.Parse(typeof(RequestStatus), status);
                model.Guid = g;
                _requestMedicineRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmedicine/save"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if (!Guid.TryParse(guid, out g)) return HttpStatusCode.InternalServerError;
                var model = _requestMedicineRepository.Get(g);
                model.Date = DateTime.Now;
                model.Guid = g;
                var medicines = (string)Request.Form.Medicines;
                var meds = medicines.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                var medList = new List<MedicineModel>();
                foreach(var med in meds) {
                    var arr = med.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var m = new MedicineModel {
                        MedicineName = arr[0],
                        MedicineQty = Convert.ToInt32(arr[1]),
                        MedicineFormat = arr[2],
                        Status = Convert.ToBoolean(arr[3]),
                        Note = arr[4]
                    };
                    medList.Add(m);
                }
                if(model.Status == RequestStatus.OpenNew) {
                    model.Status = RequestStatus.OpenInProgress;
                }
                model.Medicines = medList;
                _requestMedicineRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmedicine/submit"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if (!Guid.TryParse(guid, out g)) return HttpStatusCode.InternalServerError;
                var model = _requestMedicineRepository.Get(g);
                model.Date = DateTime.Now;
                model.Status = RequestStatus.OpenNew;
                model.Guid = g;
                _requestMedicineRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmedicine/archive"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if (!Guid.TryParse(guid, out g)) return HttpStatusCode.InternalServerError;
                var model = _requestMedicineRepository.Get(g);
                model.Date = DateTime.Now;
                model.Status = RequestStatus.ResolvedSolved;
                model.Guid = g;
                _requestMedicineRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmedicine/delete"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if (!Guid.TryParse(guid, out g)) return HttpStatusCode.InternalServerError;
                _requestMedicineRepository.Delete(g);
                return HttpStatusCode.OK;
            };

            Get["/requestmedicine/print"] = _ => {
                Guid g;
                var guid = (string)Request.Query.id;
                var model = new RequestMedicineModel { FileName = "Error 404" };
                if(Guid.TryParse(guid, out g)) {
                    model = _requestMedicineRepository.Get(g);
                }
                return View["pages/request_medicine/print.html", model];
            };
        }
    }
}