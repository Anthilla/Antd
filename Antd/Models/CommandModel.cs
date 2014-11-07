using System;
using System.Collections.Generic;

namespace Antd
{
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
		public DateTime date { get; set;}

		public string output { get; set; }
		public string error { get; set; }

		public List<string> outputTable { get; set; }
		public List<string> errorTable { get; set; }
	}
}

