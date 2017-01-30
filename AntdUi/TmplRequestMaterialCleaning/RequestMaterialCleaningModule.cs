using System;
using System.Collections.Generic;
using System.Linq;
using AntdUi.Models;
using Nancy;
using Newtonsoft.Json;

namespace AntdUi.TmplRequestMaterialCleaning {

    public class RequestMaterialCleaningModule : NancyModule {

        private readonly RequestMaterialCleaningRepository _requestMaterialCleaningRepository = new RequestMaterialCleaningRepository();
        private readonly SelectizeCacheRepository _cacheRepository = new SelectizeCacheRepository();

        public RequestMaterialCleaningModule() : base("/repo") {

            Get["/requestmaterialcleaning"] = _ => {
                var list = _requestMaterialCleaningRepository.Get();
                return JsonConvert.SerializeObject(list);
            };

            Get["/requestmaterialcleaning/{guid}"] = _ => {
                var guid = (Guid)_.guid;
                var list = _requestMaterialCleaningRepository.Get().FirstOrDefault(x => x.Guid == guid);
                return JsonConvert.SerializeObject(list);
            };

            Post["/requestmaterialcleaning"] = _ => {
                var ward = (string)Request.Form.Ward;
                var nurse = (string)Request.Form.Nurse;
                var model = new RequestMaterialCleaningModel {
                    Guid = Guid.NewGuid(),
                    Ward = ward.ToUpper(),
                    Nurse = nurse.ToTitleCase(),
                    Date = DateTime.Now,
                    Status = RequestStatus.Empty
                };
                _requestMaterialCleaningRepository.Save(model);
                _cacheRepository.Save("ward", ward.ToUpper());
                _cacheRepository.Save("nurse", nurse.ToTitleCase());
                return HttpStatusCode.OK;
            };

            Post["/requestmaterialcleaning/edit/status"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                var status = (string)Request.Form.Status;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                var model = _requestMaterialCleaningRepository.Get(g);
                model.Date = DateTime.Now;
                model.Status = (RequestStatus)Enum.Parse(typeof(RequestStatus), status);
                model.Guid = g;
                _requestMaterialCleaningRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmaterialcleaning/save"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                var model = _requestMaterialCleaningRepository.Get(g);
                model.Date = DateTime.Now;
                model.Guid = g;
                var medicines = (string)Request.Form.Materials;
                var meds = medicines.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                var medList = new List<MaterialCleaning>();
                foreach(var med in meds) {
                    var arr = med.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var m = new MaterialCleaning {
                        MaterialName = arr[0],
                        MaterialQty = Convert.ToInt32(arr[1]),
                        MaterialFormat = arr[2],
                        Status = Convert.ToBoolean(arr[3]),
                        Note = arr[4]
                    };
                    medList.Add(m);
                    _cacheRepository.Save("material_cleaning", arr[0]);
                }
                if(model.Status == RequestStatus.OpenNew) {
                    model.Status = RequestStatus.OpenInProgress;
                }
                model.Materials = medList;
                _requestMaterialCleaningRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmaterialcleaning/submit"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                var model = _requestMaterialCleaningRepository.Get(g);
                model.Date = DateTime.Now;
                model.Status = RequestStatus.OpenNew;
                model.Guid = g;
                _requestMaterialCleaningRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmaterialcleaning/archive"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                var model = _requestMaterialCleaningRepository.Get(g);
                model.Date = DateTime.Now;
                model.Status = RequestStatus.ResolvedSolved;
                model.Guid = g;
                _requestMaterialCleaningRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmaterialcleaning/delete"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                _requestMaterialCleaningRepository.Delete(g);
                return HttpStatusCode.OK;
            };
        }
    }
}