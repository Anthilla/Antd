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

using antdlib.Models;

namespace antdlib.Systemd {

    public class Systemctl {

        public static CommandModel DaemonReload() {
            Log.Logger.TraceMethod("Systemd", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            return Terminal.Execute("systemctl daemon-reload").ConvertCommandToModel();
        }

        public static CommandModel Start(string unit) {
            Log.Logger.TraceMethod("Systemd", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            return Terminal.Execute("systemctl start " + unit).ConvertCommandToModel();
        }

        public static CommandModel Stop(string unit) {
            Log.Logger.TraceMethod("Systemd", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            return Terminal.Execute("systemctl stop " + unit).ConvertCommandToModel();
        }

        public static CommandModel Restart(string unit) {
            Log.Logger.TraceMethod("Systemd", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            return Terminal.Execute("systemctl restart " + unit).ConvertCommandToModel();
        }

        public static CommandModel Reload(string unit) {
            Log.Logger.TraceMethod("Systemd", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            return Terminal.Execute("systemctl reload " + unit).ConvertCommandToModel();
        }

        public static CommandModel Status(string unit) {
            Log.Logger.TraceMethod("Systemd", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            return Terminal.Execute("systemctl status " + unit).ConvertCommandToModel();
        }

        public static CommandModel IsEnabled(string unit) {
            Log.Logger.TraceMethod("Systemd", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            return Terminal.Execute("systemctl is-enabled " + unit).ConvertCommandToModel();
        }

        public static CommandModel Enable(string unit) {
            Log.Logger.TraceMethod("Systemd", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            return Terminal.Execute("systemctl enable " + unit).ConvertCommandToModel();
        }

        public static CommandModel Disable(string unit) {
            Log.Logger.TraceMethod("Systemd", $"{System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName}.{System.Reflection.MethodBase.GetCurrentMethod().Name}");
            return Terminal.Execute("systemctl disable " + unit).ConvertCommandToModel();
        }
    }
}
