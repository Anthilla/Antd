namespace antdlib.models {
    public class CaConfigurationModel {
        public bool IsActive { get; set; }

        public string KeyPassout { get; set; } = "";
        public string RootCountryName { get; set; } = "IT";
        public string RootStateOrProvinceName { get; set; } = "";
        public string RootLocalityName { get; set; } = "";
        public string RootOrganizationName { get; set; } = "";
        public string RootOrganizationalUnitName { get; set; } = "";
        public string RootCommonName { get; set; } = "default";
        public string RootEmailAddress { get; set; } = "";
    }
}