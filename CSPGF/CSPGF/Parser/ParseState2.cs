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
                        var newSym = (SymbolCat)sym;
                        var fid = item.args[newSym.Arg];
                        var label = newSym.Label;

                        var items = this.chart.lookupAC(fid, label);
                        if (items.Count > 0)
                        {
                            var rules = this.chart.expandForest(fid);
                            foreach(ProductionApply rule in rules)
                            {
                                var newAI = new ActiveItem2(this.chart.offset, 0, rule.Function, rule.Function.Sequences[label].ToList<Symbol>(), rule.Domain().ToList<int>(), fid, label);
                                agenda.Add(newAI);
                            }

                            List<ActiveItem2> temp = new List<ActiveItem2>();
                            temp.Add(item);
                            this.chart.insertAC(fid, label, temp);
                        }
                        else
                        {
                            bool isMember = false;
                            foreach (ActiveItem2 ai in items)
                            {
                                if (ai.Equals(item))
                                {
                                    isMember = true;
                                    break;
                                }
                            }

                            if (!isMember)
                            {
                                items.Add(item);

                                var fid2 = this.chart.lookupPC(fid, label, this.chart.offset);
                                if (fid2.HasValue)
                                {
                                    agenda.Add(item.shiftOverArg(newSym.Arg, fid2.Value));
                                }
                            }
                        }
                    }
                    else if (sym is SymbolLit)
                    {
                        var newSym = (SymbolLit)sym;
                        var fid = item.args[newSym.Arg];
                        var rules = this.chart.forest[fid];
                        if (rules.Count > 0)
                        {
                            // TODO tokencallback
                        }
                        else
                        {
                            //var rule = literalCallback
                        }

                        // TODO
                    }
                    else if (sym is SymbolKS)
                    {
                        var newSym = (SymbolKS)sym;
                        // TODO
                    }
                    else if (sym is SymbolKP)
                    {
                        var newSym = (SymbolKP)sym;
                        // TODO
                    }
                    else if (sym is SymbolVar)
                    {
                        var newSym = (SymbolVar)sym;
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
