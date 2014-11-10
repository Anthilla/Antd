
///-------------------------------------------------------------------------------------
/// Copyright (c) 2014 Anthilla S.r.l. (http://www.anthilla.com)
///
/// Licensed under the BSD licenses.
///
/// 141110
///-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Antd
{
    #region Version Model
    public class VersionModel
	{
		public string key { get; set; }
		public string value { get; set; }
	}
    #endregion Version Model

    #region CpuInfo Model
    public class CpuinfoModel
    {
        public string key { get; set; }
        public string value { get; set; }
    }
    #endregion CpuInfo Model

    #region MemInfo Model
    public class MeminfoModel
    {
        public string key { get; set; }
        public string value { get; set; }
    }
    #endregion MemInfo Model

    #region Command Model
    public class OutputModel
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class ErrorModel
    {
        public string key { get; set; }
        public string value { get; set; }
    }

    public class CommandModel
    {
        public DateTime date { get; set; }

        public string output { get; set; }
        public string error { get; set; }

        public List<string> outputTable { get; set; }
        public List<string> errorTable { get; set; }
    }
    #endregion Command Model
}

