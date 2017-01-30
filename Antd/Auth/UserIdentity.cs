using System;
using System.Collections.Generic;
using Nancy.Security;

namespace Antd.Auth {
    public class UserIdentity : IUserIdentity {
        public Guid UserGuid { get; set; }
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }
}