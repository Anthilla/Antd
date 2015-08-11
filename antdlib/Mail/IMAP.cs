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

using antdlib.Security;
using MailKit;
using MailKit.Net.Imap;
using System;

namespace antdlib.Mail {
    public class IMAP {

        public class Settings {
            private static byte[] key = Cryptography.CoreKey();
            private static byte[] vector = Cryptography.CoreVector();

            private static string coreFileName = "imapConfig";
            private static string[] _files = new string[] {
                coreFileName + "Current",
                coreFileName + "001",
                coreFileName + "002"
            };

            public static XmlWriter xmlWriter = new XmlWriter(_files);

            public static void SetUrl(string value) {
                xmlWriter.Write(Label.IMAP.Url, value);
            }

            public static string Url {
                get {
                    return (xmlWriter.ReadValue(Label.IMAP.Url) == null) ? "" : xmlWriter.ReadValue(Label.IMAP.Url);
                }
            }

            public static void SetPort(string value) {
                xmlWriter.Write(Label.IMAP.Port, value);
            }

            public static string Port {
                get {
                    return (xmlWriter.ReadValue(Label.IMAP.Port) == null) ? "" : xmlWriter.ReadValue(Label.IMAP.Port);
                }
            }

            public static void SetAccount(string value) {
                xmlWriter.Write(Label.IMAP.Account, value);
            }

            public static string Account {
                get {
                    return (xmlWriter.ReadValue(Label.IMAP.Account) == null) ? "" : xmlWriter.ReadValue(Label.IMAP.Account);
                }
            }

            public static void SetPassword(string value) {
                xmlWriter.Write(Label.IMAP.Password, value);
            }

            public static string Password {
                get {
                    return (xmlWriter.ReadValue(Label.IMAP.Password) == null) ? "" : xmlWriter.ReadValue(Label.IMAP.Password);
                }
            }

            public static bool ConfigExists { get { return (xmlWriter.ReadValue(Label.IMAP.Url) == null) ? false : true; } }
        }

        public static void Get() {
            using (var client = new ImapClient()) {
                client.Connect("imap.gmail.com", 993, true);
                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate("account name", "password");
                // The Inbox folder is always available on all IMAP servers...
                var inbox = client.Inbox;
                inbox.Open(FolderAccess.ReadOnly);
                Console.WriteLine("Total messages: {0}", inbox.Count);
                Console.WriteLine("Recent messages: {0}", inbox.Recent);
                for (int i = 0; i < inbox.Count; i++) {
                    var message = inbox.GetMessage(i);
                    Console.WriteLine("Subject: {0}", message.Subject);
                }
                client.Disconnect(true);
            }
        }
    }
}
