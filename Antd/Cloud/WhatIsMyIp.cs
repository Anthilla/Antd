using System;
using antdlib.common;

namespace Antd.Cloud {
    public class WhatIsMyIp {
        public static string Get() {
            try {
                var api = new ApiConsumer();
                var ip = api.Get("http://whatismyip.akamai.com/");
                return ip;
            }
            catch(Exception) {
                return null;
            }
        }
    }
}
