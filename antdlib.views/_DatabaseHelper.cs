using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using antdlib.views.Repo;
using RaptorDB;

namespace antdlib.views {
    public class DatabaseHelper {
        public static IEnumerable<Type> GetTypesWithHelpAttribute() {
            var assembly = Assembly.GetAssembly(typeof (DatabaseRepository));
            return assembly.GetTypes().Where(type => type.GetCustomAttributes(typeof(RegisterViewAttribute), true).Length > 0);
        }
    }
}
