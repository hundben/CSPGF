using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSPGF.Grammar;
using System.Globalization;

namespace CSPGF.Parse
{
    class ParseState2
    {
        private Concrete concrete;
        private ConcreteCategory startCat;
        private Trie items;
        private Chart2 chart;
        private string currentToken;
        private Trie currentAcc;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="concrete"></param>
        /// <param name="startCat"></param>
		public ParseState2(Concrete concrete, ConcreteCategory startCat)
        {

            this.concrete = concrete;
			this.startCat = startCat;

            this.items = new Trie();
            this.chart = new Chart2(concrete);

            ConcreteCategory tempcat = concrete.GetStartCat();
            for(int fid = tempcat.FirstFID; fid <= tempcat.LastFID; fid++)
            {
                var prods = this.chart.expandForest(fid);
                foreach(ProductionApply k in prods)
                {
                    k.Function.FixSymbols();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool next(string token)
        { 
            // Temporary store token instead of callback
            this.currentToken = token;
            this.currentAcc = this.items.lookup(token) ;

            process(this.items.value);

            this.items = currentAcc;
            this.chart.shift();

            return !this.items.isEmpty();

        }

        /// <summary>
        /// TODO not used... so remove
        /// </summary>
        /// <param name="currentToken"></param>
        public void complete(string currentToken)
        {
            // TODO
        }

        /// <summary>
        /// Get the trees.
        /// </summary>
        /// <returns>A list of the trees.</returns>
        public List<Trees.Absyn.Tree> GetTrees()
        {
            this.currentToken = string.Empty;
            this.process(this.items.value);

            TreeBuilder2 tb = new TreeBuilder2();
            TreeConverter tc = new TreeConverter();
            List<Trees.Absyn.Tree> trees = new List<Trees.Absyn.Tree>();
            foreach (Tree t in tb.BuildTrees(this.chart, this.startCat))
            {
                trees.Add(tc.Intermediate2Abstract(t));
            }

            return trees;
        }

        public void process(List<ActiveItem2> agenda)
        {
            while (agenda.Count > 0)
            {
                var item = agenda[agenda.Count - 1];
                agenda.RemoveAt(agenda.Count - 1);
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

                        List<Production> rules;
                        if (this.chart.forest.ContainsKey(fid))
                        {
                            rules = this.chart.forest[fid];
                        }
                        else
                        {
                            rules = new List<Production>();
                        }

                        //var rules = this.chart.forest[fid]; // TODO can return 0


                        if (rules.Count > 0)
                        {
                            if (rules[0] is ProductionConst)
                            {
                                ProductionConst pc = (ProductionConst)rules[0];
                                List<string> tokens = pc.tokens;
                                ActiveItem2 ai2 = item.shiftOverTokn();
                                if (pc.tokens.Count > 0 && (currentToken == string.Empty || tokens[0] == currentToken))
                                {
                                    tokens.RemoveAt(0);
                                    Trie tt = new Trie();
                                    tt.value = new List<ActiveItem2>() { ai2 };
                                    this.currentAcc = this.currentAcc.insertChain1(tokens, tt);
                                }
                            }
                        }
                        else
                        {
                            List<Production> newProd = new List<Production>();
                            
                            Symbol[][] syms = new Symbol[0][];  // TODO check if this is correct?
                            List<string> tokens = new List<string>();
                            if (fid == -1)
                            {
                                // If string
                                string token = "\"" + this.currentToken + "\"";                              
                                tokens.Add(token);
                                ConcreteFunction newFun = new ConcreteFunction(token, syms);
                                newProd.Add(new ProductionConst(this.chart.nextId, newFun, tokens));    // nextId´+??
                            }
                            else if (fid == -2)
                            {
                                // If int
                                int i = 0;
                                if (int.TryParse(this.currentToken, out i))
                                {
                                    tokens.Add(this.currentToken);
                                    ConcreteFunction newFun = new ConcreteFunction(this.currentToken, syms);
                                    newProd.Add(new ProductionConst(this.chart.nextId, newFun, tokens));
                                }
                            }
                            else if (fid == -3)
                            {
                                // If float
                                float f = 0;
                                if (float.TryParse(this.currentToken, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out f))
                                {
                                    tokens.Add(this.currentToken);
                                    ConcreteFunction newFun = new ConcreteFunction(this.currentToken, syms);
                                    newProd.Add( new ProductionConst(this.chart.nextId, newFun, tokens));
                                }
                            }

                            if (newProd.Count > 0)
                            {
                                var currentProd = (ProductionConst)newProd[0];
                                fid = this.chart.nextId++;
                                this.chart.forest[fid] = newProd;

                                // notice, replaces earlier tokens but is the same... TODO maybe remove this as it's not nessecary
                                var tokens2 = currentProd.tokens;
                                var item2 = item.shiftOverArg(newSym.Arg, fid);

                                if (tokens2.Count > 0 && (this.currentToken == string.Empty || tokens2[0] == this.currentToken))
                                {
                                    // TODO remove first and keep the rest
                                    tokens2.RemoveAt(0);
                                    Trie tt = new Trie();
                                    tt.value = new List<ActiveItem2>() { item2 };
                                    this.currentAcc = this.currentAcc.insertChain1(tokens2, tt);
                                }
                            }
                        }
                    }
                    else if (sym is SymbolKS)
                    {
                        var newSym = (SymbolKS)sym;
                        var tokens = newSym.Tokens.ToList<string>();
                        var ai = item.shiftOverTokn();
                        if (tokens.Count > 1 && ai.dot > 0)
                        {
                            //ai.dot--;
                        }
                        if (tokens.Count > 0 && (this.currentToken == string.Empty || tokens[0] == this.currentToken))
                        {
                            tokens.RemoveAt(0);
                            Trie tt = new Trie();
                            tt.value = new List<ActiveItem2>() { ai };
                            this.currentAcc = this.currentAcc.insertChain1(tokens, tt);
                        }
                    }
                    else if (sym is SymbolKP)
                    {
                        var newSym = (SymbolKP)sym;
                        var pitem = item.shiftOverTokn();
                        var tokens = newSym.Tokens.ToList<string>();
                        if (tokens.Count > 0 && (this.currentToken == string.Empty || tokens[0] == this.currentToken))
                        {
                            tokens.RemoveAt(0);
                            Trie tt = new Trie();
                            tt.value = new List<ActiveItem2>() { pitem };
                            this.currentAcc = this.currentAcc.insertChain1(tokens, tt);
                        }

                        foreach ( Alternative alt in newSym.Alts)
                        {
                            tokens = alt.Alt1.ToList<string>();
                            if (tokens.Count > 0 && (this.currentToken == string.Empty || tokens[0] == this.currentToken))
                            {
                                tokens.RemoveAt(0);
                                Trie tt = new Trie();
                                tt.value = new List<ActiveItem2>() { pitem };
                                this.currentAcc = this.currentAcc.insertChain1(tokens, tt);
                            }
                        }
                    }
                    else if (sym is SymbolVar)
                    {
                        var newSym = (SymbolVar)sym;
                        // TODO Not
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    int? tempfid = this.chart.lookupPC(item.fid, item.lbl, item.offset);
                    if (!tempfid.HasValue)
                    {
                        int fid = this.chart.nextId++;

                        // TODO fix this
                        var items = this.chart.lookupACo(item.offset, item.fid, item.lbl);
                        if (items != null)
                        {
                            foreach (ActiveItem2 pitem in items)
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
                        }

                        this.chart.insertPC(item.fid, item.lbl, item.offset, fid);
                        var newProd = new ProductionApply(this.chart.nextId, item.fun, item.args.ToArray<int>());   // fid++??? why fid here? TODO remove
                        if (this.chart.forest.ContainsKey(fid))
                        {
                            this.chart.forest[fid].Add(newProd);
                        }
                        else
                        {
                            this.chart.forest[fid] = new List<Production>() { newProd };
                        }
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
                        var rule = new ProductionApply(this.chart.nextId, item.fun, item.args.ToArray<int>());  // TODO remove fid
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
