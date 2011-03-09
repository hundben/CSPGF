using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class IntLit : RLiteral
    {
        public int value { get; private set; }

        public IntLit(int _value)
        {
            value = _value;
        }

        public String ToString()
        {
            return "Integer Literal : " + value;
        }
    }
}