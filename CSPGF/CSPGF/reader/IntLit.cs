using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class IntLit : RLiteral
    {
        private int value;

        public IntLit(int _value)
        {
            value = _value;
        }

        public int getValue()
        {
            return value;
        }

        public String toString()
        {
            return "Integer Literal : " + value;
        }
    }
}