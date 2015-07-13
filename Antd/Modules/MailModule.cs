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

using Antd.CCTable;
using Antd.Mail;
using Nancy;
using Nancy.Security;
using System.Dynamic;

namespace Antd {

    public class MailModule : NancyModule {

        public MailModule()
            : base("/mail") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.CurrentContext = this.Request.Path;
                vmod.CCTable = CCTableRepository.GetAllByContext(this.Request.Path);
                vmod.Count = CCTableRepository.GetAllByContext(this.Request.Path).ToArray().Length;
                dynamic smtp = new ExpandoObject();
                smtp.Url = SMTP.Settings.Url;
                smtp.Port = SMTP.Settings.Port;
                smtp.Account = SMTP.Settings.Account;
                smtp.Password = SMTP.Settings.Password;
                vmod.SMTP = smtp;
                dynamic imap = new ExpandoObject();
                imap.Url = IMAP.Settings.Url;
                imap.Port = IMAP.Settings.Port;
                imap.Account = IMAP.Settings.Account;
                imap.Password = IMAP.Settings.Password;
                vmod.IMAP = imap;
                return View["_page-mail", vmod];
            };

            Post["/smtp/config"] = x => {
                string url = this.Request.Form.Url;
                SMTP.Settings.SetUrl(url);
                string port = this.Request.Form.Port;
                SMTP.Settings.SetPort(port);
                string account = this.Request.Form.Account;
                SMTP.Settings.SetAccount(account);
                string passwd = this.Request.Form.Password;
                SMTP.Settings.SetPassword(passwd);
                return this.Response.AsRedirect("/mail");
            };

            Post["/imap/config"] = x => {
                string url = this.Request.Form.Url;
                IMAP.Settings.SetUrl(url);
                string port = this.Request.Form.Port;
                IMAP.Settings.SetPort(port);
                string account = this.Request.Form.Account;
                IMAP.Settings.SetAccount(account);
                string passwd = this.Request.Form.Password;
                IMAP.Settings.SetPassword(passwd);
                return this.Response.AsRedirect("/mail");
            };
        }
    }
}