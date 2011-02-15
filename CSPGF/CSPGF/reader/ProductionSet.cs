using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.reader
{
    public class ProductionSet
    {
        private int id;
        private Production[] prods;

        public ProductionSet(int _id, Production[] _prods)
        {
            id = _id;
            prods = _prods;
        }

        public int length()
        {
            return prods.Length;
        }

        public Production[] productions()
        {
            return prods;
        }

        public String toString()
        {
            String ss = "Id : " + id + " , Productions : [";
            foreach (Production p in prods)
            {
                ss += " " + p.toString();
            }
            /*for (int i = 0 ; i < prods.Length ; i++)
            {
                ss += (" " + prods[i].toString());
            }*/
            ss += "]";
            return ss;
        }

        public int getId()
        {
            return id;
        }
        public Production[] getProductions()
        {
            return prods;
        }

        public HashSet<Production> getSetOfProductions()
        {
            HashSet<Production> hs = new HashSet<Production>();
            foreach (Production p in prods)
            {
                hs.Add(p);
            }
            /*for (int i = 0 ; i < prods.Length ; i++)
            {
                hs.Add(prods[i]);
            }*/
            return hs;
        }
    }
}
