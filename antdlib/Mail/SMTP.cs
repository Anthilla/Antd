///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using antdlib.Boot;
using antdlib.Security;
using MailKit.Net.Smtp;
using MimeKit;

namespace antdlib.Mail {
    public class SMTP {

        public class Settings {
            private static byte[] key = Cryptography.CoreKey();
            private static byte[] vector = Cryptography.CoreVector();

            private static string coreFileName = "smtpConfig";
            private static string[] _files = new string[] {
                coreFileName + "Current",
                coreFileName + "001",
                coreFileName + "002"
            };

            public static XmlWriter xmlWriter = new XmlWriter(_files);

            public static void SetUrl(string value) {
                xmlWriter.Write(Label.SMTP.Url, value);
            }

            public static string Url {
                get {
                    return (xmlWriter.ReadValue(Label.SMTP.Url) == null) ? "" : xmlWriter.ReadValue(Label.SMTP.Url);
                }
            }

            public static void SetPort(string value) {
                xmlWriter.Write(Label.SMTP.Port, value);
            }

            public static string Port {
                get {
                    return (xmlWriter.ReadValue(Label.SMTP.Port) == null) ? "" : xmlWriter.ReadValue(Label.SMTP.Port);
                }
            }

            public static void SetAccount(string value) {
                xmlWriter.Write(Label.SMTP.Account, value);
            }

            public static string Account {
                get {
                    return (xmlWriter.ReadValue(Label.SMTP.Account) == null) ? "" : xmlWriter.ReadValue(Label.SMTP.Account);
                }
            }

            public static void SetPassword(string value) {
                xmlWriter.Write(Label.SMTP.Password, value);
            }

            public static string Password {
                get {
                    return (xmlWriter.ReadValue(Label.SMTP.Password) == null) ? "" : xmlWriter.ReadValue(Label.SMTP.Password);
                }
            }

            public static bool ConfigExists { get { return (xmlWriter.ReadValue(Label.SMTP.Url) == null) ? false : true; } }
        }

        public static void Send() {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Fake Name", "fake.name@gmail.com"));
            message.To.Add(new MailboxAddress("Mr. Roger", "address.yeah@gmil.com"));
            message.Subject = "How you doin'?";
            message.Body = new TextPart("plain") {
                Text = @"Roger we got your son"
            };
            using (var client = new SmtpClient()) {
                client.Connect("smtp.gmail.com", 587, false);
                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("account name", "password");
                client.Send(message);
                client.Disconnect(true);
            }
        }
    }
}
