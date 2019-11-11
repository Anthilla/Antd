//using CoAP;
//using System;

//namespace hoplite.Connection {
//    /// <summary>
//    /// Metodi per gestire le chiamate REST COAP
//    /// </summary>
//    public class CoapClient {

//        private static int CoapPort = App.OPTS.BackendCoapPort01;

//        /// <summary>
//        /// Effettua una chiamata rest di tipo GET al server di riferimento
//        /// </summary>
//        /// <param name="context">Contesto coap</param>
//        /// <returns></returns>
//        public static Response Get(string context) {
//            // new a GET request
//            Request request = new Request(Method.GET) {
//                URI = new Uri($"coap://localhost:{CoapPort}/{context}")
//            };
//            request.Send();

//            // wait for one response
//            Response response = request.WaitForResponse();
//            return response;
//        }

//        /// <summary>
//        /// Effettua una chiamata rest di tipo POST al server di riferimento
//        /// </summary>
//        /// <param name="context">Contesto coap</param>
//        /// <param name="payload">Dati inviati al server</param>
//        /// <returns></returns>
//        public static Response Post(string context, string payload) {
//            // new a POST request
//            Request request = new Request(Method.POST) {
//                URI = new Uri($"coap://localhost:{CoapPort}/{context}"),
//                PayloadString = payload
//            };
//            request.Send();

//            // wait for one response
//            Response response = request.WaitForResponse();
//            return response;
//        }
//    }
//}
