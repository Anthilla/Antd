using System;
using System.Collections.Generic;
using System.Security.Claims;
using Nancy.Security;

namespace AntdUi2.Modules.Auth {

    /// <summary>
    /// Identità dell'utente
    /// Eredita <see cref="IUserIdentity"/> ed è usata dal framework <see cref="Nancy"/>
    /// </summary>
    public class UserIdentity : ClaimsPrincipal {
        public Guid UserGuid { get; set; }
        public string UserIdentifier { get; set; }
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }
}