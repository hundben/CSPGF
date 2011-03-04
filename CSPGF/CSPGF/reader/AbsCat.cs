using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class AbsCat
    {

        private String name;
        private Hypo[] hypos;
        private WeightedIdent[] functions;

        public AbsCat(String _name, Hypo[] _hypos, WeightedIdent[] _functions)
        {
            name = _name;
            hypos = _hypos;
            functions = _functions;
        }

        public String GetName()
        {
            return name;
        }

        public Hypo[] GetHypos()
        {
            return hypos;
        }

        public WeightedIdent[] GetFunctions()
        {
            return functions;
        }

        public String ToString()
        {
            String ss = "Name : " + name + " , Hypotheses : (";
            foreach (Hypo h in hypos) {
                ss += " " + h.toString();
            }
            ss += ") , String Names : (";
            foreach (WeightedIdent w in functions) {
                ss += " " + w.toString();
            }
            ss += ")";
            return ss;
        }
    }
}