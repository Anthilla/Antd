
///-------------------------------------------------------------------------------------
/// Copyright (c) 2014 Anthilla S.r.l. (http://www.anthilla.com)
///
/// Licensed under the BSD licenses.
///
/// 141110
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











