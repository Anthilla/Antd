namespace Antd.sync {
    internal class DeltaBlock<T> {
        public T Offset { get; set; }
        public T Size { get; set; }

        public void Reset(T offset, T size) {
            Offset = offset;
            Size = size;
        }

        public DeltaBlock(T offset, T size) {
            Reset(offset, size);
        }
    }
}
