
///-------------------------------------------------------------------------------------
/// Copyright (c) 2014 Anthilla S.r.l. (http://www.anthilla.com)
///
/// Licensed under the BSD licenses.
///
/// 141110
///-------------------------------------------------------------------------------------

using System.Collections.Generic;
using Nancy;
using Newtonsoft.Json;

namespace Antd
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/"] = x =>
            {
				return Response.AsRedirect("/meminfo");
            };

			Get["/meminfo"] = x =>
			{
				List<MeminfoModel> meminfo = Meminfo.GetModel();
				if (meminfo == null)
				{
					return View["page-meminfo"];
				}
				return View["page-meminfo", meminfo];
			};

			Get["/meminfo/text"] = x =>
            {
				var meminfo = Meminfo.GetText();
				return Response.AsJson(meminfo);
            };

			Get["/cpuinfo"] = x =>
			{
				List<CpuinfoModel> cpuinfo = Cpuinfo.GetModel();
				if (cpuinfo == null)
				{
					return View["page-cpuinfo"];
				}
				return View["page-cpuinfo", cpuinfo];
			};

			Get["/cpuinfo/text"] = x =>
			{
				var cpuinfo = Cpuinfo.GetText();
				return Response.AsJson(cpuinfo);
			};

			Get["/version"] = x =>
			{
				VersionModel version = Version.GetModel();
				if (version == null)
				{
					return View["page-version"];
				}
				return View["page-version", version];
			};

			Get["/version/text"] = x =>
			{
				var version = Version.GetText();
				return Response.AsJson(version);
			};

			Get["/command"] = x =>
			{
				ViewBag.Output = "";
				ViewBag.Error = "";
				return View["page-command"];
			};

			Post["/command"] = x =>
			{
				string file = (string)this.Request.Form.File;
				string args = (string)this.Request.Form.Arguments;

				CommandModel command = Command.GetModel(file, args);
				ViewBag.Output = command.output;
				ViewBag.Error = command.error;
				return View["page-command", command];
			};

//			Get["/command/get/{file}/{argument}"] = x =>
//			{
//				string file = x.file;
//				string argument = x.argument;
//
//				var a = argument.Replace("_", " ");
//				var b = a.Replace(",", "/");
//
//				string command = Command.GetText(file, b);
//				return JsonConvert.SerializeObject(command);
//			};

			Get["/command/text"] = x =>
			{
				var command = Command.GetText("", "");
				var json = JsonConvert.SerializeObject(command);
				return json;
			};
        }
    }
}