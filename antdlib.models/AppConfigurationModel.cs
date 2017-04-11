using antdlib.common;

namespace antdlib.models {
    public class AppConfigurationModel {
        public int AntdPort { get; set; } = 8084;
        public int AntdUiPort { get; set; } = 8086;
        public string DatabasePath { get; set; } = Parameter.AntdCfgDatabase;
        public string CloudAddress { get; set; } = Parameter.Cloud;
    }
}