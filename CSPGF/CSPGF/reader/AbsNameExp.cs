using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class AbsNameExp : Expr
    {
        public String name { get; private set; }

        public AbsNameExp(String _name)
        {
            name = _name;
        }

        public String ToString()
        {
            return "Abstract Name Expression : [Name : " + name + "]";
        }
    }
}