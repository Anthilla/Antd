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

using System.Dynamic;
using antdlib.CCTable;
using antdlib.Mail;
using Nancy;
using Nancy.Security;

namespace Antd.Modules {

    public class MailModule : NancyModule {

        public MailModule()
            : base("/mail") {
            this.RequiresAuthentication();

            Get["/"] = x => {
                dynamic vmod = new ExpandoObject();
                vmod.CurrentContext = Request.Path;
                vmod.CCTable = CCTableRepository.GetAllByContext(Request.Path);
                vmod.Count = CCTableRepository.GetAllByContext(Request.Path).ToArray().Length;
                dynamic smtp = new ExpandoObject();
                smtp.Url = Smtp.Settings.Url;
                smtp.Port = Smtp.Settings.Port;
                smtp.Account = Smtp.Settings.Account;
                smtp.Password = Smtp.Settings.Password;
                vmod.SMTP = smtp;
                dynamic imap = new ExpandoObject();
                imap.Url = Imap.Settings.Url;
                imap.Port = Imap.Settings.Port;
                imap.Account = Imap.Settings.Account;
                imap.Password = Imap.Settings.Password;
                vmod.IMAP = imap;
                return View["_page-mail", vmod];
            };

            Post["/smtp/config"] = x => {
                string url = Request.Form.Url;
                Smtp.Settings.SetUrl(url);
                string port = Request.Form.Port;
                Smtp.Settings.SetPort(port);
                string account = Request.Form.Account;
                Smtp.Settings.SetAccount(account);
                string passwd = Request.Form.Password;
                Smtp.Settings.SetPassword(passwd);
                return Response.AsRedirect("/mail");
            };

            Post["/imap/config"] = x => {
                string url = Request.Form.Url;
                Imap.Settings.SetUrl(url);
                string port = Request.Form.Port;
                Imap.Settings.SetPort(port);
                string account = Request.Form.Account;
                Imap.Settings.SetAccount(account);
                string passwd = Request.Form.Password;
                Imap.Settings.SetPassword(passwd);
                return Response.AsRedirect("/mail");
            };
        }
    }
}