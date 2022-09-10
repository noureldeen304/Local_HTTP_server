using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {

        static public StreamWriter sr = new StreamWriter("log.txt");
        public static void LogException(string ex)
        {            
            sr.WriteLine(ex+ "    (" + DateTime.Now + ")");
            sr.Flush();
        }
    }
}
