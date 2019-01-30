﻿using System.Collections.Generic;
using WatsonWebserver;

namespace Kvpbase {
    public partial class StorageServer
    {
        public static HttpResponse GetTasksCount(RequestMetadata md)
        {
            List<string> subdirectories = new List<string>();
            List<string> files = new List<string>();
            long bytes = 0;

            if (Common.WalkDirectory(
                CurrentSettings.Environment,
                0,
                CurrentSettings.Tasks.Directory,
                false,
                out subdirectories,
                out files,
                out bytes,
                true))
            {
                return new HttpResponse(md.CurrentHttpRequest, true, 200, null, "text/plain", files.Count, true);
            }
            else
            {
                return new HttpResponse(md.CurrentHttpRequest, false, 500, null, "application/json", null, true);
            }
        }
    }
}