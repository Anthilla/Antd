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

namespace antdlib.Users {

    public enum UserType : byte {
        IsSystemUser = 0,
        IsApplicationUser = 1,
        Master = 99
    }

    public class UserModel {

        public string _Id { get; set; }

        public string Guid { get; set; }

        public string UID { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Alias { get; set; }

        public SystemUserPassword Password { get; set; }

        public string GroupID { get; set; }

        public string Info { get; set; }

        public string HomeDirectory { get; set; }

        public string LoginShell { get; set; }

        public string LastChanged { get; set; }

        public string MinimumNumberOfDays { get; set; }

        public string MaximumNumberOfDays { get; set; }

        public string Warn { get; set; }

        public string Inactive { get; set; }

        public string Expire { get; set; }

        public UserType UserType { get; set; }
    }

    public class SystemUserPassword {
        public string Type { get; set; }

        public string Salt { get; set; }

        public string Result { get; set; }

    }

    public class AuthUser {
        public string Name { get; set; }

        public string Email { get; set; }

        public Guid Guid { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public UserType UserType { get; set; }
    }
}
