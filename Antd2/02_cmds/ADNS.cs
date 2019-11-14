using ARSoft.Tools.Net.Dns;
using System;
using System.Threading.Tasks;

namespace Antd2.cmds {
    public class ADNS {

        public static void Start() {
            using (DnsServer server = new DnsServer(10, 10)) {
                server.QueryReceived += OnQueryReceived;
                server.Start();
                System.Console.ReadLine();
            }
            //using DnsServer server = new DnsServer(10, 10);
        }

        static async Task OnQueryReceived(object sender, QueryReceivedEventArgs e) {
            DnsMessage query = e.Query as DnsMessage;
            if (query == null)
                return;

            DnsMessage response = query.CreateResponseInstance();

            if ((query.Questions.Count == 1)) {
                // send query to upstream server
                DnsQuestion question = query.Questions[0];
                Console.WriteLine($"[adns] {question.Name} {question.RecordType} {question.RecordClass}");
                DnsMessage upstreamResponse = await DnsClient.Default.ResolveAsync(question.Name, question.RecordType, question.RecordClass);

                // if got an answer, copy it to the message sent to the client
                if (response != null) {
                    foreach (DnsRecordBase record in (upstreamResponse.AnswerRecords)) {
                        response.AnswerRecords.Add(record);
                    }
                    foreach (DnsRecordBase record in (upstreamResponse.AdditionalRecords)) {
                        response.AdditionalRecords.Add(record);
                    }

                    if (response.AnswerRecords.Count == 0) {
                        response.ReturnCode = ReturnCode.NxDomain;
                        Console.WriteLine("risolvi internament");
                        //risolvi internamente
                    }
                    else {
                        response.ReturnCode = ReturnCode.NoError;
                    }

                    // set the response
                    e.Response = response;
                }
                else {
                    e.Response.ReturnCode = ReturnCode.ServerFailure;
                }
            }

            //if (response.Questions.Any()) {
            //    DnsQuestion question = response.Questions[0];
            //    DnsMessage upstreamResponse = await DnsClient.Default.ResolveAsync(question.Name, question.RecordType, question.RecordClass);

            //    response.AdditionalRecords.AddRange(upstreamResponse.AdditionalRecords);
            //    response.ReturnCode = ReturnCode.NoError;

            //    if (!question.Name.ToString().Contains("pippo.com")) {
            //        response.AnswerRecords.AddRange(upstreamResponse.AnswerRecords);
            //    }
            //    else {
            //        response.AnswerRecords.AddRange(
            //            upstreamResponse.AnswerRecords
            //                .Where(w => !(w is ARecord))
            //                .Concat(
            //                    upstreamResponse.AnswerRecords
            //                        .OfType<ARecord>()
            //                        .Select(a => new ARecord(a.Name, a.TimeToLive, IPAddress.Parse("192.168.111.201"))) // some local ip address
            //                )
            //        );
            //    }

            //    e.Response = response;
            //}

            //// check for valid query
            //if ((query.Questions.Count == 1)
            //    && (query.Questions[0].RecordType == RecordType.Txt)
            //    && (query.Questions[0].Name.Equals(DomainName.Parse("example.com")))) {
            //    response.ReturnCode = ReturnCode.NoError;
            //    response.AnswerRecords.Add(new TxtRecord(DomainName.Parse("example.com"), 3600, "Hello world"));
            //}
            //else {
            //    response.ReturnCode = ReturnCode.ServerFailure;
            //}

            //// set the response
            //e.Response = response;
        }
    }
}
