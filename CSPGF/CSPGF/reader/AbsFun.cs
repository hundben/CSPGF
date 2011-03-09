using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class AbsFun
    {
        public String name { get; private set; }
        public Type type { get; private set; }
        public int arit { get; private set; }
        public Eq[] eqs { get; private set; }
        private double weight;

        public AbsFun(String _str, Type _type, int _arit, Eq[] _eqs, double _weight)
        {
            name = _str;
            type = _type;
            arit = _arit;
            eqs = _eqs;
            weight = _weight;
        }

        public String ToString()
        {
            String sb = "<function name = " + name + " type = " + type + " arity = " + arit + " equations = [";
            foreach (Eq e in eqs) {
                sb += eqs + ", ";
            }
            sb += "] weight = " + weight + " > ";
            return sb;
        }
    }
}
