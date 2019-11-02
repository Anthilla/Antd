using System;
using System.Collections.Generic;
using System.IO;

namespace ProcFsCore
{
    public struct Process
    {
        private static readonly int CurrentPid = Native.GetPid();

        private bool _initialized;
        
        public int Pid { get; }

        private string _name;
        public string Name
        {
            get
            {
                EnsureInitialized();
                return _name;
            }
        }

        private ProcessState _state;
        public ProcessState State
        {
            get
            {
                EnsureInitialized();
                return _state;
            }
        }

        private int _parentPid;
        public int ParentPid
        {
            get
            {
                EnsureInitialized();
                return _parentPid;
            }
        }

        private int _groupId;
        public int GroupId
        {
            get
            {
                EnsureInitialized();
                return _groupId;
            }
        }

        private int _sessionId;
        public int SessionId
        {
            get
            {
                EnsureInitialized();
                return _sessionId;
            }
        }

        private long _minorFaults;
        public long MinorFaults
        {
            get
            {
                EnsureInitialized();
                return _minorFaults;
            }
        }

        private long _majorFaults;
        public long MajorFaults
        {
            get
            {
                EnsureInitialized();
                return _majorFaults;
            }
        }

        private double _userProcessorTime;
        public double UserProcessorTime
        {
            get
            {
                EnsureInitialized();
                return _userProcessorTime;
            }
        }

        private double _kernelProcessorTime;
        public double KernelProcessorTime
        {
            get
            {
                EnsureInitialized();
                return _kernelProcessorTime;
            }
        }

        private short _priority;
        public short Priority
        {
            get
            {
                EnsureInitialized();
                return _priority;
            }
        }

        private short _nice;
        public short Nice
        {
            get
            {
                EnsureInitialized();
                return _nice;
            }
        }

        private int _threadCount;
        public int ThreadCount
        {
            get
            {
                EnsureInitialized();
                return _threadCount;
            }
        }

        private long _startTimeTicks;
        public long StartTimeTicks
        {
            get
            {
                EnsureInitialized();
                return _startTimeTicks;
            }
        }

        private long _virtualMemorySize;
        public long VirtualMemorySize
        {
            get
            {
                EnsureInitialized();
                return _virtualMemorySize;
            }
        }

        private long _residentSetSize;
        public long ResidentSetSize
        {
            get
            {
                EnsureInitialized();
                return _residentSetSize;
            }
        }

        private static readonly Func<char, bool> ZeroPredicate = ch => ch == '\0';
        private string _commandLine;
        public string CommandLine
        {
            get
            {
                if (_commandLine == null)
                {
                    try
                    {
                        using (var cmdLineBuffer = Buffer<byte, X256>.FromFile($"{ProcFs.RootPath}/{Pid}/cmdline"))
                        {
                            var cmdLineSpan = cmdLineBuffer.Span.Trim(ZeroPredicate);
                            _commandLine = cmdLineSpan.IsEmpty ? "" : cmdLineSpan.ToUtf8String();
                        }
                    }
                    catch (IOException)
                    {
                        _commandLine = "";
                    }
                }

                return _commandLine;
            }
        }
        
        private DateTime? _startTimeUtc;
        public DateTime StartTimeUtc
        {
            get
            {
                if (_startTimeUtc == null)
                    _startTimeUtc = ProcFs.BootTimeUtc + TimeSpan.FromSeconds(StartTimeTicks / (double)ProcFs.TicksPerSecond);
                return _startTimeUtc.Value;
            }
        }

        public IEnumerable<Link> OpenFiles
        {
            get
            {
                foreach (var linkFile in Directory.EnumerateFiles($"{ProcFs.RootPath}/{Pid}/fd"))
                    yield return Link.Read(linkFile);
            }
        }

        public ProcessIO IO => ProcessIO.Get(Pid);

        public static Process Current => new Process(CurrentPid);
        
        public Process(int pid)
            : this()
        {
            Pid = pid;
        }

        private void EnsureInitialized()
        {
            if (!_initialized) 
                Refresh();
        }
        
        public void Refresh()
        {
            _initialized = false;
            _commandLine = null;
            _startTimeUtc = null;
            var statReader = new Utf8FileReader<X512>($"{ProcFs.RootPath}/{Pid}/stat");
            try
            {
                // See http://man7.org/linux/man-pages/man5/proc.5.html /proc/[pid]/stat section

                // (1) pid
                statReader.SkipWord();

                // (2) name
                var name = statReader.ReadWord();
                _name = name.Slice(1, name.Length - 2).ToUtf8String();

                // (3) state
                _state = GetProcessState((char) statReader.ReadWord()[0]);

                // (4) ppid
                _parentPid = statReader.ReadInt32();

                // (5) pgrp
                _groupId = statReader.ReadInt32();

                // (6) session
                _sessionId = statReader.ReadInt32();

                // (7) tty_nr
                statReader.SkipWord();

                // (8) tpgid
                statReader.SkipWord();

                // (9) flags
                statReader.SkipWord();

                // (10) minflt
                _minorFaults = statReader.ReadInt64();

                // (11) cminflt
                statReader.SkipWord();

                // (12) majflt
                _majorFaults = statReader.ReadInt64();

                // (13) cmajflt
                statReader.SkipWord();

                // (14) utime
                _userProcessorTime = statReader.ReadInt64() / (double) ProcFs.TicksPerSecond;

                // (15) stime
                _kernelProcessorTime = statReader.ReadInt64() / (double) ProcFs.TicksPerSecond;

                // (16) cutime
                statReader.SkipWord();

                // (17) cstime
                statReader.SkipWord();

                // (18) priority
                _priority = statReader.ReadInt16();

                // (19) nice
                _nice = statReader.ReadInt16();

                // (20) num_threads
                _threadCount = statReader.ReadInt32();

                // (21) itrealvalue
                statReader.SkipWord();

                // (22) starttime
                _startTimeTicks = statReader.ReadInt64();

                // (23) vsize
                _virtualMemorySize = statReader.ReadInt64();

                // (24) rss
                _residentSetSize = statReader.ReadInt64() * Environment.SystemPageSize;
            }
            finally
            {
                statReader.Dispose();
            }

            _initialized = true;
        }

        private static ProcessState GetProcessState(char state)
        {
            switch (state)
            {
                case 'R':
                    return ProcessState.Running;
                case 'S':
                    return ProcessState.Sleeping;
                case 'D':
                    return ProcessState.Waiting;
                case 'Z':
                    return ProcessState.Zombie;
                case 'T':
                    return ProcessState.Stopped;
                case 't':
                    return ProcessState.TracingStop;
                case 'x':
                case 'X':
                    return ProcessState.Dead;
                case 'K':
                    return ProcessState.WakeKill;
                case 'W':
                    return ProcessState.Waking;
                case 'P':
                    return ProcessState.Parked;
                default:
                    return ProcessState.Unknown;
            }
        }
    }
}