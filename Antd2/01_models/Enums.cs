namespace Antd2.models {
    public enum SystemctlType : sbyte {
        none = -1,
        Service,
        Mount,
        Timer,
        Target
    }

    public enum AuthenticationType : sbyte {
        none = -1,
        simple
    }

    public enum NetworkAdapterMembership : sbyte {
        none = -1,
        InternalNetwork,
        ExternalNetwork
    }

    public enum NetworkAdapterType : sbyte {
        none = -1,
        Physical,
        Virtual,
        Bond,
        Bridge
    }
}
