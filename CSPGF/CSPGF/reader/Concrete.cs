using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CSPGF.reader
{
    public class Concrete
    {

        private String name;
        private Dictionary<String, RLiteral> flags;
        private Sequence[] seqs;
        private CncFun[] cncFuns;
        private ProductionSet[] prods;
        private Dictionary<String, CncCat> cncCats;
        private int fId;
        private String startCat;

        public Concrete(String _name, Dictionary<String, RLiteral> _flags, Sequence[] _seqs, CncFun[] _cncFuns,
            ProductionSet[] _prods, Dictionary<String, CncCat> _cncCats, int _fId, String _defaultStartCat)
        {
            name = _name;
            flags = _flags;
            seqs = _seqs;
            cncFuns = _cncFuns;
            prods = _prods;
            cncCats = _cncCats;
            fId = _fId;
            startCat = _defaultStartCat;
        }

        public String getName()
        {
            return name;
        }

        public CncCat getConcreteCats(String absCat)
        {
            return cncCats[absCat];
        }

        public Sequence[] getSequences()
        {
            return seqs;
        }

        public CncFun[] getCncFuns()
        {
            return cncFuns;
        }

        public ProductionSet[] getProductionSet()
        {
            return prods;
        }

        public CncCat[] getCncCat()
        {
            //TODO Fixa koden så den blir snyggare?
            CncCat[] array = new CncCat[cncCats.Count];
            int i = 0;
            foreach (KeyValuePair<String, CncCat> c in cncCats)
            {
                array[i] = c.Value;
                i++;
            }
            return array;
        }

        public int getFId()
        {
            return fId;
        }

        public CncCat getStartCat()
        {
            CncCat cat = cncCats[startCat];
            if (cat == null)
                return new CncCat(startCat, 0, 0, null);
            else
                return cat;
        }

        public Production[] getProductions()
        {
            int size = 0;
            foreach (ProductionSet ps in prods)
            {
                size += ps.length();
            }
            Production[] tprods = new Production[size];
            int i = 0;
            foreach (ProductionSet ps in prods)
            {
                foreach (Production p in ps.productions())
                {
                    tprods[i] = p;
                    i++;
                }
            }
            return tprods;
        }

        public String toString()
        {
            return "concrete" + name;
        }

        public Dictionary<int, HashSet<Production>> getSetOfProductions()
        {
            Dictionary<int, HashSet<Production>> dict = new Dictionary<int, HashSet<Production>>();
            foreach (ProductionSet p in prods)
            {
                dict.Add(p.getId(), p.getSetOfProductions());
            }
            //for (int i = 0 ; i < prods.Length ; i++)
            //    hm.Add(prods[i].getId(), prods[i].getSetOfProductions());
            return dict;
        }
    }
}
