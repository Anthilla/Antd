using System.Net.Http;
using System.Threading.Tasks;

namespace Antd.cmds {
    public class PublicIp {

        public static string Get() {
            return GetAsync().GetAwaiter().GetResult();
        }

        public static async Task<string> GetAsync() {
            return await new HttpClient().GetStringAsync("http://api.ipify.org");
        }
    }
}
