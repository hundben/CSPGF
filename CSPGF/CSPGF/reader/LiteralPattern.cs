using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class LiteralPattern : Pattern
    {
        private RLiteral value;

        public LiteralPattern(RLiteral _value)
        {
            value = _value;
        }

        public String toString()
        {
            return "Literal Pattern : " + value.toString();
        }

        public RLiteral getLit()
        {
            return value;
        }
    }
}
