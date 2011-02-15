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

        public String getName()
        {
            return name;
        }

        public Hypo[] getHypos()
        {
            return hypos;
        }

        public WeightedIdent[] getFunctions()
        {
            return functions;
        }

        public String toString()
        {
            String ss = "Name : " + name + " , Hypotheses : (";
            foreach (Hypo h in hypos)
            {
                ss += " " + h.toString();
            }
            //for (int i = 0 ; i < hypos.Length ; i++)
            //{
            //    ss += (" " + hypos[i].toString());
            //}
            ss += ") , String Names : (";
            foreach (WeightedIdent w in functions)
            {
                ss += " " + w.toString();
            }
            /*for (int i = 0 ; i < functions.Length ; i++)
            {
                ss += (" " + functions[i].toString());
            }*/
            ss += ")";
            return ss;
        }
    }
}
