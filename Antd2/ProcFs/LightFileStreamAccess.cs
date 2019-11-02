using System;

namespace Antd.ProcFs {
    [Flags]
    public enum LightFileStreamAccess {
        ReadOnly = 0x000000,
        WriteOnly = 0x000001,
        ReadWrite = 0x000002,
        Create = 0x000040,
        Exclusive = 0x000080,
        NoControlTTY = 0x000100,
        Truncate = 0x000200,
        Append = 0x000400,
        NoBlock = 0x000800,
        Synchronized = 0x001000,
        Direct = 0x004000,
        LargeFile = 0x008000,
        Directory = 0x010000,
        NoFollow = 0x020000,
        NoAccessTime = 0x040000,
        CloseOnExec = 0x080000,
        Path = 0x200000,
        TempFile = 0x400000
    }
}