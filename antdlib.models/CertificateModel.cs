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

namespace antdlib.models {
    public enum CertificateAuthorityLevel : byte {
        Root = 1,
        Intermediate = 2,
        Common = 3,
        Other = 99
    }

    public enum CertificateAssignment : byte {
        Service = 1,
        User = 2,
        DomainController = 3,
        SmartCard = 4,
        Other = 99
    }

    public class CertificateModel {
        public string Guid { get; set; }
        public bool IsPresent { get; set; }
        public bool IsRevoked { get; set; } = false;
        public string CertificateGuid { get; set; }
        public string CertificatePath { get; set; }
        public string CertificateDerPath { get; set; }
        public string CertificatePfxPath { get; set; }
        public string CertificateCountryName { get; set; }
        public string CertificateStateProvinceNameh { get; set; }
        public string CertificateLocalityName { get; set; }
        public string CertificateOrganizationName { get; set; }
        public string CertificateOrganizationalUnitName { get; set; }
        public string CertificateCommonName { get; set; }
        public string CertificateEmailAddress { get; set; }
        public string CertificatePassphrase { get; set; }
        public string CertificateBytes { get; set; }
        public bool IsProtectedByPassphrase { get; set; }

        public DateTime ReleaseDateTime { get; set; }
        public DateTime ExpirationDateTime { get; set; }

        public CertificateAuthorityLevel CertificateAuthorityLevel { get; set; }
        public CertificateAssignment CertificateAssignment { get; set; }
        public IEnumerable<string> AssignmentUserGuids { get; set; }
        public string AssignmentGuid { get; set; }

        public string AssignmentServiceGuid { get; set; }
        public string AssignmentServiceAlias { get; set; }
    }
}
