
///-------------------------------------------------------------------------------------
/// Copyright (c) 2014 Anthilla S.r.l. (http://www.anthilla.com)
///
/// Licensed under the BSD licenses.
///
/// 141110
///-------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Antd
{
    public class LinqFiles
    {
        public static void GetFiles()
        {
            // Modify this path as necessary. 
            string startFolder = @"/framework/anthilla";
//            string startFolder = @"c:\LinqFiles\";

            // Take a snapshot of the file system.
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(startFolder);

            // This method assumes that the application has discovery permissions 
            // for all folders under the specified path.
            IEnumerable<System.IO.FileInfo> fileList = dir.GetFiles("*.*", System.IO.SearchOption.AllDirectories);

            string searchTerm = @"loading";
//            string searchTerm = @"Information";

            // Search the contents of each file. 
            // A regular expression created with the RegEx class 
            // could be used instead of the Contains method. 
            // queryMatchingFiles is an IEnumerable<string>. 
            var queryMatchingFiles =
                from file in fileList
                let fileText = GetFileText(file.FullName)
                where fileText.Contains(searchTerm)
                select file.FullName;

            // Execute the query.
            Console.WriteLine("The term \"{0}\" was found in:", searchTerm);
            foreach (string filename in queryMatchingFiles)
            {
                Console.WriteLine(filename);
            }

            // Keep the console window open in debug mode.
            Console.WriteLine("Press any key to exit");
        }

        // Read the contents of the file. 
		public static string GetFileText(string name)
        {
            string fileContents = String.Empty;

            // If the file has been deleted since we took  
            // the snapshot, ignore it and return the empty string. 
            if (System.IO.File.Exists(name))
            {
                fileContents = System.IO.File.ReadAllText(name);
            }
            return fileContents;
        }
    }
}
