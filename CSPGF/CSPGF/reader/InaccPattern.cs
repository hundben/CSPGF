using System;

namespace CSPGF.reader
{
    public class InaccPattern : Pattern
    {
        Expr exp;

        public InaccPattern(Expr _exp)
        {
            exp = _exp;
        }

        public override String ToString()
        {
            return "Inaccessible Pattern : " + exp.ToString();
        }
    }
}