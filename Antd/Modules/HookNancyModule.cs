using Nancy;
using Nancy.Routing;
using System;

namespace Antd.Modules {
    public class HookNancyModule : NancyModule {
        //todo far ineritare questa classe da tutti gli altri Moduli di Nancy
        private static string url;

        //public HookNancyModule(string v) {
        //    url = v;
        //}

        public HookNancyModule() : base("/base") {

            Before += y => {
                Console.WriteLine("Before");
                Console.WriteLine(Request.Url);
                Console.WriteLine(Request.Path);
                Console.WriteLine(Request.Method);
                //log here
                return null;
            };

            After += y => {
                //log here too
                Console.WriteLine("After");
            };
        }
    }

    public class HookTwoNancyModule : HookNancyModule {
        public HookTwoNancyModule() /*: base("/base")*/ {

            Get["Hook page", "/hook"] = x => {
                return Response.AsText("Hello World!");
            };
        }
    }
}
