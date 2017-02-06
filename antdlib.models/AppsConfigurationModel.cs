using System.Collections.Generic;

namespace antdlib.models {

    public class AppsConfigurationModel {
        public bool IsActive { get; set; }

        public List<ApplicationModel> Apps { get; set; } = new List<ApplicationModel>();
    }

    public class ApplicationModel {
        public string Guid { get; set; } = System.Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string RepositoryName { get; set; }
        public IEnumerable<string> Exes { get; set; }
        public IEnumerable<string> WorkingDirectories { get; set; }
        public string UnitPrepare { get; set; }
        public string UnitMount { get; set; }
        public IEnumerable<string> UnitLauncher { get; set; }
    }
}
