using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class ProductionSet
    {
        public int id { get; private set; }
        public Production[] prods { get; private set; }

        public ProductionSet(int _id, Production[] _prods)
        {
            id = _id;
            prods = _prods;
        }

        public int Length()
        {
            return prods.Length;
        }

        public override String ToString()
        {
            String ss = "Id : " + id + " , Productions : [";
            foreach (Production p in prods)
            {
                ss += " " + p.ToString();
            }
            ss += "]";
            return ss;
        }

        public HashSet<Production> GetSetOfProductions()
        {
            HashSet<Production> hs = new HashSet<Production>();
            foreach (Production p in prods) {
                hs.Add(p);
            }
            return hs;
        }
    }
}
