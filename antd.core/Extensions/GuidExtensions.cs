using System;

namespace antd.core {
    public static class GuidExtensions {
        public static Guid ToGuid(this Guid? source) {
            return source ?? Guid.Empty;
        }

        public static byte[] ToKey(this Guid guid) {
            return Encryption.Hash(guid.ToString());
        }

        public static byte[] ToVector(this Guid guid) {
            var coreVector = new byte[16];
            Array.Copy(Encryption.Hash(guid.ToString()), 0, coreVector, 0, coreVector.Length);
            return coreVector;
        }
    }
}