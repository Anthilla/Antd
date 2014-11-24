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
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Antd
{
    public class Command
    {
        public static string GetText(string file, string args)
        {
            string output = string.Empty;
            string error = string.Empty;

            Process process = new Process();
            process.StartInfo.FileName = file;
            process.StartInfo.Arguments = args;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();

            using (StreamReader streamReader = process.StandardOutput)
            {
                output = streamReader.ReadToEnd();
            }

            using (StreamReader streamReader = process.StandardError)
            {
                error = streamReader.ReadToEnd();
            }
            process.WaitForExit();

            Tuple<string, string> result = new Tuple<string, string>(output, error);
            string json = JsonConvert.SerializeObject(result);
            return json;
        }

        public static CommandModel GetModel(string file, string args)
        {
            string output = string.Empty;
            string error = string.Empty;

            Process process = new Process();
            process.StartInfo.FileName = file;
            process.StartInfo.Arguments = args;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.UseShellExecute = false;

            try 
            {
                process.Start();

                using (StreamReader streamReader = process.StandardOutput) {
                    output = streamReader.ReadToEnd();
                }

                using (StreamReader streamReader = process.StandardError) {
                    error = streamReader.ReadToEnd();
                }
                process.WaitForExit();

                CommandModel command = new CommandModel();
                command.date = DateTime.Now;
                command.output = output;
                command.outputTable = TextToList(output);
                command.error = error;
                command.errorTable = TextToList(error);
                return command;
            }
            catch (Exception ex) 
            {
                CommandModel command = new CommandModel();
                command.error = ex.Message;
                command.errorTable = TextToList(ex.Message);
                return command;
            }
        }

        private static List<string> TextToList(string text)
        {
            List<string> stringList = new List<string>();

            string[] rowDivider = new String[] { "\n" };
            string[] rowList = text.Split(rowDivider, StringSplitOptions.None).ToArray();
            foreach (string row in rowList)
            {
                if (row != null && row != "")
                {
                    stringList.Add(row);
                }
            }

            return stringList;
        }
    }
}