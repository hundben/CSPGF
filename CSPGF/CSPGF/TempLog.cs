// -----------------------------------------------------------------------
// <copyright file="TempLog.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CSPGF
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Small logging class.
    /// </summary>
    internal class TempLog
    {
        public static string GetTempPath()
        {
            string path = System.Environment.GetEnvironmentVariable("TEMP");
            if (!path.EndsWith("\\")) path += "\\";
            return path;
        }


        public static void NewLog()
        {
#if (DEBUG)
            System.IO.FileStream fs = System.IO.File.Create(GetTempPath() + "cspgf.txt");
            fs.Close();
#endif
        }

        public static void LogMessageToFile(string msg)
        {
#if (DEBUG)

            System.IO.StreamWriter sw = System.IO.File.AppendText(GetTempPath() + "cspgf.txt");
            try
            {
                sw.WriteLine(msg);
            }
            finally
            {
                sw.Close();
            }
#endif
        }

        public static void LogWithTimeStamp(string msg)
        {
#if (DEBUG)

            System.IO.StreamWriter sw = System.IO.File.AppendText(GetTempPath() + "cspgf.txt");
            try
            {
                string logLine = System.String.Format(
                    "{0:G}: {1}.", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
            }
            finally
            {
                sw.Close();
            }
#endif
        }
    }
}
