using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class StringLit : RLiteral
    {
        public String value { get; private set; }

        public StringLit(String _value)
        {
            value = _value;
        }

        public String ToString()
        {
            return "String literal : " + value;
        }
    }
}
