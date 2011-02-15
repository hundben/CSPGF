using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class MetaExp : Expr
    {
        private int id;

        public MetaExp(int _id)
        {
            id = _id;
        }

        public String toString()
        {
            return "Meta Expression : [Id : " + id + "]";
        }

        public int getMeta()
        {
            return id;
        }
    }
}