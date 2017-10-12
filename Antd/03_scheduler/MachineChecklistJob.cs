using Antd.models;
using anthilla.scheduler;
using System;
using System.Net.NetworkInformation;

namespace Antd {
    public class MachineChecklistJob : Job {

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

        private int _repetitionIntervalTime = 1000 * 60;

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

        private const byte Ok = 0;
        private const byte Ko = 1;
        private const string internetTarget = "8.8.8.8";
        private const string internetDnsTarget = "www.google.com";

        public override void DoJob() {
            var checklist = new MachineStatusChecklistModel();
            checklist.InternetReach = PingStatus(internetTarget);
            checklist.InternetDnsReach = PingStatus(internetDnsTarget);
            Application.Checklist = checklist;
        }

        private byte PingStatus(string ip) {
            try {
                Ping pingSender = new Ping();
                PingReply reply = pingSender.Send(ip, 200);
                return reply.Status == IPStatus.Success ? Ok : Ko;
            }
            catch(Exception) {
                return Ko;
            }
        }
    }
}
