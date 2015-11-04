//-------------------------------------------------------------------------------------
//     Copyright (c) 2014, Anthilla S.r.l. (http://www.anthilla.com)
//     All rights reserved.
//
//     Redistribution and use in source and binary forms, with or without
//     modification, are permitted provided that the following conditions are met:
//         * Redistributions of source code must retain the above copyright
//           notice, this list of conditions and the following disclaimer.
//         * Redistributions in binary form must reproduce the above copyright
//           notice, this list of conditions and the following disclaimer in the
//           documentation and/or other materials provided with the distribution.
//         * Neither the name of the Anthilla S.r.l. nor the
//           names of its contributors may be used to endorse or promote products
//           derived from this software without specific prior written permission.
//
//     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
//     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//     DISCLAIMED. IN NO EVENT SHALL ANTHILLA S.R.L. BE LIABLE FOR ANY
//     DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//     (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//     LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//     ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//     (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//     SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
//     20141110
//-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace antdlib.Directories {

    public class DirectoryFinder {
        public HashSet<string> List { get; } = new HashSet<string>();

        public DirectoryFinder(string path, string pattern) {
            Find(path, pattern, true);
        }

        private void Find(string path, string pattern, bool getSubDir) {
            string[] files = null;
            string[] subDirs = null;
            string[] foundDirs = null;

            try {
                files = Directory.GetFiles(path, pattern);
            }
            catch (UnauthorizedAccessException e) {
                Console.WriteLine(e.Message);
            }
            catch (DirectoryNotFoundException e) {
                Console.WriteLine(e.Message);
            }
            if (files != null) {
                foreach (var f in files) {
                    List.Add(Path.GetFullPath(f));
                }
            }

            try {
                subDirs = Directory.GetDirectories(path);
                foundDirs = Directory.GetDirectories(path, pattern);
            }
            catch (UnauthorizedAccessException e) {
                Console.WriteLine(e.Message);
            }
            catch (DirectoryNotFoundException e) {
                Console.WriteLine(e.Message);
            }
            if (foundDirs != null) {
                foreach (var fd in foundDirs) {
                    List.Add(Path.GetFullPath(fd));
                }
            }
            if (subDirs == null)
                return;
            foreach (var d in subDirs.Where(d => getSubDir)) {
                Find(d, pattern, true);
            }
        }
    }
}