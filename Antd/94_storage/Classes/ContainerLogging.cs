﻿namespace Kvpbase {
    public class ContainerLogging {
        #region Public-Members

        public int? Enabled { get; set; }
        public int? ReadContainer { get; set; }
        public int? ReadObject { get; set; }
        public int? CreateContainer { get; set; }
        public int? CreateObject { get; set; }
        public int? DeleteContainer { get; set; }
        public int? DeleteObject { get; set; }

        #endregion

        #region Constructors-and-Factories

        public ContainerLogging() {

        }

        #endregion
    }
}
