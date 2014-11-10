
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
///         * Neither the name of the <organization> nor the
///           names of its contributors may be used to endorse or promote products
///           derived from this software without specific prior written permission.
/// 
///     THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
///     ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
///     WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
///     DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
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
using Newtonsoft.Json;

namespace Antd
{
    public class TextToJson
    {
        public static List<MeminfoModel> Meminfo(string meminfoText)
        {
            List<MeminfoModel> meminfoList = new List<MeminfoModel>();

			string[] rowDivider = new String[] { "\n" };
            string[] cellDivider = new String[] { ": " };

            string[] rowList = meminfoText.Split(rowDivider, StringSplitOptions.None).ToArray();
			foreach (string row in rowList)
            {
				if (row != null && row != "") {
					string[] cellList = row.Split (cellDivider, StringSplitOptions.None).ToArray ();
					MeminfoModel meminfo = new MeminfoModel ();
					meminfo.key = cellList [0];
					meminfo.value = cellList [1];
					meminfoList.Add (meminfo);
				}
            }

            return meminfoList;
        }

		public static List<CpuinfoModel> Cpuinfo(string cpuinfoText)
		{
			List<CpuinfoModel> cpuinfoList = new List<CpuinfoModel>();

			string[] rowDivider = new String[] { "\n" };
			string[] cellDivider = new String[] { ": " };

			string[] rowList = cpuinfoText.Split(rowDivider, StringSplitOptions.None).ToArray();
			foreach (string row in rowList)
			{
				if(row != null && row != "")
				{
					string[] cellList = row.Split(cellDivider, StringSplitOptions.None).ToArray();
					CpuinfoModel cpuinfo = new CpuinfoModel ();
					cpuinfo.key = cellList [0];
					cpuinfo.value = cellList [1];
					cpuinfoList.Add(cpuinfo);
				}
			}

			return cpuinfoList;
		}

		public static VersionModel Version(string versionText)
		{
			VersionModel version = new VersionModel ();
			version.key = "";
			version.value = versionText;
			return version;
		}
    }
}











