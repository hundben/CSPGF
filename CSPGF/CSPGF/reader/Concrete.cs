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

namespace CSPGF.Reader
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Concrete
    {
        // TODO: check for errors
        private Dictionary<string, RLiteral> flags;
        private string startCat;
        // We are missing: printnames, lindefs, lexicon, pproductions, lproductions ? Are any of these needed?
        public Concrete(
            string name,
            Dictionary<string, RLiteral> flags,
            List<Sequence> seqs,
            List<CncFun> cncFuns,
            List<ProductionSet> prods,
            Dictionary<string, CncCat> cncCats,
            int fId,
            string defaultStartCat)
        {
            this.Name = name;
            this.flags = flags;
            this.Seqs = seqs;
            this.CncFuns = cncFuns;
            this.Prods = prods;
            this.CncCats = cncCats;
            this.FId = fId;
            this.startCat = defaultStartCat;
        }

        public string Name { get; private set; }

        public List<Sequence> Seqs { get; private set; }

        public List<CncFun> CncFuns { get; private set; }

        public List<ProductionSet> Prods { get; private set; }

        public Dictionary<string, CncCat> CncCats { get; private set; }

        public int FId { get; private set; }

        public List<CncCat> GetCncCats()
        {
            List<CncCat> tmp = new List<CncCat>();
            foreach (KeyValuePair<string, CncCat> c in this.CncCats) 
            {
                tmp.Add(c.Value);
            }

            return tmp;
        }

        public CncCat GetStartCat()
        {
            if (this.CncCats.ContainsKey(this.startCat))
            {
                return this.CncCats[this.startCat];
            }
            else
            {
                return new CncCat(this.startCat, 0, 0, new List<string>());
            }
        }

        public List<Production> GetProductions()
        {
            List<Production> tmp = new List<Production>();
            foreach (ProductionSet ps in this.Prods) 
            {
                foreach (Production p in ps.Prods) 
                {
                    tmp.Add(p);
                }
            }

            return tmp;
        }

        public override string ToString()
        {
            return "Concrete" + this.Name;
        }

        public Dictionary<int, HashSet<Production>> GetSetOfProductions()
        {
            Dictionary<int, HashSet<Production>> dict = new Dictionary<int, HashSet<Production>>();
            foreach (ProductionSet p in this.Prods) 
            {
                dict.Add(p.ID, p.GetSetOfProductions());
            }

            return dict;
        }
    }
}
