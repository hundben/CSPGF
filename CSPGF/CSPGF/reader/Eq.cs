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

        public override String ToString()
        {
            String ss = "Patterns : (";
            foreach (Pattern p in patts)
            {
                ss += " " + p.ToString();
            }
            ss += ") , Expression : " + expr.ToString();
            return ss;
        }
    }
}
