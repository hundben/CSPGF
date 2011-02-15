using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class Type
    {
        private Hypo[] hypos;
        private String str;
        private Expr[] exprs;

        public Type(Hypo[] _hypos, String _str, Expr[] _exprs)
        {
            hypos = _hypos;
            str = _str;
            exprs = _exprs;
        }

        public String toString()
        {
            String ss = "Hypotheses : (";
            for (int i = 0 ; i < hypos.Length ; i++)
            {
                ss += (" " + hypos[i].toString());
            }
            ss += (") , Name : " + str + " , Expressions : (");
            for (int i = 0 ; i < exprs.Length ; i++)
            {
                ss += (" " + exprs[i].toString());
            }
            ss += ")";
            return ss;
        }

        public Hypo[] getHypos()
        {
            return hypos;
        }
        public String getName()
        {
            return str;
        }
        public Expr[] getExprs()
        {
            return exprs;
        }
    }
}
