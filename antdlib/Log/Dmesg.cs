///-------------------------------------------------------------------------------------
///     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
///     All rights reserved.
///
///     Redistribution and use in source and binary forms, with or without
///     modification, are permitted provided that the following conditions are met:
///         * Redistributions of source code must retain the above copyright
///           notice, this list of conditions and the following disclaimer.
///         * Redistributions in binary form must reproduce the above copyright
///           notice, this list of conditions and the following disclaimer in the
///           documentation and/or other materials provided with the distribution.
///         * Neither the name of the Anthilla S.r.l. nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
///
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
///     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
///     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
///     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
///     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
///     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
///     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
///
///     20141110
///-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace antdlib.Log {
    //http://www.linfo.org/dmesg.html
    // Usage:
    // dmesg[options]

    //Display or control the kernel ring buffer.

    //Options:
    // -C, --clear clear the kernel ring buffer
    // -c, --read-clear read and clear all messages
    // -D, --console-off disable printing messages to console
    // -E, --console-on enable printing messages to console
    // -F, --file<file> use the file instead of the kernel log buffer
    // -f, --facility<list> restrict output to defined facilities
    // -H, --human human readable output
    // -k, --kernel display kernel messages
    // -L, --color[=< when >] colorize messages (auto, always or never)
    //                               colors disabled by default
    // -l, --level<list> restrict output to defined levels
    // -n, --console-level<level> set level of messages printed to console
    // -P, --nopager               do not pipe output into a pager
    // -r, --raw print the raw message buffer
    // -S, --syslog force to use syslog(2) rather than /dev/kmsg
    // -s, --buffer-size<size> buffer size to query the kernel ring buffer
    // -u, --userspace display userspace messages
    // -w, --follow wait for new messages
    // -x, --decode decode facility and level to readable string
    // -d, --show-delta show time delta between printed messages
    // -e, --reltime show local time and time delta in readable format
    // -T, --ctime show human readable timestamp
    // -t, --notime don't print messages timestamp
    //     --time-format<format> show time stamp using format:
    //                               [delta|reltime|ctime|notime|iso]
    //    Suspending/resume will make ctime and iso timestamps inaccurate.

    // -h, --help display this help and exit
    //     -V, --version output version information and exit

    //    Supported log facilities:
    //    kern - kernel messages
    //        user - random user-level messages
    //        mail - mail system
    //      daemon - system daemons
    //        auth - security/authorization messages
    //      syslog - messages generated internally by syslogd
    //         lpr - line printer subsystem
    //        news - network news subsystem

    //    Supported log levels (priorities):
    //   emerg - system is unusable
    //   alert - action must be taken immediately
    //    crit - critical conditions
    //     err - error conditions
    //    warn - warning conditions
    //  notice - normal but significant condition
    //    info - informational
    //   debug - debug-level messages


    //For more details see dmesg(1).
    public class Dmesg {
        public static void Status() {

        }
    }
}
