namespace antdlib.common {
    public enum RequestStatus {
        Empty = -1,
        OpenNew,
        OpenAssigned,
        OpenInProgress,
        OpenInfoRquested,
        OpenHold,
        OpenBlocked,
        ResolvedSolved,
        ResolvedClosed
    }
}