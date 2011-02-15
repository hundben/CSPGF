using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class StringLit : RLiteral
    {
        String value;

        public StringLit(String _value)
        {
            value = _value;
        }

        public String toString()
        {
            String s = "String literal : " + value;
            return s;
        }

        public String getValue()
        {
            return value;
        }
    }
}
