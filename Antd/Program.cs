
///-------------------------------------------------------------------------------------
/// Copyright (c) 2014 Anthilla S.r.l. (http://www.anthilla.com)
///
/// Licensed under the BSD licenses.
///
/// 141110
///-------------------------------------------------------------------------------------


using System;
using System.Threading;
using Microsoft.Owin.Hosting;
using Nancy;
using Owin;

namespace Antd
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var stop = new ManualResetEvent(false);
            Console.CancelKeyPress +=
                (sender, e) =>
                {
                    Console.WriteLine("^C");
                    stop.Set();
                    e.Cancel = true;
                };
            var url = "http://+:7777/";
            using (WebApp.Start<Startup>(url))
            {
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "loading service");
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    service type -> server");
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "                 -> server url -> {0}", url);
                Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "service is now running");
                stop.WaitOne();
                Console.WriteLine("");
            }
        }
    }

    internal class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "loading service configuration");
            StaticConfiguration.DisableErrorTraces = false;
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    disableerrortraces -> false");
            Database.Start();
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    denso-db -> loaded");
            app.UseNancy();
            Console.WriteLine(ConsoleTime.GetTime(DateTime.Now) + "    nancy-fx -> loaded");
        }
    }

    public class Database
    {
        public static void Start()
        {
            var root = "/framework/anthilla";
//            var root = @"D:\Anthilla";
            Start(root);
        }

        public static void Start(string root)
        {
            DeNSo.Configuration.BasePath = new string[] { System.IO.Path.Combine(root, "AnthDB") };
            DeNSo.Configuration.EnableJournaling = true;
            DeNSo.Configuration.DBCheckTimeSpan = new System.TimeSpan(0, 1, 0);
            DeNSo.Configuration.ReindexCheck = new System.TimeSpan(0, 1, 0);

            DeNSo.Session.DefaultDataBase = "Data";
            DeNSo.Session.Start();
        }

        public static void ShutDown()
        {
            DeNSo.Session.ShutDown();
        }
    }
}