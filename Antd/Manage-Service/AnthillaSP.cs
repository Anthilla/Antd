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

using Antd.UnitFiles;

namespace Antd.ServiceManagement {

    public class AnthillaSP {

        public static CommandModel EnableAnthillaServer() {
            return Systemctl.Enable("/cfg/anthilla.units.d/anthillaserver.service");
        }

        public static CommandModel EnableAnthillaSP() {
            return Systemctl.Enable("/cfg/anthilla.units.d/anthillasp.service");
        }

        public static CommandModel StartAnthillaServer() {
            return Systemctl.Start("anthillaserver.service");
        }

        public static CommandModel StartAnthillaSP() {
            return Systemctl.Start("anthillasp.service");
        }

        public static CommandModel StopAnthillaServer() {
            return Systemctl.Stop("anthillaserver.service");
        }

        public static CommandModel StopAnthillaSP() {
            return Systemctl.Stop("anthillasp.service");
        }

        public static CommandModel StatusAnthillaServer() {
            return Systemctl.Status("anthillaserver.service");
        }

        public static CommandModel StatusAnthillaSP() {
            return Systemctl.Status("anthillasp.service");
        }

        public static string AnthillaServerPID { get { return Service.GetPID("AnthillaServer.exe"); } }

        public static string AnthillaSPPID { get { return Service.GetPID("AnthillaSP.exe"); } }
    }
}