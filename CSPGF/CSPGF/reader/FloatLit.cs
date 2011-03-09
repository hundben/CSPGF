using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class FloatLit : RLiteral
    {
        public double value { get; private set; }

        public FloatLit(double _value)
        {
            value = _value;
        }

        public String ToString()
        {
            return "Float literal : " + value;
        }
    }
}
