using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class AbsNameExp : Expr
    {
        String name;

        public AbsNameExp(String _name)
        {
            name = _name;
        }

        public String toString()
        {
            return "Abstract Name Expression : [Name : " + name + "]";
        }

        public String getName()
        {
            return name;
        }


    }
}