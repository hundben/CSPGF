using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF
{
    public class LinearizerException : Exception
    {
        //public String s { get; private set; }
        LinearizerException(String _str) : base(_str)
        {
            //s = _str;
        }
        LinearizerException()
        {
        }

        public override String ToString()
        {
            return "LinearizerException: " + base.ToString();
        }
    }
}
