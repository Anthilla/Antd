using System;
using System.Linq;
using Nancy;
using Newtonsoft.Json;

namespace AntdUi.TmplTextFile {

    public class TextFileModule : NancyModule {

        private readonly TextFileRepository _requestWarehouseRepository = new TextFileRepository();

        public TextFileModule() : base("/repo") {

            Get["/textfile"] = _ => {
                var list = _requestWarehouseRepository.Get();
                return JsonConvert.SerializeObject(list);
            };

            Get["/textfile/{guid}"] = _ => {
                var guid = (Guid)_.guid;
                var list = _requestWarehouseRepository.Get().FirstOrDefault(x => x.Guid == guid);
                return JsonConvert.SerializeObject(list);
            };

            Post["/textfile"] = _ => {
                var name = Request.Form.Name;
                var content = Request.Form.Content;
                var model = new TextFileModel {
                    Guid = Guid.NewGuid(),
                    Date = DateTime.Now,
                    Name = name,
                    Content = content
                };
                _requestWarehouseRepository.Save(model);
                return HttpStatusCode.OK;
            };

            Post["/textfile/edit"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                var name = Request.Form.Name;
                var content = Request.Form.Content;
                var model = new TextFileModel {
                    Date = DateTime.Now,
                    Name = name,
                    Content = content
                };
                if(Guid.TryParse(guid, out g)) {
                    model.Guid = g;
                    _requestWarehouseRepository.Edit(model);
                    return HttpStatusCode.OK;
                }
                return HttpStatusCode.InternalServerError;
            };

            Post["/textfile/delete"] = _ => {
                Guid g;
                var guid = (string)Request.Form.Guid;
                if(Guid.TryParse(guid, out g)) {
                    _requestWarehouseRepository.Delete(g);
                    return HttpStatusCode.OK;
                }
                return HttpStatusCode.InternalServerError;
            };

            Get["/textfile/print"] = _ => {
                Guid g;
                var guid = (string)Request.Query.id;
                var model = new TextFileModel { Content = "Error 404" };
                if(Guid.TryParse(guid, out g)) {
                    model = _requestWarehouseRepository.Get(g);
                }
                return View["pages/request_warehouse/print.html", model];
            };
        }
    }
}