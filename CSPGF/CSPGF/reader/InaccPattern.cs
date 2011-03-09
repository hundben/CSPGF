using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class InaccPattern : Pattern
    {
        Expr exp;

        public InaccPattern(Expr _exp)
        {
            exp = _exp;
        }

        public String ToString()
        {
            return "Inaccessible Pattern : " + exp.ToString();
        }


    }

}