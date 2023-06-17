using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        static StreamWriter sr;
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime 

            sr = new StreamWriter("log.txt");
            sr.WriteLine(DateTime.Now.ToString("h:mm:ss tt"));
            sr.WriteLine(ex.Message);
            sr.Close();

        }
    }
}