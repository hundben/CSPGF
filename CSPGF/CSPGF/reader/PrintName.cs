using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class PrintName
    {
        String absName;
        String printName;

        public PrintName(String _absName, String _printName)
        {
            absName = _absName;
            printName = _printName;
        }

        public override String ToString()
        {
            return "Abstract Name : " + absName + " , Print Name : " + printName;
        }
    }
}
