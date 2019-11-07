using System;
using System.Threading.Tasks;

namespace Antd.Varlink {
    public class GetInfoResult {
        public string vendor {
            get;
            set;
        }

        public string product {
            get;
            set;
        }

        public string version {
            get;
            set;
        }

        public string url {
            get;
            set;
        }

        public string[] interfaces {
            get;
            set;
        }
    }

    public class ResolveResult {
        public string address {
            get;
            set;
        }
    }

    public class ResolveArgs {
        public string @interface {
            get;
            set;
        }
    }

    public class Resolver : IDisposable {
        public const string DefaultAddress = "unix:/run/org.varlink.resolver";
        public const string InterfaceName = "org.varlink.resolver";
        private readonly IConnection _conn;
        public Resolver(string address = DefaultAddress) {
            _conn = new Connection(address);
        }

        public Task<GetInfoResult> GetInfoAsync() {
            return _conn.CallAsync<GetInfoResult>("org.varlink.resolver.GetInfo", GetErrorParametersType);
        }

        public Task<ResolveResult> ResolveAsync(ResolveArgs args) {
            return _conn.CallAsync<ResolveResult>("org.varlink.resolver.Resolve", GetErrorParametersType, args);
        }

        public static string Resolve(string @interface) {
            using (var resolver = new Resolver()) {
                ResolveResult result =
                    resolver.ResolveAsync(new ResolveArgs { @interface = @interface })
                    .GetAwaiter().GetResult();
                return result.address;
            }
        }

        public static bool TryResolve(string @interface, out string address) {
            try {
                address = Resolve(@interface);
            }
            catch {
                address = null;
            }
            return address != null;
        }

        private static System.Type GetErrorParametersType(string args) {
            return null;
        }

        public void Dispose() {
            _conn.Dispose();
        }
    }
}
