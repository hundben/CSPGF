using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class AbsCat
    {

        public String name { get; private set; }
        public Hypo[] hypos { get; private set; }
        public WeightedIdent[] functions {get; private set; }

        public AbsCat(String _name, Hypo[] _hypos, WeightedIdent[] _functions)
        {
            name = _name;
            hypos = _hypos;
            functions = _functions;
        }

        public override String ToString()
        {
            String ss = "Name : " + name + " , Hypotheses : (";
            foreach (Hypo h in hypos) {
                ss += " " + h.ToString();
            }
            ss += ") , String Names : (";
            foreach (WeightedIdent w in functions) {
                ss += " " + w.ToString();
            }
            ss += ")";
            return ss;
        }
    }
}