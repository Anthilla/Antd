////-------------------------------------------------------------------------------------
////     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
////     All rights reserved.
////
////     Redistribution and use in source and binary forms, with or without
////     modification, are permitted provided that the following conditions are met:
////         * Redistributions of source code must retain the above copyright
////           notice, this list of conditions and the following disclaimer.
////         * Redistributions in binary form must reproduce the above copyright
////           notice, this list of conditions and the following disclaimer in the
////           documentation and/or other materials provided with the distribution.
////         * Neither the name of the Anthilla S.r.l. nor the
////           names of its contributors may be used to endorse or promote products
////           derived from this software without specific prior written permission.
////
////     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
////     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
////     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
////     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
////     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
////     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
////     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
////     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
////     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
////     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
////
////     20141110
////-------------------------------------------------------------------------------------

//using System;
//using System.Collections.Generic;
//using System.Security.Cryptography;
//using System.Text;
//using library.u2f;
//using Nancy;

//namespace Antd.Modules {

//    public class AuthenticationModule : NancyModule {
//        public class TempUser {
//            public string Username { get; set; }
//            public byte[] Password { get; set; }
//            public string TokenId { get; set; }
//        }

//        private readonly IDictionary<string, TempUser> _users = new Dictionary<string, TempUser>();

//        public AuthenticationModule() {

//            Get["/authenticate"] = x => View["login-authentication"];

//            Post["/register"] = x => {
//                var username = (string)Request.Form.Username;
//                var password = (string)Request.Form.Password;
//                var token = (string)Request.Form.Token;
//                try {
//                    var answer = new U2FRequest("25311", "5hQfQbHQGLIauepG9Sa5LQAMGYk=").Validate(token);
//                    if (answer.IsSignatureValid == false && answer.IsValid == false) {
//                        return HttpStatusCode.Forbidden;
//                    }
//                    var user = new TempUser {
//                        Username = username,
//                        Password = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password)),
//                        TokenId = token.Substring(0, 12),
//                    };
//                    _users.Add(user.Username, user);
//                    return HttpStatusCode.OK;
//                }
//                catch (Exception) {
//                    return HttpStatusCode.ImATeapot;
//                }
//            };

//            Post["/authenticate"] = x => {
//                var username = (string)Request.Form.Username;
//                var password = (string)Request.Form.Password;
//                var token = (string)Request.Form.Token;
//                if (_users.Count < 1) {
//                    return "Error: user not valid.";
//                }
//                var user = _users[username];
//                if (user == null) {
//                    return "Error: user not valid.";
//                }
//                var passwordHashing = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(password));
//                if (Encoding.ASCII.GetString(passwordHashing) != Encoding.ASCII.GetString(user.Password)) {
//                    return "Error: password not valid.";
//                }
//                var tokenId = token.Substring(0, 12);
//                if (tokenId != user.TokenId) {
//                    return "Error: token not valid.";
//                }
//                try {
//                    var answer = new U2FRequest("25311", "5hQfQbHQGLIauepG9Sa5LQAMGYk=").Validate(token);
//                    if (answer.IsSignatureValid == false && answer.IsValid == false) {
//                        return HttpStatusCode.Forbidden;
//                    }
//                    return HttpStatusCode.OK;
//                }
//                catch (Exception) {
//                    return HttpStatusCode.ImATeapot;
//                }
//            };
//        }
//    }
//}