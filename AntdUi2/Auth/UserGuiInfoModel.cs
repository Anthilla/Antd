using System;

namespace AntdUi2.Modules.Auth {
    public class UserGuiInfoModel {
        public Guid Guid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Alias { get; set; }
        public string[] FunctionCodes { get; set; }
    }
}
