using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.common;
using AntdUi.Models;
using Nancy;
using Newtonsoft.Json;

namespace AntdUi.TmplRequestMaterialGeneric {

    public class RequestMaterialGenericModule : NancyModule {

        private readonly RequestMaterialGenericRepository _requestMaterialGenericRepository = new RequestMaterialGenericRepository();
        private readonly SelectizeCacheRepository _cacheRepository = new SelectizeCacheRepository();

        public RequestMaterialGenericModule() : base("/repo") {

            Get["/requestmaterialgeneric"] = _ => {
                var list = _requestMaterialGenericRepository.Get();
                return JsonConvert.SerializeObject(list);
            };

            Get["/requestmaterialgeneric/{guid}"] = _ => {
                var guid = (Guid)_.guid;
                var list = _requestMaterialGenericRepository.Get().FirstOrDefault(x => x.Guid == guid);
                return JsonConvert.SerializeObject(list);
            };

            Post["/requestmaterialgeneric"] = _ => {
                var ward = (string)Request.Form.Ward;
                var nurse = (string)Request.Form.Nurse;
                var model = new RequestMaterialGenericModel {
                    Guid = Guid.NewGuid(),
                    Ward = ward.ToUpper(),
                    Nurse = nurse.ToTitleCase(),
                    Date = DateTime.Now,
                    Status = RequestStatus.Empty
                };
                _requestMaterialGenericRepository.Save(model);
                _cacheRepository.Save("ward", ward.ToUpper());
                _cacheRepository.Save("nurse", nurse.ToTitleCase());
                return HttpStatusCode.OK;
            };

            Post["/requestmaterialgeneric/edit/status"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                var status = (string)Request.Form.Status;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                var model = _requestMaterialGenericRepository.Get(g);
                model.Date = DateTime.Now;
                model.Status = (RequestStatus)Enum.Parse(typeof(RequestStatus), status);
                model.Guid = g;
                _requestMaterialGenericRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmaterialgeneric/save"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                var model = _requestMaterialGenericRepository.Get(g);
                model.Date = DateTime.Now;
                model.Guid = g;
                var medicines = (string)Request.Form.Materials;
                var meds = medicines.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                var medList = new List<MaterialGeneric>();
                foreach(var med in meds) {
                    var arr = med.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    var m = new MaterialGeneric {
                        MaterialName = arr[0],
                        MaterialQty = Convert.ToInt32(arr[1]),
                        MaterialFormat = arr[2],
                        Status = Convert.ToBoolean(arr[3]),
                        Note = arr[4]
                    };
                    medList.Add(m);
                    _cacheRepository.Save("material_generic", arr[0]);
                }
                if(model.Status == RequestStatus.OpenNew) {
                    model.Status = RequestStatus.OpenInProgress;
                }
                model.Materials = medList;
                _requestMaterialGenericRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmaterialgeneric/submit"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                var model = _requestMaterialGenericRepository.Get(g);
                model.Date = DateTime.Now;
                model.Status = RequestStatus.OpenNew;
                model.Guid = g;
                _requestMaterialGenericRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmaterialgeneric/archive"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                var model = _requestMaterialGenericRepository.Get(g);
                model.Date = DateTime.Now;
                model.Status = RequestStatus.ResolvedSolved;
                model.Guid = g;
                _requestMaterialGenericRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/requestmaterialgeneric/delete"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                _requestMaterialGenericRepository.Delete(g);
                return HttpStatusCode.OK;
            };
        }
    }
}