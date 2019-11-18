using System.IO;

namespace antd.core {
    public class CommonStream {
        public static void CopyTo(Stream src, Stream dest) {
            var bytes = new byte[4096];
            int cnt;
            while((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
                dest.Write(bytes, 0, cnt);
            }
        }
    }
}
