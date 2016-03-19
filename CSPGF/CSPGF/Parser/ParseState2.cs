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
                        // TODO
                    }
                    else if (sym is SymbolLit)
                    {
                        // TODO
                    }
                    else if (sym is SymbolKS)
                    {
                        // TODO
                    }
                    else if (sym is SymbolKP)
                    {
                        // TODO
                    }
                    else if (sym is SymbolVar)
                    {
                        // TODO
                    }
                }
                else
                {
                    int? tempfid = this.chart.lookupPC(item.fid, item.lbl, item.offset);
                    if (!tempfid.HasValue)
                    {
                        int fid = this.chart.nextId++;

                        var items = this.chart.lookupACo(item.offset, item.fid, item.lbl);
                        foreach(ActiveItem2 pitem in items)
                        {
                            var temp = pitem.seq[pitem.dot];
                            if (temp is SymbolCat)
                            {
                                var arg = ((SymbolCat)temp).Arg;
                                agenda.Add(pitem.shiftOverArg(arg, fid));

                            }
                            else if (temp is SymbolLit)
                            {
                                var arg = ((SymbolLit)temp).Arg;
                                agenda.Add(pitem.shiftOverArg(arg, fid));
                            }
                        }

                        this.chart.insertPC(item.fid, item.lbl, item.offset, fid);
                        var newProd = new ProductionApply(this.chart.nextId++, item.fun, item.args.ToArray<int>());
                        this.chart.forest[fid].Add(newProd);
                    }
                    else
                    {
                        int fid = tempfid.Value;
                        var labels = this.chart.labelsAC(fid);
                        foreach(int k in labels.Keys)
                        {
                            var newAI = new ActiveItem2(this.chart.offset, 0, item.fun, item.fun.Sequences[k].ToList<Symbol>(), item.args, fid, k);
                            agenda.Add(newAI);
                        }

                        var rules = this.chart.forest[fid];
                        var rule = new ProductionApply(this.chart.nextId++, item.fun, item.args.ToArray<int>());
                        bool isMember = false;
                        foreach(Production p in rules)
                        {
                            if (p is ProductionApply)
                            {
                                if (rule.Equals((ProductionApply)p))
                                {
                                    isMember = true;
                                }
                            }
                        }
                        
                        if (!isMember)
                        {
                            rules.Add(rule);
                        }
                    }
                }
            }

        }
    }
}
