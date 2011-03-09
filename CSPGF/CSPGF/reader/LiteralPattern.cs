using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class LiteralPattern : Pattern
    {
        public RLiteral value { get; private set; }

        public LiteralPattern(RLiteral _value)
        {
            value = _value;
        }

        public String ToString()
        {
            return "Literal Pattern : " + value.ToString();
        }
    }
}
