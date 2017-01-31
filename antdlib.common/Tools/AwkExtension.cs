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
using System.Linq;

namespace antdlib.common {
    public static class AwkExtension {

        private static readonly string Empty = string.Empty;
        private static readonly IEnumerable<string> EmptyList = new List<string>();

        public static string Print(this string input, int elementIndex, char divider = ' ') {
            if(string.IsNullOrEmpty(input)) {
                return Empty;
            }
            elementIndex = elementIndex == 0 ? 0 : elementIndex - 1;
            var inputArr = input.Split(divider);
            return elementIndex > inputArr.Length ? Empty : inputArr[elementIndex];
        }

        public static string Print(this string input, int elementIndex, string divider = " ", StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries) {
            if(string.IsNullOrEmpty(input)) {
                return Empty;
            }
            elementIndex = elementIndex == 0 ? 0 : elementIndex - 1;
            var inputArr = input.Split(new[] { divider }, option);
            return elementIndex > inputArr.Length ? Empty : inputArr[elementIndex];
        }

        public static IEnumerable<string> Print(this IEnumerable<string> inputLines, int elementIndex, char divider = ' ') {
            var inputList = inputLines as IList<string> ?? inputLines.ToList();
            if(!inputList.Any()) {
                return EmptyList;
            }
            elementIndex = elementIndex == 0 ? 0 : elementIndex - 1;
            var list = new List<string>();
            foreach(var input in inputList) {
                var inputArr = input.Split(divider);
                if(inputArr.Length < elementIndex) {
                    list.Add(inputArr[elementIndex]);
                }
            }
            return list;
        }

        public static IEnumerable<string> Print(this IEnumerable<string> inputLines, int elementIndex, string divider = " ", StringSplitOptions option = StringSplitOptions.RemoveEmptyEntries) {
            var inputList = inputLines as IList<string> ?? inputLines.ToList();
            if(!inputList.Any()) {
                return EmptyList;
            }
            elementIndex = elementIndex == 0 ? 0 : elementIndex - 1;
            var list = new List<string>();
            foreach(var input in inputList) {
                var inputArr = input.Split(new[] { divider }, option);
                if(inputArr.Length < elementIndex) {
                    list.Add(inputArr[elementIndex]);
                }
            }
            return list;
        }
    }
}
