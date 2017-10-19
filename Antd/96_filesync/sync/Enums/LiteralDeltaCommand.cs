namespace Antd.sync {
    /// <summary>
    /// This table contains all the delta commands for literal data
    /// </summary>
    public enum LiteralDeltaCommand : byte {
        /// <summary>
        /// The next byte contains the number of bytes with literal data that follows
        /// </summary>
        LiteralSizeByte = 0x41,
        /// <summary>
        /// The next short contains the number of bytes with literal data that follows
        /// </summary>
        LiteralSizeShort = 0x42,
        /// <summary>
        /// The next integer contains the number of bytes with literal data that follows
        /// </summary>
        LiteralSizeInt = 0x43,
        /// <summary>
        /// The next long contains the number of bytes with literal data that follows
        /// </summary>
        LiteralSizeLong = 0x44,

    }
}
