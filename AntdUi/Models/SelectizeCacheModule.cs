using Nancy;
using Newtonsoft.Json;

namespace AntdUi.Models {

    public class SelectizeCacheModule : NancyModule {

        private readonly SelectizeCacheRepository _cacheRepository = new SelectizeCacheRepository();

        public SelectizeCacheModule() : base("/cache") {

            Get["/{key}"] = _ => {
                var list = _cacheRepository.Get((string)_.key);
                return JsonConvert.SerializeObject(list);
            };
        }
    }
}