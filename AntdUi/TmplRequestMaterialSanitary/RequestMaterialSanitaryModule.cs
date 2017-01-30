using System;
using System.Collections.Generic;
using System.Linq;
using AntdUi.Models;
using Nancy;
using Newtonsoft.Json;

namespace AntdUi.TmplRequestMaterialSanitary {

    public class RequestMaterialSanitaryModule : NancyModule {

        private readonly RequestMaterialSanitaryRepository _requestMaterialSanitaryRepository = new RequestMaterialSanitaryRepository();
        private readonly SelectizeCacheRepository _cacheRepository = new SelectizeCacheRepository();

        public RequestMaterialSanitaryModule() : base("/repo") {

            Get["/requestmaterialsanitary"] = _ => {
                var list = _requestMaterialSanitaryRepository.Get();
                return JsonConvert.SerializeObject(list);
            };

            Get["/requestmaterialsanitary/{guid}"] = _ => {
                var guid = (Guid)_.guid;
                var list = _requestMaterialSanitaryRepository.Get().FirstOrDefault(x => x.Guid == guid);
                return JsonConvert.SerializeObject(list);
            };

            Post["/requestmaterialsanitary"] = _ => {
                var ward = (string)Request.Form.Ward;
                var nurse = (string)Request.Form.Nurse;
                var model = new RequestMaterialSanitaryModel {
                    Guid = Guid.NewGuid(),
                    Ward = ward.ToUpper(),
                    Nurse = nurse.ToTitleCase(),
                    Date = DateTime.Now,
                    Status = RequestStatus.Empty
                };
                _requestMaterialSanitaryRepository.Save(model);
                _cacheRepository.Save("ward", ward.ToUpper());
                _cacheRepository.Save("nurse", nurse.ToTitleCase());
                return HttpStatusCode.OK;
            };

            Post["/requestmaterialsanitary/edit/status"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                var status = (string)Request.Form.Status;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                var model = _requestMaterialSanitaryRepository.Get(g);
                model.Date = DateTime.Now;
                model.Status = (RequestStatus)Enum.Parse(typeof(RequestStatus), status);
                model.Guid = g;
                _requestMaterialSanitaryRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmaterialsanitary/save"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                var model = _requestMaterialSanitaryRepository.Get(g);
                model.Date = DateTime.Now;
                model.Guid = g;
                var medicines = (string)Request.Form.Materials;
                var meds = medicines.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                var medList = new List<MaterialSanitary>();
                foreach(var med in meds) {
                    var arr = med.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var m = new MaterialSanitary {
                        MaterialName = arr[0],
                        MaterialQty = Convert.ToInt32(arr[1]),
                        MaterialFormat = arr[2],
                        Status = Convert.ToBoolean(arr[3]),
                        Note = arr[4]
                    };
                    medList.Add(m);
                    _cacheRepository.Save("material_sanitary", arr[0]);
                }
                if(model.Status == RequestStatus.OpenNew) {
                    model.Status = RequestStatus.OpenInProgress;
                }
                model.Materials = medList;
                _requestMaterialSanitaryRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmaterialsanitary/submit"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                var model = _requestMaterialSanitaryRepository.Get(g);
                model.Date = DateTime.Now;
                model.Status = RequestStatus.OpenNew;
                model.Guid = g;
                _requestMaterialSanitaryRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmaterialsanitary/archive"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                var model = _requestMaterialSanitaryRepository.Get(g);
                model.Date = DateTime.Now;
                model.Status = RequestStatus.ResolvedSolved;
                model.Guid = g;
                _requestMaterialSanitaryRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmaterialsanitary/delete"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                _requestMaterialSanitaryRepository.Delete(g);
                return HttpStatusCode.OK;
            };
        }
    }
}