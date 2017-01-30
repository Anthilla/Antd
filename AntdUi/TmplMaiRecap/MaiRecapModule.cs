using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using AntdUi.Models;
using Nancy;
using Newtonsoft.Json;

namespace AntdUi.TmplMaiRecap {

    public class MaiRecapModule : NancyModule {

        private readonly MaiRecapRepository _requestMaterialCleaningRepository = new MaiRecapRepository();
        private readonly SelectizeCacheRepository _cacheRepository = new SelectizeCacheRepository();

        public MaiRecapModule() : base("/repo") {

            Get["/mairecap"] = _ => {
                var list = _requestMaterialCleaningRepository.Get();
                return JsonConvert.SerializeObject(list);
            };

            Get["/mairecap/{guid}"] = _ => {
                var guid = (Guid)_.guid;
                var list = _requestMaterialCleaningRepository.Get().FirstOrDefault(x => x.Guid == guid);
                return JsonConvert.SerializeObject(list);
            };

            Post["/mairecap"] = _ => {
                var ward = (string)Request.Form.Ward;
                var nurse = (string)Request.Form.Nurse;
                var month = (string)Request.Form.Month;
                var totalBed = (int)Request.Form.TotalBed;
                var totalAvailableSupport = (int)Request.Form.TotalAvailableSupport;
                var model = new MaiRecapModel {
                    Guid = Guid.NewGuid(),
                    Ward = ward.ToUpper(),
                    Nurse = nurse.ToTitleCase(),
                    Month = month.ToTitleCase(),
                    TotalBed = totalBed,
                    TotalAvailableSupport = totalAvailableSupport,
                    Date = DateTime.Now,
                    Status = RequestStatus.Empty
                };
                _requestMaterialCleaningRepository.Save(model);
                _cacheRepository.Save("ward", ward.ToUpper());
                _cacheRepository.Save("nurse", nurse.ToTitleCase());
                return HttpStatusCode.OK;
            };

            Post["/mairecap/edit/status"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                var status = (string)Request.Form.Status;
                if(Guid.TryParse(guid, out g)) {
                    var model = _requestMaterialCleaningRepository.Get(g);
                    model.Date = DateTime.Now;
                    model.Status = (RequestStatus)Enum.Parse(typeof(RequestStatus), status);
                    model.Guid = g;
                    _requestMaterialCleaningRepository.Edit(model);
                    return HttpStatusCode.OK;
                }
                return HttpStatusCode.InternalServerError;
            };

            Post["/mairecap/save"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                var model = _requestMaterialCleaningRepository.Get(g);
                model.Date = DateTime.Now;
                model.Guid = g;
                var totalKitchenClean = (int)Request.Form.TotalKitchenClean;
                var totalNightDiaperChange = (int)Request.Form.TotalNightDiaperChange;
                model.TotalKitchenClean = totalKitchenClean;
                model.TotalNightDiaperChange = totalNightDiaperChange;
                var guests = (string)Request.Form.Guests;
                var gsts = guests.Split(new[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                var gstList = new List<Guest>();
                foreach(var gst in gsts) {
                    var arr = gst.Split(new[] { "," }, StringSplitOptions.None);
                    var m = new Guest {
                        Gender = Convert.ToChar(arr[0].ToUpper()),
                        BedIndex = Convert.ToInt32(arr[1]),
                        GuestName = arr[2],
                        TotalBath = Convert.ToInt32(arr[3]),
                        TotalNail = Convert.ToInt32(arr[4]),
                        TotalFemaleShave = Convert.ToInt32(arr[5]),
                        TotalMaleShave = Convert.ToInt32(arr[6]),
                        TotalBarber = Convert.ToInt32(arr[7]),
                        TotalSupport = Convert.ToInt32(arr[8]),
                        TotalBedClean = Convert.ToInt32(arr[9]),
                        TotalNotMobilized = Convert.ToInt32(arr[10]),
                        TotalRest = Convert.ToInt32(arr[11]),
                        Note = arr[12],
                        Exit = arr[13]
                    };
                    gstList.Add(m);
                    _cacheRepository.Save("guest_name", arr[2]);
                }
                model.Guests = gstList.OrderBy(x => x.BedIndex).ThenBy(x => x.Exit);
                _requestMaterialCleaningRepository.Edit(model);
                return HttpStatusCode.OK;
            };

            Post["/mairecap/submit"] = _ => {
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

            Post["/mairecap/archive"] = _ => {
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

            Post["/mairecap/delete"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                _requestMaterialCleaningRepository.Delete(g);
                return HttpStatusCode.OK;
            };

            Get["/mairecap/print/{guid}"] = _ => {
                Guid g;
                var guid = (string)_.guid;
                if(!Guid.TryParse(guid, out g))
                    return HttpStatusCode.InternalServerError;
                dynamic model = new ExpandoObject();
                var file = _requestMaterialCleaningRepository.Get(g);
                model.File = file;
                model.Guests = file.Guests;
                return View["pages/mai_recap/print", model];
            };
        }
    }
}