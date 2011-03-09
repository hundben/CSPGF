using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class LiteralExp : Expr
    {
        public RLiteral literal { get; private set; }

        public LiteralExp(RLiteral _literal)
        {
            literal = _literal;
        }

        public String ToString()
        {
            return "Literal Expression : " + literal.ToString();
        }
    }
}