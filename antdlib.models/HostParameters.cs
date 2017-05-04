using System.Collections.Generic;

namespace antdlib.models {

    public class HostParameters {

        #region [    modules    ]
        public List<string> Modprobes { get; set; } = new List<string>();
        public List<string> Rmmod { get; set; } = new List<string>();
        public List<string> ModulesBlacklist { get; set; } = new List<string>();
        #endregion

        #region [    services    ]
        public List<string> ServicesStart { get; set; } = new List<string>();
        public List<string> ServicesStop { get; set; } = new List<string>();
        #endregion

        #region [    parameters    ]
        public List<string> OsParameters { get; set; } = new List<string>();
        #endregion

        #region [    commands    ]
        public List<Control> StartCommands { get; set; } = new List<Control>();
        public List<Control> EndCommands { get; set; } = new List<Control>();
        #endregion
    }
}
