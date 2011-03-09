using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class VarAsPattern : Pattern
    {
        String name;
        Pattern patt;

        public VarAsPattern(String _name, Pattern _patt)
        {
            name = _name;
            patt = _patt;
        }

        public String ToString()
        {
            return "Variable as Pattern : [ Variable Name : " + name + " , Pattern : " + patt + "]";
        }

    }
}