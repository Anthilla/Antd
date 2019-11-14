using System;
using System.Buffers.Text;

namespace antdlib.ProcFs {
    public interface IUtf8Reader : IDisposable {
        ReadOnlySpan<byte> WhiteSpaces { get; }
        ReadOnlySpan<byte> LineSeparators { get; }
        bool EndOfStream { get; }

        void SkipSeparators(ReadOnlySpan<byte> separators);
        void SkipFragment(ReadOnlySpan<byte> separators, bool isSingleString = false);
        ReadOnlySpan<byte> ReadFragment(ReadOnlySpan<byte> separators);
        ReadOnlySpan<byte> ReadToEnd();
    }

    public static class Utf8ReaderExtensions {
        public static unsafe void SkipFragment<TReader>(this ref TReader reader, char separator)
            where TReader : struct, IUtf8Reader {
            var separatorsBuff = stackalloc byte[1] { (byte)separator };
            var separators = new ReadOnlySpan<byte>(separatorsBuff, 1);
            reader.SkipFragment(separators);
        }

        public static unsafe ReadOnlySpan<byte> ReadFragment<TReader>(this ref TReader reader, char separator)
            where TReader : struct, IUtf8Reader {
            var separatorsBuff = stackalloc byte[1] { (byte)separator };
            var separators = new ReadOnlySpan<byte>(separatorsBuff, 1);
            return reader.ReadFragment(separators);
        }

        public static ReadOnlySpan<byte> ReadLine<TReader>(this ref TReader reader)
            where TReader : struct, IUtf8Reader {
            return reader.ReadFragment(reader.LineSeparators);
        }

        public static void SkipLine<TReader>(this ref TReader reader)
            where TReader : struct, IUtf8Reader {
            reader.SkipFragment(reader.LineSeparators);
        }

        public static ReadOnlySpan<byte> ReadWord<TReader>(this ref TReader reader)
            where TReader : struct, IUtf8Reader {
            return reader.ReadFragment(reader.WhiteSpaces);
        }

        public static void SkipWord<TReader>(this ref TReader reader)
            where TReader : struct, IUtf8Reader {
            reader.SkipFragment(reader.WhiteSpaces);
        }

        public static void SkipWhiteSpaces<TReader>(this ref TReader reader)
            where TReader : struct, IUtf8Reader {
            reader.SkipSeparators(reader.WhiteSpaces);
        }

        public static string ReadStringWord<TReader>(this ref TReader reader)
            where TReader : struct, IUtf8Reader {
            return reader.ReadWord().ToUtf8String();
        }

        public static short ReadInt16<TReader>(this ref TReader reader, char format = default)
            where TReader : struct, IUtf8Reader {
            var word = reader.ReadWord();
            if (Utf8Parser.TryParse(word, out short result, out _, format))
                return result;
            throw new FormatException($"{word.ToUtf8String()} is not valid Int16 value");
        }

        public static int ReadInt32<TReader>(this ref TReader reader, char format = default)
            where TReader : struct, IUtf8Reader {
            var word = reader.ReadWord();
            if (Utf8Parser.TryParse(word, out int result, out _, format))
                return result;
            throw new FormatException($"{word.ToUtf8String()} is not valid Int32 value");
        }

        public static long ReadInt64<TReader>(this ref TReader reader, char format = default)
            where TReader : struct, IUtf8Reader {
            var word = reader.ReadWord();
            if (Utf8Parser.TryParse(word, out long result, out _, format))
                return result;
            throw new FormatException($"{word.ToUtf8String()} is not valid Int64 value");
        }
    }
}