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

using System;
using System.Collections.Generic;
using System.Linq;

namespace antdlib.Firewall {
    public class MacAddressModel {
        public string _Id { get; set; }
        public string Guid { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsNew { get; set; }
    }

    public class MacAddressDiscovery {
        public static IEnumerable<MacAddressModel> GetAll() {
            using (var session = DeNSo.Session.New) {
                return session.Get<MacAddressModel>();
            }
        }

        public static IEnumerable<MacAddressModel> GetEnabled() {
            return GetAll().Where(_ => _.IsEnabled && _.IsNew == false);
        }

        public static IEnumerable<MacAddressModel> GetDisabled() {
            return GetAll().Where(_ => _.IsEnabled == false && _.IsNew == false);
        }

        public static IEnumerable<MacAddressModel> GetNew() {
            return GetAll().Where(_ => _.IsEnabled == false && _.IsNew);
        }

        public static MacAddressModel GetByGuid(string guid) {
            return GetAll().FirstOrDefault(_ => _.Guid == guid);
        }

        public static MacAddressModel GetByValue(string value) {
            return GetAll().FirstOrDefault(_ => _.Value == value);
        }

        public static void AddMacAddress(string value, string description) {
            if (GetByValue(value) == null)
                return;
            using (var session = DeNSo.Session.New) {
                session.Set(new MacAddressModel {
                    _Id = Guid.NewGuid().ToString(),
                    Guid = Guid.NewGuid().ToString(),
                    Value = value,
                    Description = description,
                    IsEnabled = false,
                    IsNew = true
                });
            }
        }

        public static void Discover() {
            //todo trova comando
            var lines = Terminal.Terminal.Execute("comando per recuperare dei mac address...").Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines) {
                //todo fai qualcosa
                AddMacAddress("", "");
            }
        }

        public static void Unlock(string guid) {
            var item = GetByGuid(guid);
            if (item == null)
                return;
            using (var session = DeNSo.Session.New) {
                item.IsEnabled = true;
                item.IsNew = false;
                session.Set(item);
            }
            Terminal.Terminal.Execute($"echo {item.Value}");
        }

        public static void Block(string guid) {
            var item = GetByGuid(guid);
            if (item == null)
                return;
            using (var session = DeNSo.Session.New) {
                item.IsEnabled = false;
                item.IsNew = false;
                session.Set(item);
            }
            Terminal.Terminal.Execute($"echo {item.Value}");
        }
    }
}
