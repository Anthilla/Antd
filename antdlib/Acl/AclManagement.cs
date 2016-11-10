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
using antdlib.common;
using antdlib.common.Tool;

namespace antdlib.Acl {

    public class AclManagement {

        private static readonly Bash Bash = new Bash();

        public class Permissions {
            public class Rwx {
                public string Value { get; } = "rwx";
                public int N { get; } = 7;
            }
            public class Rw {
                public string Value { get; } = "rw";
                public int N { get; } = 6;
            }
            public class Rx {
                public string Value { get; } = "rx";
                public int N { get; } = 5;
            }
            public class R {
                public string Value { get; } = "r";
                public int N { get; } = 4;
            }
            public class Wx {
                public string Value { get; } = "wx";
                public int N { get; } = 3;
            }
            public class W {
                public string Value { get; } = "w";
                public int N { get; } = 2;
            }
            public class X {
                public string Value { get; } = "x";
                public int N { get; } = 1;
            }
            public class O {
                public string Value { get; } = "";
                public int N { get; } = 0;
            }
        }

        public static void GetAcl(string path) {
            Bash.Execute($"getfacl {path}", false);
        }

        public static void SetUserAcl(string path, string perms, string user = "") {
            try {
                var r = Bash.Execute($"setfacl -R -m \"u:{user}:{perms}\" {path}", false);
                if(r.Trim().Length > 0) {
                    throw new Exception(r);
                }
            }
            catch(Exception ex) {
                ConsoleLogger.Warn($"There's been an error while setting acl: {ex.Message}");
            }
        }

        public static void SetGroupAcl(string path, string perms, string group = "") {
            try {
                var r = Bash.Execute($"setfacl -R -m \"g:{group}:{perms}\" {path}");
                if(r.Trim().Length > 0) {
                    throw new Exception(r);
                }
            }
            catch(Exception ex) {
                ConsoleLogger.Warn($"There's been an error while setting acl: {ex.Message}");
            }
        }

        public static void RemoveUserAcl(string path, string user = "") {
            try {
                var r = Bash.Execute($"setfacl -R -x \"u:{user}\" {path}");
                if(r.Trim().Length > 0) {
                    throw new Exception(r);
                }
            }
            catch(Exception ex) {
                ConsoleLogger.Warn($"There's been an error while setting acl: {ex.Message}");
            }
        }

        public static void RemoveGroupAcl(string path, string group = "") {
            try {
                var r = Bash.Execute($"setfacl -R -x \"g:{group}\" {path}");
                if(r.Trim().Length > 0) {
                    throw new Exception(r);
                }
            }
            catch(Exception ex) {
                ConsoleLogger.Warn($"There's been an error while setting acl: {ex.Message}");
            }
        }

        public static void SetOwner(string path, string userOwner, string groupOwner) {
            try {
                var r = Bash.Execute($"chown {userOwner}:{groupOwner} -R {path}");
                if(r.Trim().Length > 0) {
                    throw new Exception(r);
                }
            }
            catch(Exception ex) {
                ConsoleLogger.Warn($"There's been an error while setting acl: {ex.Message}");
            }
        }
    }
}