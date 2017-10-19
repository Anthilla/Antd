namespace Antd.sync {
    /// <summary>
    /// These are the possible commands to specify a copy.
    /// The first item is the offset in the original file,
    /// the second item is the length of the block to copy.
    /// </summary>
    public enum CopyDeltaCommand : byte {
        Copy_Byte_Byte = 0x45,
        Copy_Byte_Short = 0x46,
        Copy_Byte_Int = 0x47,
        Copy_Byte_Long = 0x48,

        Copy_Short_Byte = 0x49,
        Copy_Short_Short = 0x4a,
        Copy_Short_Int = 0x4b,
        Copy_Short_Long = 0x4c,

        Copy_Int_Byte = 0x4d,
        Copy_Int_Short = 0x4e,
        Copy_Int_Int = 0x4f,
        Copy_Int_Long = 0x50,

        Copy_Long_Byte = 0x51,
        Copy_Long_Short = 0x52,
        Copy_Long_Int = 0x53,
        Copy_Long_Long = 0x54,
    }
}
