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
using System.Linq;

namespace Antd.Storage {
    public class Volumes {

        public class Block {
            public string _Id { get; set; }

            public string Name { get; set; }

            public string Attributes { get; set; }

            public string Type { get; set; }

            public string UUID { get; set; }

            public string SecType { get; set; }

            public string Label { get; set; }

            public string PartLabel { get; set; }

            public string PartUUID { get; set; }

            public string Maj { get; set; }

            public string Min { get; set; }

            public string Rm { get; set; }

            public string Size { get; set; }

            public string Ro { get; set; }

            public string DiskType { get; set; }

            public string MountPoint { get; set; }

            public class Info {
                public string DiskBytes { get; set; }

                public string DiskSectors { get; set; }
            }
        }

        public class GET {
            public static string[] ByID() {
                return Terminal.Execute("ls -1 /dev/disk/by-id").ConvertCommandToModel().output.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            }

            public static string[] ByLabel() {
                return Terminal.Execute("ls -1 /dev/disk/by-label").ConvertCommandToModel().output.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            }

            public static string[] ByPartLabel() {
                return Terminal.Execute("ls -1 /dev/disk/by-partlabel").ConvertCommandToModel().output.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            }

            public static string[] ByPartUuid() {
                return Terminal.Execute("ls -1 /dev/disk/by-partuuid").ConvertCommandToModel().output.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            }

            public static string[] ByUuid() {
                return Terminal.Execute("ls -1 /dev/disk/by-uuid").ConvertCommandToModel().output.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            }
        }

        public static void PopulateBlocks() {
            var rows = Terminal.Execute("lsblk -npl").ConvertCommandToModel().output.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            foreach (var row in rows) {
                var cells = row.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                var blk = new Block();
                blk.Name = cells[0];
                blk.Maj = cells[1];
                blk.Min = cells[1];
                blk.Rm = cells[2];
                blk.Size = cells[3];
                blk.Ro = cells[4];
                blk.DiskType = cells[5];
                blk.MountPoint = (cells.Length > 6) ? cells[6] : "";
                var fromBlkid = GetBlockFromBlkid(blk.Name);
                if (fromBlkid != null) {
                    blk.Label = fromBlkid.Label;
                    blk.UUID = fromBlkid.UUID;
                    blk.PartLabel = fromBlkid.PartLabel;
                    blk.PartUUID = fromBlkid.PartUUID;
                    blk.Type = fromBlkid.Type;
                }
                SaveToDb(blk);
            }
        }

        public static List<Block> BlocksFromDd() {
            var list = DeNSo.Session.New.Get<Block>(b => b != null).OrderBy(n => n.Name).ToList();
            if (list.ToArray().Length < 1) {
                PopulateBlocks();
            }
            return list;
        }

        public static void SaveToDb(Block model) {
            model._Id = Guid.NewGuid().ToString();
            var old = DeNSo.Session.New.Get<Block>(b => b != null && b.Name == model.Name).FirstOrDefault();
            if (old != null) {
                DeNSo.Session.New.Delete(old);
            }
            DeNSo.Session.New.Set(model);
        }

        public static List<string> BlockList() {
            var blocks = new List<string>() { };
            var rows = Terminal.Execute("lsblk -lnp --output=NAME").ConvertCommandToModel().output.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            foreach (var row in rows) {
                blocks.Add(row);
            }
            return blocks;
        }

        private static Block GetBlockFromLsblk(string q) {
            return Lsblk().Where(b => b.Name == q && b != null).FirstOrDefault();
        }

        private static Block GetBlockFromBlkid(string q) {
            return Blkid().Where(b => b.Name == q && b != null).FirstOrDefault();
        }

        private static List<Block> Lsblk() {
            var list = new List<Block>() { };
            var rows = Terminal.Execute("lsblk -npl").ConvertCommandToModel().output.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            foreach (var row in rows) {
                var cells = row.Split(new String[] { " " }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                var blk = new Block();
                blk.Name = cells[0];
                blk.Maj = cells[1];
                blk.Min = cells[1];
                blk.Rm = cells[2];
                blk.Size = cells[3];
                blk.Ro = cells[4];
                blk.DiskType = cells[5];
                blk.MountPoint = (cells.Length > 6) ? cells[6] : "";
                list.Add(blk);
            }
            return list;
        }

        private static List<Block> Blkid() {
            var list = new List<Block>() { };
            var rows = Terminal.Execute("blkid").ConvertCommandToModel().output.Split(new String[] { "\n" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
            foreach (var row in rows) {
                var cells = row.Split(new String[] { ":" }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                var blk = new Block();
                blk.Name = cells[0];
                blk.Attributes = cells[1];
                var attrs = cells[1].Split(' ').ToArray();
                blk.Type = AssignValue("TYPE=", attrs);
                blk.UUID = AssignValue("UUID=", attrs);
                blk.SecType = AssignValue("SEC_TYPE=", attrs);
                blk.Label = AssignValue("LABEL=", attrs);
                blk.PartLabel = AssignValue("PARTLABEL=", attrs);
                blk.PartUUID = AssignValue("PARTUUID=", attrs);
                list.Add(blk);
            }
            return list;
        }

        private static string AssignValue(string label, string[] arr) {
            return GetValue(arr.Where(a => a.Contains(label)).FirstOrDefault());
        }

        private static string GetValue(string input) {
            if (input != null) {
                var val = input.Split('=').ToArray()[1];
                return val.Substring(1, val.Length - 2);
            }
            return "";
        }
    }
}
