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
    /// TODO: Update summary.
    /// </summary>
    internal class TempLog
    {
        private static readonly string LOG_FILENAME = Path.GetTempPath() + "cspgf.txt";

        public static void NewLog()
        {
#if (DEBUG)
            File.Create(LOG_FILENAME);
#endif
        }

        public static void LogMessageToFile(string msg)
        {
#if (DEBUG)
            msg = string.Format("{0:G}: {1}\r\n", DateTime.Now, msg);

            File.AppendAllText(LOG_FILENAME, msg);
#endif
        }
    }
}
