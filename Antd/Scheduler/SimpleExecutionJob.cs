//using anthilla.scheduler;

//namespace Antd.Scheduler {
//    public class SimpleExecutionJob : Job {

//        #region [    Core Parameter    ]
//        private bool _isRepeatable = false;

//        public override bool IsRepeatable {
//            get {
//                return _isRepeatable;
//            }
//            set {
//                value = _isRepeatable;
//            }
//        }

//        private int _repetitionIntervalTime = int.MinValue;

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
//            System.Console.WriteLine(string.Format("The Job \"{0}\" was executed.", Name));
//        }
//    }
//}
