/*
Copyright (c) 2011, Christian Ståhlfors (christian.stahlfors@gmail.com), Erik Bergström (erktheorc@gmail.com)
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
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
        public int fId { get; private set; }
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

        public override String ToString()
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
