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
using antdlib.Common;

namespace antdlib {

    public class ApplicationSetting {
        private const string CoreFileName = "antdConfig";

        private static readonly string[] Files = {
                CoreFileName + "Current",
                CoreFileName + "001",
                CoreFileName + "002"
            };

        public static readonly XmlWriter Writer = new XmlWriter(Files);

        #region core CRUD
        public static void Set(string key, string value) {
            Writer.Write(key, value);
        }

        public static string Get(string key) {
            return Writer.ReadValue(key);
        }

        public static void Delete(string key) {
            throw new NotImplementedException();
        }
        #endregion core CRUD

        public static void WriteDefaults() {
            if (Writer.CheckValue("AntdHttpPort") == false) {
                Writer.Write("AntdHttpPort", "8084");
            }
            if (Writer.CheckValue("AntdHttpsPort") == false) {
                Writer.Write("AntdHttpsPort", "443");
            }
            if (Writer.CheckValue(Parameter.LabelAntdDatabase) == false) {
                Writer.Write(Parameter.LabelAntdDatabase, Parameter.AntdCfgDatabase);
            }
            if (Writer.CheckValue("ssl") == false) {
                Writer.Write("ssl", "yes");
            }
            if (Writer.CheckValue("certificate") == false) {
                Writer.Write("certificate", $"{Parameter.AntdCfg}/certificate.pfx");
            }
            if (Writer.CheckValue("ca") == false) {
                Writer.Write("ca", "no");
            }
            if (Writer.CheckValue("ca_path") == false) {
                Writer.Write("ca", Parameter.CertificateAuthority);
            }
            if (Writer.CheckValue("x509") == false) {
                Writer.Write("x509", "");
            }
            if (Writer.CheckValue(Parameter.LabelAuthIsEnabled) == false) {
                Writer.Write(Parameter.LabelAuthIsEnabled, false.ToString());
            }
            if (Writer.CheckValue("protocol") == false) {
                Writer.Write("protocol", "https");
            }
        }

        public static string X509() {
            return Get("x509") ?? Parameter.CertificateAuthorityX509;
        }

        public static void SetX509(string val) {
            Writer.Write("x509", val);
        }

        public static string CaPath() {
            return Get("ca_path") ?? Parameter.CertificateAuthority;
        }

        public static void SetCaPath(string path) {
            Writer.Write("ca_path", path);
        }

        public static string HttpsPort() {
            return Get("AntdHttpsPort") ?? "443";
        }

        public static void SetHttpsPort(string port) {
            Writer.Write("AntdHttpsPort", port);
        }

        public static string HttpPort() {
            return Get("AntdHttpPort") ?? "7777";
        }

        public static void SetHttpPort(string port) {
            Writer.Write("AntdHttpPort", port);
        }

        public static string HttpProtocol() {
            return Get("protocol") ?? "https";
        }

        public static void SwitchToHttps() {
            Writer.Write("protocol", "https");
        }

        public static void SwitchToHttp() {
            Writer.Write("protocol", "http");
        }

        public static bool TwoFactorAuth() {
            var value = Get(Parameter.LabelAuthIsEnabled) ?? "false";
            return Convert.ToBoolean(value);
        }

        public static void EnableTwoFactorAuth() {
            Writer.Write(Parameter.LabelAuthIsEnabled, "true");
        }

        public static void DisableTwoFactorAuth() {
            Writer.Write(Parameter.LabelAuthIsEnabled, "false");
        }

        public static string CertificatePath() {
            return Get("certificate") ?? $"{Parameter.AntdCfg}/certificate.pfx";
        }

        public static void SetCertificatePath(string newCert) {
            Writer.Write("certificate", $"{Parameter.AntdCfg}/certificate.pfx");
        }

        public static string Ssl() {
            return Get("ssl") ?? "yes";
        }

        public static void EnableSsl() {
            Writer.Write("ssl", "yes");
        }

        public static void DisableSsl() {
            Writer.Write("ssl", "no");
        }

        public static string CertificateAuthority() {
            return Get("ca") ?? "no";
        }

        public static void EnableCertificateAuthority() {
            Writer.Write("ca", "yes");
        }

        public static void DisableCertificateAuthority() {
            Writer.Write("ca", "no");
        }

        public static string DatabasePath() {
            return Get(Parameter.LabelAntdDatabase) ?? Parameter.AntdCfgDatabase;
        }

        public static string HostUri() {
            return Get(Parameter.LabelAntdPort) == null ? "http://+:7777/" : $"http://+:{Writer.ReadValue(Parameter.LabelAntdPort)}/";
        }

        public static string WebsocketPort() {
            return Get("AntdWebsocketPort") ?? "12345";
        }

        public static void SetWebsocketPort(string port) {
            Writer.Write("AntdWebsocketPort", port);
        }
    }
}
