using System;
using anthilla.core;

namespace Antd.Cloud {
    public class WhatIsMyIp {
        public static string Get() {
            try {
                var api = new ApiConsumer();
                var ip = api.GetString("http://whatismyip.akamai.com/");
                return ip;
            }
            catch(Exception) {
                return null;
            }
        }
    }
}
