﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Antd.Users {

    public enum UserType : byte {
        IsSystemUser = 0,
        IsApplicationUser = 1,
        Master = 99
    }

    public class UserModel {

        public string _Id { get; set; }

        public string Guid { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Alias { get; set; }

        public SystemUserPassword Password { get; set; }

        public string LastChanged { get; set; }

        public string MinimumNumberOfDays { get; set; }

        public string MaximumNumberOfDays { get; set; }

        public string Warn { get; set; }

        public string Inactive { get; set; }

        public string Expire { get; set; }

        public UserType UserType { get; set; }
    }

    public class SystemUserPassword {
        public string Type { get; set; }

        public string Salt { get; set; }

        public string Result { get; set; }

    }

    public class AuthUser {
        public string Name { get; set; }

        public Guid Guid { get; set; }
        
        public string Password { get; set; }

        public string Salt { get; set; }
        
        public UserType UserType { get; set; }
    }
}
