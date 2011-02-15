using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class LiteralExp : Expr
    {
        private RLiteral literal;

        public LiteralExp(RLiteral _literal)
        {
            literal = _literal;
        }

        public String toString()
        {
            return "Literal Expression : " + literal.toString();
        }

        public RLiteral getLiteral()
        {
            return literal;
        }
    }
}