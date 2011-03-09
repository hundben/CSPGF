using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace CSPGF.reader
{
    public class Concrete
    {
        //TODO: Kolla igenom så inget failat!
        public String name { get; private set; }
        private Dictionary<String, RLiteral> flags;
        public Sequence[] seqs { get; private set; }
        public CncFun[] cncFuns { get; private set; }
        public ProductionSet[] prods { get; private set; }
        public Dictionary<String, CncCat> cncCats { get; private set; }
        private int fId { get; private set; }
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

        //TODO: Check where its used
        //public CncCat GetConcreteCats(String absCat)
        //{
        //    return cncCats[absCat];
        //}

        public CncCat[] GetCncCat()
        {
            //TODO Fixa koden så den blir snyggare?
            CncCat[] array = new CncCat[cncCats.Count];
            int i = 0;
            foreach (KeyValuePair<String, CncCat> c in cncCats) {
                array[i] = c.Value;
                i++;
            }
            return array;
        }

        public CncCat GetStartCat()
        {
            CncCat cat = cncCats[startCat];
            if (cat == null)
                return new CncCat(startCat, 0, 0, null);
            else
                return cat;
        }

        public Production[] GetProductions()
        {
            int size = 0;
            foreach (ProductionSet ps in prods) {
                size += ps.Length();
            }
            Production[] tprods = new Production[size];
            int i = 0;
            foreach (ProductionSet ps in prods) {
                foreach (Production p in ps.prods) {
                    tprods[i] = p;
                    i++;
                }
            }
            return tprods;
        }

        public String ToString()
        {
            return "concrete" + name;
        }

        public Dictionary<int, HashSet<Production>> GetSetOfProductions()
        {
            Dictionary<int, HashSet<Production>> dict = new Dictionary<int, HashSet<Production>>();
            foreach (ProductionSet p in prods) {
                dict.Add(p.id, p.GetSetOfProductions());
            }
            return dict;
        }
    }
}
