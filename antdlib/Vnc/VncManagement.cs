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

using antdlib.Users;
using System;
using System.Linq;

namespace antdlib.Vnc {
    public class VncManagement {
        public static string Get(string userGuid) {
            var q = $"{userGuid.Substring(0, 8)}-vnc";
            var user = UserEntity.Repository.GetByUserIdentity(userGuid);
            try {
                if (user == null) {
                    throw new ArgumentNullException(nameof(user));
                }
                var vncClaim = user.Claims.FirstOrDefault(_ => _.Type == UserEntity.ClaimType.Vnc && _.Key == q);
                return vncClaim?.Value;
            }
            catch (Exception ex) {
                throw new NotImplementedException();
            }
        }

        public static void Set(string userGuid, string vncAddress) {
            UserEntity.Repository.AddClaim(userGuid, UserEntity.ClaimType.Vnc, UserEntity.ClaimMode.Null, $"{userGuid.Substring(0, 8)}-vnc", vncAddress);
        }

        public static void Edit(string userGuid, string oldValue, string newValue) {
            var user = UserEntity.Repository.GetByUserIdentity(userGuid);
            try {
                if (user == null) {
                    throw new ArgumentNullException(nameof(user));
                }
                UserEntity.Repository.AddClaim(userGuid, UserEntity.ClaimType.Vnc, UserEntity.ClaimMode.Null, $"{userGuid.Substring(0, 8)}-vnc", newValue);
                var vncClaim = user.Claims.FirstOrDefault(_ => _.Type == UserEntity.ClaimType.Vnc && _.Value == oldValue);
                UserEntity.Repository.RemoveClaim(userGuid, vncClaim.ClaimGuid);
            }
            catch (Exception ex) {
                throw new NotImplementedException();
            }
        }

        public static void Remove(string userGuid, string value) {
            var user = UserEntity.Repository.GetByUserIdentity(userGuid);
            try {
                if (user == null) {
                    throw new ArgumentNullException(nameof(user));
                }
                var vncClaim = user.Claims.FirstOrDefault(_ => _.Type == UserEntity.ClaimType.Vnc && _.Value == value);
                UserEntity.Repository.RemoveClaim(userGuid, vncClaim.ClaimGuid);
            }
            catch (Exception ex) {
                throw new NotImplementedException();
            }
        }
    }
}
