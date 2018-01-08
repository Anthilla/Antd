//using anthilla.scheduler;
//using Antd.cmds;

//namespace Antd {
//    public class UpdateCloudInfoJob : Job {

//        #region [    Core Parameter    ]
//        private bool _isRepeatable = true;

//        public override bool IsRepeatable {
//            get {
//                return _isRepeatable;
//            }
//            set {
//                value = _isRepeatable;
//            }
//        }

//        private int _repetitionIntervalTime = 1000 * 60 * 5;

//        public override int RepetitionIntervalTime {
//            get {
//                return _repetitionIntervalTime;
//            }

//            set {
//                value = _repetitionIntervalTime;
//            }
//        }

//        public override string Name {
//            get {
//                return GetType().Name;
//            }

//            set {
//                value = GetType().Name;
//            }
//        }
//        #endregion

//        public override void DoJob() {
//            Cloud.Update();
//        }
//    }
//}
