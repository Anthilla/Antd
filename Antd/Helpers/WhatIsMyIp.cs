using anthilla.core;

namespace Antd.Helpers {
    public class WhatIsMyIp {
        public static string Get() {
            return ApiConsumer.GetString("http://whatismyip.akamai.com/");
        }
    }
}
