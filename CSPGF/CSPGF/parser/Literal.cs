using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser
{
    class Literal : Tree
    {
        public String value;
        public Literal(String _value)
        {
            value = _value;
        }
    }
}
