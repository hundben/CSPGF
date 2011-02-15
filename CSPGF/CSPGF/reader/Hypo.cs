using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class Hypo
    {
        private Boolean bind;
        private String str;
        private Type t;

        public Hypo(Boolean _bind, String _str, Type _t)
        {
            bind = _bind;
            str = _str;
            t = _t;
        }

        public String toString()
        {
            return "Bound Type : " + bind + " , Name : " + str + " , Type : (" + t + ")";
        }

        public Boolean getBind()
        {
            return bind;
        }
        public String getName()
        {
            return str;
        }
        public Type getType()
        {
            return t;
        }
    }
}