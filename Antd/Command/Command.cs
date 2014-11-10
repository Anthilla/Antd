
///-------------------------------------------------------------------------------------
/// Copyright (c) 2014 Anthilla S.r.l. (http://www.anthilla.com)
///
/// Licensed under the BSD licenses.
///
/// 141110
///-------------------------------------------------------------------------------------
      
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Antd
{
    public class Command
    {
		public static string GetText(string file, string args)
        {
            string output = string.Empty;
            string error = string.Empty;

			Process process = new Process ();
			process.StartInfo.FileName = file;
			process.StartInfo.Arguments = args;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = false;
			process.Start ();

            using (StreamReader streamReader = process.StandardOutput)
            {
                output = streamReader.ReadToEnd();
            }

            using (StreamReader streamReader = process.StandardError)
            {
                error = streamReader.ReadToEnd();
            }
			process.WaitForExit ();

			Tuple<string, string> result = new Tuple<string, string> (output, error);
			string json = JsonConvert.SerializeObject (result);
			return json;
        }

		public static CommandModel GetModel(string file, string args)
		{
			string output = string.Empty;
			string error = string.Empty;

			Process process = new Process ();
			process.StartInfo.FileName = file;
			process.StartInfo.Arguments = args;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.UseShellExecute = false;
			process.Start ();

			using (StreamReader streamReader = process.StandardOutput)
			{
				output = streamReader.ReadToEnd();
			}

			using (StreamReader streamReader = process.StandardError)
			{
				error = streamReader.ReadToEnd();
			}
			process.WaitForExit ();

			CommandModel command = new CommandModel ();
			command.date = DateTime.Now;
			command.output = output;
			command.outputTable = TextToList(output);
			command.error = error;
			command.errorTable = TextToList(error);
			return command;
		}

		private static List<string> TextToList(string text)
		{
			List<string> stringList = new List<string>();

			string[] rowDivider = new String[] { "\n" };
			string[] rowList = text.Split(rowDivider, StringSplitOptions.None).ToArray();
			foreach (string row in rowList)
			{
				if(row != null && row != "")
				{
					stringList.Add(row);
				}
			}

			return stringList;
		}
    }
}
