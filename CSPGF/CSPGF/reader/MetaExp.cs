using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class MetaExp : Expr
    {
        public int meta { get; private set; }

        public MetaExp(int _id)
        {
            meta = _id;
        }

        public String ToString()
        {
            return "Meta Expression : [Id : " + meta + "]";
        }
    }
}