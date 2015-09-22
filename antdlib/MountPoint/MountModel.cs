
using System.Collections.Generic;
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
namespace antdlib.MountPoint {
    public class MountModel {
        public string _Id { get; set; }

        public string Guid { get; set; }

        public string DFPTimestamp { get; set; }

        public string Device { get; set; } = "";

        public string Path { get; set; } = "";

        public string MountedPath { get; set; } = "";

        public MountStatus MountStatus { get; set; } = MountStatus.Unmounted;

        public MountContext MountContext { get; set; } = MountContext.Core;

        public string Type { get; set; } = "";

        public string Options { get; set; } = "";

        public HashSet<string> AssociatedUnits { get; set; } = new HashSet<string>() { };

        public MountEntity MountEntity { get; set; }
    }

    public enum MountStatus : byte {
        Mounted = 1,
        Unmounted = 2,
        MountedTMP = 3,
        DifferentMount = 4,
        MountedReadOnly = 5,
        MountedReadWrite = 6,
        Error = 99
    }

    public enum MountContext : byte {
        Core = 1,
        External = 2,
        Other = 99
    }

    public enum MountEntity : byte {
        Directory = 1,
        File = 2,
        Other = 99
    }
}
