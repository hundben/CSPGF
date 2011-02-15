using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class ImpArgPattern : Pattern
    {
        Pattern name;

        public ImpArgPattern(Pattern _name)
        {
            name = _name;
        }

        public String toString()
        {
            return "Implicit Argument Pattern : " + name.toString();
        }

    }

}