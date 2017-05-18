
using System.Collections.Generic;

namespace antdlib.config.Antd_SVCS {
    public enum ServiceDataType : byte {
        Boolean = 1,
        String = 2,
        StringArray = 3,
        DataArray = 4,
        Section = 97,
        Disabled = 98,
        Other = 99
    }

    public class ServiceSamba {
        public string DataKey { get; set; }

        public string DataValue { get; set; }

        public string DataFilePath { get; set; }
    }

    public class ServiceBind {
        public string DataKey { get; set; }

        public string DataValue { get; set; }

        public string DataFilePath { get; set; }

        public string TypeIsDataArray { get; set; } = "false";
    }

    public class ServiceDhcp {
        public string DataKey { get; set; }

        public string DataValue { get; set; }

        public string DataFilePath { get; set; }

        public string TypeIsDataArray { get; set; } = "false";
    }

    public class Helper {
        public class ServiceData {
            public static ServiceDataType SupposeDataType(string value) {
                if(value == "true" || value == "True" ||
                   value == "false" || value == "False" ||
                   value == "yes" || value == "Yes" ||
                   value == "no" || value == "No") {
                    return ServiceDataType.Boolean;
                }
                if(value.Contains(";")) {
                    return ServiceDataType.StringArray;
                }
                return value.Contains("aaa") ? ServiceDataType.DataArray : ServiceDataType.String;
            }

            public static KeyValuePair<string, string> SupposeBooleanVerbs(string value) {
                switch(value) {
                    case "true":
                    case "false":
                        return new KeyValuePair<string, string>("true", "false");
                    case "True":
                    case "False":
                        return new KeyValuePair<string, string>("True", "False");
                    case "yes":
                    case "no":
                        return new KeyValuePair<string, string>("yes", "no");
                    case "Yes":
                    case "No":
                        return new KeyValuePair<string, string>("Yes", "No");
                }
                return new KeyValuePair<string, string>("", "");
            }
        }
    }
}
