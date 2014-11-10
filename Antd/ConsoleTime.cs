
///-------------------------------------------------------------------------------------
/// Copyright (c) 2014 Anthilla S.r.l. (http://www.anthilla.com)
///
/// Licensed under the BSD licenses.
///
/// 141110
///-------------------------------------------------------------------------------------

using System;

namespace Antd
{
    public static class ConsoleTime
    {
        public static string GetTime(DateTime dt)
        {
            var str = "[";
            str += dt.ToString("MM/dd/yy");
            str += " ";
            str += dt.ToString("H:mm:ss");
            str += "] ";
            return str;
        }
    }
}