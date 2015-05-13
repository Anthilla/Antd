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

using System;
using System.Collections.Generic;

namespace Antd {

    #region Version Model

    public class VersionModel {

        public string key { get; set; }

        public string value { get; set; }
    }

    #endregion Version Model

    #region CpuInfo Model

    public class CpuinfoModel {

        public string key { get; set; }

        public string value { get; set; }
    }

    #endregion CpuInfo Model

    #region MemInfo Model

    public class MeminfoModel {

        public string key { get; set; }

        public string value { get; set; }
    }

    #endregion MemInfo Model

    #region Network Model

    public class NetworkModel {

        public string hostname { get; set; }
    }

    #endregion Network Model

    #region Command Model

    public class OutputModel {

        public string key { get; set; }

        public string value { get; set; }
    }

    public class ErrorModel {

        public string key { get; set; }

        public string value { get; set; }
    }

    public class CommandModel {

        public Tuple<string, string> input { get; set; }

        public DateTime date { get; set; }

        public string output { get; set; }

        public string error { get; set; }

        public List<string> outputTable { get; set; }

        public List<string> errorTable { get; set; }

        public bool isError() {
            return !String.IsNullOrEmpty(this.error);
        }
    }

    #endregion Command Model

    #region Directory Model

    public class _dirObject {

        public bool isDirectory { get; set; }

        public bool isFile { get; set; }
    }

    public class AntdirModel : _dirObject {

        public string name { get; set; }
    }

    public class DirItemModel {

        public bool isFile { get; set; }

        public string path { get; set; }

        public string name { get; set; }
    }

    #endregion Directory Model

    #region Unit Model

    public class UnitModel {

        public string name { get; set; }

        public string status { get; set; }
    }

    #endregion Unit Model

    #region Sysctl Model

    public class SysctlModel {

        public string param { get; set; }

        public string value { get; set; }
    }

    #endregion Sysctl Model

    #region Mount Model

    public class MountModel {

        public string device { get; set; }

        public string mountpoint { get; set; }

        public string fstype { get; set; }

        public string rorw { get; set; }

        public string dv1 { get; set; }

        public string dv2 { get; set; }
    }

    #endregion Mount Model

    #region User Model

    public class UserModel {

        public string username { get; set; }

        public string password { get; set; }

        public string lastchanged { get; set; }

        public string minimumnumberofdays { get; set; }

        public string maximumnumberofdays { get; set; }

        public string warn { get; set; }

        public string inactive { get; set; }

        public string expire { get; set; }
    }

    #endregion User Model

    #region Networkd Model

    public class NetworkdModel {

        public string matchName { get; set; }

        public string matchHost { get; set; }

        public string matchVirtualization { get; set; }

        public string networkDHCP { get; set; }

        public string networkDNS { get; set; }

        public string networkBridge { get; set; }

        public string networkIPForward { get; set; }

        public string addressAddress { get; set; }

        public string routeGateway { get; set; }
    }

    #endregion Networkd Model

    #region ConfigFile Model

    public class ConfigFileModel {

        public string _Id { get; set; }

        public string timestamp { get; set; }

        public int version { get; set; }

        public string content { get; set; }

        public string path { get; set; }
    }

    #endregion ConfigFile Model
}