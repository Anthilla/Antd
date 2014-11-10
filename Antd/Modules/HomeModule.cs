
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