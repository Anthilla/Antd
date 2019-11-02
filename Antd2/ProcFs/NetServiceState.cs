namespace Antd.ProcFs {
    public enum NetServiceState {
        Unknown,
        Established,
        SynSent,
        SynReceived,
        FinWait1,
        FinWait2,
        TimeWait,
        Closed,
        CloseWait,
        LastAck,
        Listen,
        Closing,
        NewSynReceived
    }
}