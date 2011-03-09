using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class Hypo
    {
        public Boolean bind { get; private set; }
        public String name { get; private set; }
        public Type type { get; private set; }

        public Hypo(Boolean _bind, String _str, Type _type)
        {
            bind = _bind;
            name = _str;
            type = _type;
        }

        public String ToString()
        {
            return "Bound Type : " + bind + " , Name : " + name + " , Type : (" + type + ")";
        }
    }
}