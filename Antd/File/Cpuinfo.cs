
///-------------------------------------------------------------------------------------
/// Copyright (c) 2014 Anthilla S.r.l. (http://www.anthilla.com)
///
/// Licensed under the BSD licenses.
///
/// 141110
///-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Antd
{
	public class Cpuinfo
	{
		public static string GetText()
		{
			string meminfoContent = "";
			meminfoContent = LinqFiles.GetFileText("/proc/cpuinfo");

			string meminfoJson = JsonConvert.SerializeObject(meminfoContent);
			return meminfoJson;
		}

		public static List<CpuinfoModel> GetModel()
		{
			string meminfoContent = "";
			meminfoContent = LinqFiles.GetFileText("/proc/cpuinfo");

			var meminfo = TextToJson.Cpuinfo(meminfoContent);
			return meminfo;
		}
	}
}

