using System;
using System.Runtime.CompilerServices;

namespace antdlib.ProcFs {
    public struct Utf8SpanReader : IUtf8Reader {
        private readonly unsafe byte* _pointer;
        private readonly int _length;
        private readonly ReadOnlyMemory<byte> _whiteSpaces;
        private readonly ReadOnlyMemory<byte> _lineSeparators;

        private int _position;

        private unsafe ReadOnlySpan<byte> Span {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => new ReadOnlySpan<byte>(_pointer, _length);
        }

        public ReadOnlySpan<byte> WhiteSpaces => _whiteSpaces.Span;
        public ReadOnlySpan<byte> LineSeparators => _lineSeparators.Span;
        public bool EndOfStream => _position == _length;

        public unsafe Utf8SpanReader(ReadOnlySpan<byte> span, ReadOnlyMemory<byte>? whiteSpaces = null, ReadOnlyMemory<byte>? lineSeparators = null) {
            fixed (byte* ptr = &span.GetPinnableReference())
                _pointer = ptr;
            _length = span.Length;
            _whiteSpaces = (whiteSpaces ?? Utf8FileReader<X256>.DefaultWhiteSpaces);
            _lineSeparators = (lineSeparators ?? Utf8FileReader<X256>.DefaultLineSeparators);
            _position = 0;
        }

        public void Dispose() {
        }

        public void SkipSeparators(ReadOnlySpan<byte> separators) {
            var span = Span;
            while (!EndOfStream && separators.IndexOf(span[_position]) >= 0)
                ++_position;
        }

        public void SkipFragment(ReadOnlySpan<byte> separators, bool isSingleString = false) {
            if (EndOfStream)
                return;

            var separatorPos = isSingleString
                ? Span.Slice(_position).IndexOf(separators)
                : Span.Slice(_position).IndexOfAny(separators);

            if (separatorPos < 0) {
                _position = _length;
                return;
            }

            _position += separatorPos + 1;
            SkipSeparators(separators);
        }

        public ReadOnlySpan<byte> ReadFragment(ReadOnlySpan<byte> separators) {
            if (EndOfStream)
                return default;

            var span = Span.Slice(_position);
            var separatorPos = span.IndexOfAny(separators);

            if (separatorPos < 0) {
                _position = _length;
                return span;
            }

            var result = span.Slice(0, separatorPos);
            _position += separatorPos + 1;
            SkipSeparators(separators);
            return result;
        }

        public ReadOnlySpan<byte> ReadToEnd() {
            var result = Span.Slice(_position);
            _position = _length;
            return result;
        }

        public override string ToString() => Span.ToUtf8String();
    }
}