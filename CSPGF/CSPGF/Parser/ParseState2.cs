using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSPGF.Grammar;

namespace CSPGF.Parse
{
    class ParseState2
    {
        private Concrete concrete;
        private CncCat startCat;
        private Trie items;
        private Chart2 chart;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="concrete"></param>
        /// <param name="startCat"></param>
		public ParseState2(Concrete concrete, CncCat startCat)
        {

            this.concrete = concrete;
			this.startCat = startCat;

            this.items = new Trie();
            this.chart = new Chart2(concrete);

            CncCat tempcat = concrete.GetStartCat();
            for(int fid = tempcat.FirstFID; fid <= tempcat.LastFID; fid++)
            {
                var prods = this.chart.expandForest(fid);
                foreach(ProductionApply k in prods)
                {
                    int lbl = 0;
                    foreach(Symbol[] sym in k.Function.Sequences)
                    {
                        var activeItem = new ActiveItem2(0, 0, k.Function, sym.ToList<Symbol>(), k.Domain().ToList<int>(), fid, lbl);
                        items.value.Add(activeItem);
                        lbl++;
                    }
                }
            }

            this.items.insertChain(new List<string>(), items);
        }

        public void next(string token)
        {
            var acc = this.items.lookup(token);
            if (acc.isEmpty())
            {
                acc = new Trie();
            }


            // TODO
        }

        public void complete(string currentToken)
        {
            // TODO
        }

        public void extractTrees()
        {
            // TODO
        }

        public void process(List<ActiveItem2> agenda)
        {
            while (agenda.Count > 0)
            {
                var item = agenda[0];
                agenda.RemoveAt(0);
                var lin = item.seq;

                if (item.dot < lin.Count)
                {
                    var sym = lin[item.dot];
                    if (sym is SymbolCat)
                    {

                    }
                    else if (sym is SymbolLit)
                    {

                    }
                    else if (sym is SymbolKS)
                    {

                    }
                    else if (sym is SymbolKP)
                    {

                    }
                    else if (sym is SymbolVar)
                    {

                    }
                }
            }

        }
    }
}
