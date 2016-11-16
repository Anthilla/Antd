using System;
using System.Collections.Generic;
using System.Linq;
using antdlib.views;
using antdlib.views.Repo;

namespace Antd.Database {
    public class BindServerOptionsRepository {

        private const string ConfigGuid = "70185A84-6D7E-45F1-B915-493141F6C526";
        private const string ViewName = "BindServerOptions";

        public BindServerOptionsSchema Default = new BindServerOptionsSchema {
            Notify = "no",
            MaxCacheSize = "128M",
            MaxCacheTtl = "108000",
            MaxNcacheTtl = "3",
            Forwarders = "8.8.8.8, 4.4.4.4",
            AllowNotify = "iif, inet",
            AllowTransfer = "iif, inet",
            Recursion = "yes",
            TransferFormat = "many-answers",
            QuerySourceAddress = "*",
            QuerySourcePort = "*",
            Version = "none",
            AllowQuery = "loif, iif, oif, lonet, inet, onet",
            AllowRecursion = "loif, iif, oif, lonet, inet, onet",
            IxfrFromDifferences = "yes",
            ListenOnV6 = "none",
            ListenOnPort53 = "loif, iif, oif",
            DnssecEnabled = "yes",
            DnssecValidation = "yes",
            DnssecLookaside = "auto",
            AuthNxdomain = "yes",
            KeyName = "",
            KeySecret = "",
            ControlAcl = "inet",
            ControlIp = "10.1.19.1",
            ControlPort = "953",
            ControlAllow = "loif, iif, oif",
            LoggingChannel = "syslog",
            LoggingDaemon = "syslogsyslog daemon",
            LoggingSeverity = "info",
            LoggingPrintCategory = "yes",
            LoggingPrintSeverity = "yes",
            LoggingPrintTime = "yes",
            TrustedKeys = "",
            AclLocalInterfaces = "127.0.0.1",
            AclInternalInterfaces = "10.1.19.1, 10.99.19.1",
            AclExternalInterfaces = "192.168.111.2, 192.168.222.2",
            AclLocalNetworks = "127.0.0.0/8",
            AclInternalNetworks = "10.1.0.0/16, 10.99.0.0/16",
            AclExternalNetworks = "192.168.111.2/32, 192.168.222.2/32",
        };

        public IEnumerable<BindServerOptionsSchema> GetAll() {
            var result = DatabaseRepository.Query<BindServerOptionsSchema>(AntdApplication.Database, ViewName);
            return result;
        }

        public BindServerOptionsSchema Get() {
            var result = DatabaseRepository.Query<BindServerOptionsSchema>(AntdApplication.Database, ViewName, schema => schema.Id == ConfigGuid || schema.Guid == ConfigGuid);
            return result.FirstOrDefault();
        }

        public bool IsEnabled() {
            return Get() != null;
        }

        public bool Set(BindServerOptionsModel obj) {
            var tryget = Get();
            if(tryget != null) {
                Delete();
            }
            obj.Id = Guid.Parse(ConfigGuid);
            obj.Guid = ConfigGuid;
            var result = DatabaseRepository.Save(AntdApplication.Database, obj, true);
            return result;
        }

        public bool Delete() {
            var result = DatabaseRepository.Delete<BindServerOptionsModel>(AntdApplication.Database, Guid.Parse(ConfigGuid));
            return result;
        }
    }
}
