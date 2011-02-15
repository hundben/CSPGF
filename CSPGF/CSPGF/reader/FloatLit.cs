using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class FloatLit : RLiteral
    {
        private double value;

        public FloatLit(double _value)
        {
            value = _value;
        }

        public String toString()
        {
            return "Float literal : " + value;
        }

        public double getValue()
        {
            return value;
        }

    }
}
