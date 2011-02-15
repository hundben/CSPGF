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
        private Type type;

        public Hypo(Boolean _bind, String _str, Type _type)
        {
            bind = _bind;
            str = _str;
            type = _type;
        }

        public String toString()
        {
            return "Bound Type : " + bind + " , Name : " + str + " , Type : (" + type + ")";
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
            return type;
        }
    }
}