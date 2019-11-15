using NWebDav.Server.Http;
using System.Security.Principal;

namespace NWebDav.Server.HttpListener {
    public class HttpSession : IHttpSession {
        internal HttpSession(IPrincipal principal) {
            Principal = principal;
        }

        public IPrincipal Principal { get; }
    }
}
