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

using antdlib.MountPoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace antdlib.Users {

    public class SystemGroup {

        private static string file = "/etc/group";

        private static string FILE = Mount.SetFILESPath(file);

        public static void SetReady() {
            if (!File.Exists(FILE)) {
                File.Copy(file, FILE, true);
            }
            Mount.File(file);
        }

        private static bool CheckIsActive() {
            var mount = MountRepository.Get(file);
            return (mount == null) ? false : true;
        }

        public static bool IsActive { get { return CheckIsActive(); } }

        public static void ImportGroupsToDatabase() {
            if (File.Exists(file)) {
                var groupsString = FileSystem.ReadFile(file);
                var groups = groupsString.Split(new String[] { Environment.NewLine }, StringSplitOptions.None).ToArray();
                foreach (var group in groups) {
                    var mu = MapGroup(group);
                    DeNSo.Session.New.Set(mu);
                }
            }
        }

        public static IEnumerable<GroupModel> GetAllFromDatabase() {
            var users = DeNSo.Session.New.Get<GroupModel>().ToList();
            if (users.Count < 1) {
                ImportGroupsToDatabase();
            }
            return users;
        }

        private static GroupModel MapGroup(string groupString) {
            var groupInfo = groupString.Split(new String[] { ":" }, StringSplitOptions.None).ToArray();
            GroupModel group = new GroupModel() { };
            if (groupInfo.Length > 3) {
                group.Guid = Guid.NewGuid().ToString();
                group.Alias = groupInfo[0];
                group.Password = groupInfo[1];
                group.GID = groupInfo[2];
                var users = new List<string>() { };
                if (groupInfo[3].Length > 0) {
                    users = groupInfo[3].Split(new String[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                group.UserList = users;
            }
            return group;
        }

        public static void CreateGroup(string group) {
            Terminal.Execute($"groupadd {group}");
        }
    }
}