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

        }
    }
}
