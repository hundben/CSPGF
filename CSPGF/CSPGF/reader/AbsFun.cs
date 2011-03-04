using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class AbsFun
    {
        private String str;
        private Type type;
        private int arit;
        private Eq[] eqs;
        private double weight;

        public AbsFun(String _str, Type _type, int _arit, Eq[] _eqs, double _weight)
        {
            str = _str;
            type = _type;
            arit = _arit;
            eqs = _eqs;
            weight = _weight;
        }

        public String ToString()
        {
            String sb = "<function name = " + str + " type = " + type + " arity = " + arit + " equations = [";
            foreach (Eq e in eqs) {
                sb += eqs + ", ";
            }
            sb += "] weight = " + weight + " > ";
            return sb;
        }

        public String GetName()
        {
            return str;
        }
        public Type GetType()
        {
            return type;
        }
        public int GetArit()
        {
            return arit;
        }
        public Eq[] GetEqs()
        {
            return eqs;
        }
    }
}
