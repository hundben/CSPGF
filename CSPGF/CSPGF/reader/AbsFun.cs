using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class AbsFun
    {
        private String str;
        private Type t;
        private int arit;
        private Eq[] eqs;
        private double weight;

        public AbsFun(String _str, Type _t, int _arit, Eq[] _eqs, double _weight)
        {
            str = _str;
            t = _t;
            arit = _arit;
            eqs = _eqs;
            weight = _weight;
        }

        public String toString()
        {
            String sb = "<function name = " + str + " type = " + t + " arity = " + arit + " equations = [";
            foreach (Eq e in eqs)
            {
                sb += eqs + ", ";
            }
            sb += "] weight = " + weight + " > ";
            return sb;
        }

        public String getName()
        {
            return str;
        }
        public Type getType()
        {
            return t;
        }
        public int getArit()
        {
            return arit;
        }
        public Eq[] getEqs()
        {
            return eqs;
        }
    }
}
