using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class Eq
    {
        Pattern[] patts;
        Expr expr;

        public Eq(Pattern[] _patts, Expr _expr)
        {
            patts = _patts;
            expr = _expr;
        }

        public String toString()
        {
            String ss = "Patterns : (";
            foreach (Pattern p in patts)
            {
                ss += " " + p.toString();
            }
            //for (int i = 0 ; i < patts.Length ; i++)
            //    ss += (" " + patts[i].toString());
            ss += ") , Expression : " + expr.toString();
            return ss;
        }
    }
}
