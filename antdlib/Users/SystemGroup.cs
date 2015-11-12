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

using antdlib.MountPoint;
using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.Common;

namespace antdlib.Users {

    public class SystemGroup {
        private const string File = "/etc/group";

        private static readonly string MntFile = Mount.SetFilesPath(File);

        public static void SetReady() {
            if (!System.IO.File.Exists(MntFile) && System.IO.File.Exists(File)) {
                System.IO.File.Copy(File, MntFile, true);
            }
            else if (System.IO.File.Exists(MntFile) && FileSystem.IsNewerThan(File, MntFile)) {
                System.IO.File.Delete(MntFile);
                System.IO.File.Copy(File, MntFile, true);
            }
            Mount.File(File);
        }

        private static bool CheckIsActive() {
            return (MountRepository.Get(File) != null);
        }

        public static bool IsActive => CheckIsActive();

        public static void ImportGroupsToDatabase() {
            if (!System.IO.File.Exists(File))
                return;
            var groupsString = FileSystem.ReadFile(File);
            var groups = groupsString.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToArray();
            foreach (var mu in groups.Select(MapGroup)) {
                DeNSo.Session.New.Set(mu);
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
            var groupInfo = groupString.Split(new[] { ":" }, StringSplitOptions.None).ToArray();
            var groupObject = new GroupModel {
                Guid = Guid.NewGuid().ToString(),
                Alias = groupInfo[0],
                Password = groupInfo[1],
                Gid = groupInfo[2]
            };
            if (groupInfo[3].Length <= 0)
                return groupObject;
            var users = groupInfo[3].Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).ToList();
            groupObject.UserList = users;
            return groupObject;
        }

        public static void CreateGroup(string group) {
            Terminal.Terminal.Background.Execute($"groupadd {group}");
        }
    }
}