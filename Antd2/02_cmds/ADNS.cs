using ARSoft.Tools.Net;
using ARSoft.Tools.Net.Dns;
using System.Net;
using System.Threading.Tasks;

namespace Antd2.cmds {
    public class ADNS {

        public static void Start() {
            using DnsServer server = new DnsServer(IPAddress.Any, 10, 10);
            //server.ClientConnected += OnClientConnected;
            server.QueryReceived += OnQueryReceived;

            server.Start();
        }

        //static async Task OnClientConnected(object sender, ClientConnectedEventArgs e) {
        //    await Task.Delay(1).ConfigureAwait(false);
        //    if (!IPAddress.IsLoopback(e.RemoteEndpoint.Address))
        //        e.RefuseConnect = true;
        //}

        static async Task OnQueryReceived(object sender, QueryReceivedEventArgs e) {
            await Task.Delay(1).ConfigureAwait(false);
            DnsMessage query = e.Query as DnsMessage;

            if (query == null)
                return;

            DnsMessage response = query.CreateResponseInstance();

            // check for valid query
            if ((query.Questions.Count == 1)
                && (query.Questions[0].RecordType == RecordType.Txt)
                && (query.Questions[0].Name.Equals(DomainName.Parse("example.com")))) {
                response.ReturnCode = ReturnCode.NoError;
                response.AnswerRecords.Add(new TxtRecord(DomainName.Parse("example.com"), 3600, "Hello world"));
            }
            else {
                response.ReturnCode = ReturnCode.ServerFailure;
            }

            // set the response
            e.Response = response;
        }
    }
}
