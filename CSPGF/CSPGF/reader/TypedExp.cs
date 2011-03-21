using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class TypedExp : Expr
    {
        public Expr exp { get; private set; }
        public Type type { get; private set; }

        public TypedExp(Expr _exp, Type _t)
        {
            exp = _exp;
            type = _t;
        }

        public override String ToString()
        {
            return "Typed Expression : [Expr : " + exp.ToString() + " , Type : " + type.ToString() + "]";
        }
    }
}
