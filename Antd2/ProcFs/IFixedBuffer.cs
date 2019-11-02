using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ProcFsCore
{
    public interface IFixedBuffer
    {
        Span<byte> Span { get; }
    }
    
#pragma warning disable 649
    public unsafe struct X8 : IFixedBuffer
    {
        private const int Length = 8;
        private fixed byte _buffer[Length];

        public Span<byte> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateSpan(ref _buffer[0], Length);
        }
    }

    public unsafe struct X16 : IFixedBuffer
    {
        private const int Length = 16;
        private fixed byte _buffer[Length];

        public Span<byte> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateSpan(ref _buffer[0], Length);
        }
    }

    public unsafe struct X32 : IFixedBuffer
    {
        private const int Length = 32;
        private fixed byte _buffer[Length];

        public Span<byte> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateSpan(ref _buffer[0], Length);
        }
    }

    public unsafe struct X64 : IFixedBuffer
    {
        private const int Length = 64;
        private fixed byte _buffer[Length];

        public Span<byte> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateSpan(ref _buffer[0], Length);
        }
    }

    public unsafe struct X128 : IFixedBuffer
    {
        private const int Length = 128;
        private fixed byte _buffer[Length];

        public Span<byte> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateSpan(ref _buffer[0], Length);
        }
    }

    public unsafe struct X256 : IFixedBuffer
    {
        private const int Length = 256;
        private fixed byte _buffer[Length];

        public Span<byte> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateSpan(ref _buffer[0], Length);
        }
    }

    public unsafe struct X512 : IFixedBuffer
    {
        private const int Length = 512;
        private fixed byte _buffer[Length];

        public Span<byte> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateSpan(ref _buffer[0], Length);
        }
    }

    public unsafe struct X1024 : IFixedBuffer
    {
        private const int Length = 1024;
        private fixed byte _buffer[Length];

        public Span<byte> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateSpan(ref _buffer[0], Length);
        }
    }

    public unsafe struct X2048 : IFixedBuffer
    {
        private const int Length = 2048;
        private fixed byte _buffer[Length];

        public Span<byte> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateSpan(ref _buffer[0], Length);
        }
    }

    public unsafe struct X4096 : IFixedBuffer
    {
        private const int Length = 4096;
        private fixed byte _buffer[Length];

        public Span<byte> Span
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => MemoryMarshal.CreateSpan(ref _buffer[0], Length);
        }
    }
#pragma warning restore 649
}