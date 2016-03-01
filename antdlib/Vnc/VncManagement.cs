//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using antdlib.Log;
using antdlib.Users;
using System;
using System.Collections.Generic;
using System.Linq;

namespace antdlib.Vnc {
    public class VncManagement {
        public static IEnumerable<string> Get(string userGuid) {
            var q = $"{userGuid.Substring(0, 8)}-vnc";
            var user = UserEntity.Repository.GetByUserIdentity(userGuid);
            try {
                if (user == null) {
                    throw new ArgumentNullException(nameof(user));
                }
                var vncClaim = user.Claims.Where(_ => _.Type == UserEntity.ClaimType.Vnc && _.Key == q);
                return vncClaim.Select(_ => _.Value);
            }
            catch (Exception ex) {
                ConsoleLogger.Warn(ex.Message);
                return new List<string>();
            }
        }

        public static IDictionary<string, string> GetQueryStrings(string userGuid) {
            var user = UserEntity.Repository.GetByUserGuid(userGuid);
            var dict = new Dictionary<string, string>();
            try {
                if (user == null) {
                    throw new ArgumentNullException(nameof(user));
                }
                var vncClaim = user.Claims.Where(_ => _.Type == UserEntity.ClaimType.Vnc).ToList();
                foreach (var claim in vncClaim) {
                    dict.Add(claim.Key, ConvertToQueryString(claim.Value));
                }
                return dict;
            }
            catch (Exception ex) {
                ConsoleLogger.Warn(ex.Message);
                return dict;
            }
        }

        private static string ConvertToQueryString(string address) {
            var addressInfo = address.Split(':');
            if (address.Length > 1) {
                return $"?host={addressInfo[0]}&port={addressInfo[1]}";
            }
            return string.Empty;
        }

        public static void Set(string userGuid, string vncAddress) {
            UserEntity.Repository.AddClaim(userGuid, UserEntity.ClaimType.Vnc, UserEntity.ClaimMode.Null, $"{userGuid.Substring(0, 8)}-vnc", vncAddress);
        }
    }
}
