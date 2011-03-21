using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class AppExp : Expr
    {
        public Expr lExp { get; private set; }
        public Expr rExp { get; private set; }

        public AppExp(Expr _lExp, Expr _rExp)
        {
            lExp = _lExp;
            rExp = _rExp;
        }

        public override String ToString()
        {
            return "Expression application [Left-hand side : ( " + lExp.ToString() + "), Right-hand side : (" + rExp.ToString() + ")]";
        }
    }
}