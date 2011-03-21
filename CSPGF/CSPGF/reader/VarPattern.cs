using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class VarPattern : Pattern
    {
        String name;

        public VarPattern(String _name)
        {
            name = _name;
        }

        public override String ToString()
        {
            return "Variable Pattern : " + name;
        }
    }
}