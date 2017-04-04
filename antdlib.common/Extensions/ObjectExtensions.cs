using Newtonsoft.Json;
using System.Collections.Generic;
using System.Reflection;

namespace antdlib.common {
    public static class ObjectExtensions {
        public static IDictionary<string, string> ToDictionary<T>(this T item) where T : class, new() {
            var result = new Dictionary<string, string>();
            if(item == null)
                return result;
            var type = item.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var pi in properties) {
                var selfValue = type.GetProperty(pi.Name).GetValue(item, null);
                if(selfValue != null) {
                    result.Add(pi.Name, selfValue.ToString());
                }
            }
            return result;
        }

        public static string ToJson<T>(this T obj) {
            return obj == null ? string.Empty : JsonConvert.SerializeObject(obj);
        }
    }
}
