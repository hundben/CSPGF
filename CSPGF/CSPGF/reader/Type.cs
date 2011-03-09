using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class Type
    {
        public Hypo[] hypos { get; private set; }
        public String name { get; private set; }
        public Expr[] exprs { get; private set; }

        public Type(Hypo[] _hypos, String _str, Expr[] _exprs)
        {
            hypos = _hypos;
            name = _str;
            exprs = _exprs;
        }

        public String ToString()
        {
            String ss = "Hypotheses : (";
            foreach (Hypo h in hypos)
            {
                ss += " " + h.ToString();
            }
            ss += (") , Name : " + name + " , Expressions : (");
            foreach (Expr e in exprs)
            {
                ss += " " + e.ToString();
            }
            ss += ")";
            return ss;
        }
    }
}
