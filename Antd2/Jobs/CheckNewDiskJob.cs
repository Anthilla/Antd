using Antd2.cmds;
using Antd2.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Antd2.Jobs {
    public class CheckNewDiskJob : Job {

        private int _repetitionIntervalTime = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;

        #region [    Core Parameter    ]
        private bool _isRepeatable = true;

        public override bool IsRepeatable {
            get {
                return _isRepeatable;
            }
            set {
                value = _isRepeatable;
            }
        }

        public override int RepetitionIntervalTime {
            get {
                return _repetitionIntervalTime;
            }

            set {
                value = _repetitionIntervalTime;
            }
        }

        public override string Name {
            get {
                return GetType().Name;
            }

            set {
                value = GetType().Name;
            }
        }
        #endregion

        private static List<string> DiskList = null;

        public static readonly IDictionary<string, bool> NewDiskNotify = new Dictionary<string, bool>();

        public override void DoJob() {
            var disks = Lsblk.GetDisks();

            if (DiskList == null) {
                DiskList = disks;
                foreach (var disk in DiskList) {
                    NewDiskNotify[disk] = false;
                }
            }
            else {
                var addDisks = disks.Except(DiskList).ToList();

                foreach (var disk in addDisks) {
                    NewDiskNotify[disk] = true;
                }
                //var removedDisks = DiskList.Except(disks).ToList();

                DiskList = disks;
            }
        }
    }
}
