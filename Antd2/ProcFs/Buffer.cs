using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Antd.ProcFs {
    public struct Buffer<T, TFixed> : IDisposable
        where T : unmanaged
        where TFixed : unmanaged, IFixedBuffer {
        public static readonly int MinimumCapacity = Unsafe.SizeOf<TFixed>() / Unsafe.SizeOf<T>();

        private T[] _rentedBuffer;
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        private TFixed _fixedBuffer; // It must be non-readonly otherwise it will always load the copy of it to stack instead of reference
        // ReSharper restore FieldCanBeMadeReadOnly.Local

        public int Length { get; private set; }

        public Span<T> Span {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (Length == 0)
                    return default;

                return _rentedBuffer == null
                    ? MemoryMarshal.Cast<byte, T>(_fixedBuffer.Span).Slice(0, Length)
                    : new Span<T>(_rentedBuffer, 0, Length);
            }
        }

        public Buffer(int length) {
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length));
            Length = length;
            _rentedBuffer = Length > MinimumCapacity ? ArrayPool<T>.Shared.Rent(Length) : null;
            _fixedBuffer = default;
        }

        public void Resize(int newLength) {
            if (newLength < 0)
                throw new ArgumentOutOfRangeException(nameof(newLength));
            if (newLength > Length) {
                var currentBufferCapacity = Length > MinimumCapacity ? _rentedBuffer.Length : MinimumCapacity;
                if (newLength > currentBufferCapacity) {
                    var newBuffer = ArrayPool<T>.Shared.Rent(newLength);
                    Span.CopyTo(newBuffer);
                    if (_rentedBuffer != null)
                        ArrayPool<T>.Shared.Return(_rentedBuffer);
                    _rentedBuffer = newBuffer;
                }
            }

            Length = newLength;
        }

        public void Dispose() {
            if (_rentedBuffer != null) {
                ArrayPool<T>.Shared.Return(_rentedBuffer);
                _rentedBuffer = null;
            }
            Length = 0;
        }

        public static Buffer<byte, TFixed> FromFile(string fileName) {
            using (var stream = LightFileStream.OpenRead(fileName)) {
                var buffer = new Buffer<byte, TFixed>(Buffer<byte, TFixed>.MinimumCapacity);
                var totalReadBytes = 0;
                while (true) {
                    var readBytes = stream.Read(buffer.Span.Slice(totalReadBytes));
                    if (readBytes == 0)
                        break;

                    totalReadBytes += readBytes;
                    if (totalReadBytes == buffer.Length)
                        buffer.Resize(buffer.Length * 2);
                }
                buffer.Resize(totalReadBytes);
                return buffer;
            }
        }
    }
}