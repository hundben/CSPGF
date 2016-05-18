//-----------------------------------------------------------------------
// <copyright file="ParseState.cs" company="None">
//  Copyright (c) 2011-2016, Christian Ståhlfors (christian.stahlfors@gmail.com), 
//   Erik Bergström (erktheorc@gmail.com) 
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//   * Redistributions of source code must retain the above copyright
//     notice, this list of conditions and the following disclaimer.
//   * Redistributions in binary form must reproduce the above copyright
//     notice, this list of conditions and the following disclaimer in the
//     documentation and/or other materials provided with the distribution.
//   * Neither the name of the &lt;organization&gt; nor the
//     names of its contributors may be used to endorse or promote products
//     derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &quot;AS IS&quot; AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL &lt;COPYRIGHT HOLDER&gt; BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CSPGF.Grammar;
using System.Globalization;

namespace CSPGF.Parse
{
    class ParseState
    {
        private Concrete concrete;
        private ConcreteCategory startCat;
        private Trie items;
        private Chart chart;
        private string currentToken;
        private Trie currentAcc;
        /// <summary>
        /// Creates a new instance of ParseState2
        /// </summary>
        /// <param name="concrete"></param>
        /// <param name="startCat"></param>
		public ParseState(Concrete concrete)
        {

            this.concrete = concrete;
			this.startCat = concrete.GetStartCat();

            this.items = new Trie();
            this.chart = new Chart(concrete);

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
                        var activeItem = new ActiveItem(0, 0, k.Function, sym.ToList<Symbol>(), k.Domain().ToList<int>(), fid, lbl);
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
        public bool Next(string token)
        { 
            // Temporary store token instead of callback
            this.currentToken = token;
            this.currentAcc = this.items.lookup(token) ;

            Process(this.items.value);

            this.items = currentAcc;
            this.chart.shift();

            return !this.items.isEmpty();

        }
        
        /// <summary>
        /// Returns a list of predictions
        /// </summary>
        /// <returns>A list of predictions.</returns>
        public List<string> Predict()
        {
            // TODO check if correct
            return this.items.Predict();
        }

        /// <summary>
        /// Resets the parser but keeps grammar.
        /// </summary>
        public void Reset()
        {
            // TODO implement
        }

        /// <summary>
        /// Removes one token if the recovery-parser is used.
        /// </summary>
        /// <returns></returns>
        public bool RemoveToken()
        {
            // TODO implement
            return false;
        }

        /// <summary>
        /// Get the trees.
        /// </summary>
        /// <returns>A list of the trees.</returns>
        public List<Trees.Absyn.Tree> GetTrees()
        {
            this.currentToken = string.Empty;
            this.Process(this.items.value);

            TreeBuilder tb = new TreeBuilder();
            TreeConverter tc = new TreeConverter();
            List<Trees.Absyn.Tree> trees = new List<Trees.Absyn.Tree>();
            foreach (Tree t in tb.BuildTrees(this.chart, this.startCat))
            {
                trees.Add(tc.Intermediate2Abstract(t));
            }

            return trees;
        }

        public void Process(List<ActiveItem> agenda)
        {
            while (agenda.Count > 0)
            {
                var item = agenda[agenda.Count - 1];
                agenda.RemoveAt(agenda.Count - 1);
                var lin = item.Seq;

                if (item.Dot < lin.Count)
                {
                    var sym = lin[item.Dot];
                    if (sym is SymbolCat)
                    {
                        var newSym = (SymbolCat)sym;
                        var fid = item.Args[newSym.Arg];
                        var label = newSym.Label;
                        var items = this.chart.lookupAC(fid, label);
                        
                        if (items == null)
                        {
                            var rules = this.chart.expandForest(fid);
                            
                            foreach(ProductionApply rule in rules)
                            {
                                var newAI = new ActiveItem(this.chart.offset, 0, rule.Function, rule.Function.Sequences[label].ToList<Symbol>(), rule.Domain().ToList<int>(), fid, label);
                                agenda.Add(newAI);
                            }

                            List<ActiveItem> temp = new List<ActiveItem>();
                            temp.Add(item);
                            this.chart.insertAC(fid, label, temp);
                        }
                        else
                        {
                            bool isMember = false;
                            foreach (ActiveItem ai in items)
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
                                    agenda.Add(item.ShiftOverArg(newSym.Arg, fid2.Value));
                                }
                            }
                        }
                    }
                    else if (sym is SymbolLit)
                    {
                        var newSym = (SymbolLit)sym;
                        var fid = item.Args[newSym.Arg];

                        List<Production> rules;
                        if (this.chart.forest.ContainsKey(fid))
                        {
                            rules = this.chart.forest[fid];
                        }
                        else
                        {
                            rules = new List<Production>();
                        }

                        if (rules.Count > 0)
                        {
                            if (rules[0] is ProductionConst)
                            {
                                ProductionConst pc = (ProductionConst)rules[0];
                                List<string> tokens = new List<string>(pc.tokens);
                                ActiveItem ai2 = item.ShiftOverTokn();
                                if (pc.tokens.Count > 0 && (currentToken == string.Empty || tokens[0] == currentToken))
                                {
                                    tokens.RemoveAt(0);
                                    Trie tt = new Trie();
                                    tt.value = new List<ActiveItem>() { ai2 };
                                    this.currentAcc = this.currentAcc.insertChain1(tokens, tt);
                                }
                            }
                        }
                        else
                        {
                            List<Production> newProd = new List<Production>();
                            
                            Symbol[][] syms = new Symbol[0][];
                            if (fid == -1)
                            {
                                // If string
                                string token = "\"" + this.currentToken + "\"";                              
                                ConcreteFunction newFun = new ConcreteFunction(token, syms);
                                newProd.Add(new ProductionConst(this.chart.nextId, newFun, new List<string>() { token }, -1));    // nextId´+??
                            }
                            else if (fid == -2)
                            {
                                // If int
                                int i = 0;
                                if (int.TryParse(this.currentToken, out i))
                                {
                                    ConcreteFunction newFun = new ConcreteFunction(this.currentToken, syms);
                                    newProd.Add(new ProductionConst(this.chart.nextId, newFun, new List<string>() { this.currentToken }, -2));
                                }
                            }
                            else if (fid == -3)
                            {
                                // If float
                                float f = 0;
                                if (float.TryParse(this.currentToken, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out f))
                                {
                                    ConcreteFunction newFun = new ConcreteFunction(this.currentToken, syms);
                                    newProd.Add( new ProductionConst(this.chart.nextId, newFun, new List<string>() { this.currentToken }, -3));
                                }
                            }

                            if (newProd.Count > 0)
                            {
                                var currentProd = (ProductionConst)newProd[0];
                                fid = this.chart.nextId++;
                                this.chart.forest[fid] = newProd;

                                var tokens2 = new List<string>(currentProd.tokens); 
                                var item2 = item.ShiftOverArg(newSym.Arg, fid);

                                if (tokens2.Count > 0 && (this.currentToken == string.Empty || tokens2[0] == this.currentToken))
                                {
                                    tokens2.RemoveAt(0);
                                    Trie tt = new Trie();
                                    tt.value = new List<ActiveItem>() { item2 };
                                    this.currentAcc = this.currentAcc.insertChain1(tokens2, tt);
                                }
                            }
                        }
                    }
                    else if (sym is SymbolKS)
                    {
                        var newSym = (SymbolKS)sym;
                        var tokens = new List<string>(newSym.Tokens.ToList<string>());
                        var ai = item.ShiftOverTokn();
                        if (tokens.Count > 0 && (this.currentToken == string.Empty || tokens[0] == this.currentToken))
                        {
                            tokens.RemoveAt(0);
                            Trie tt = new Trie();
                            tt.value = new List<ActiveItem>() { ai };
                            this.currentAcc = this.currentAcc.insertChain1(tokens, tt);
                        }
                    }
                    else if (sym is SymbolKP)
                    {
                        var newSym = (SymbolKP)sym;
                        var pitem = item.ShiftOverTokn();
                        var tokens = newSym.Tokens.ToList<string>();
                        if (tokens.Count > 0 && (this.currentToken == string.Empty || tokens[0] == this.currentToken))
                        {
                            tokens.RemoveAt(0);
                            Trie tt = new Trie();
                            tt.value = new List<ActiveItem>() { pitem };
                            this.currentAcc = this.currentAcc.insertChain1(tokens, tt);
                        }

                        foreach ( Alternative alt in newSym.Alts)
                        {
                            tokens = new List<string>(alt.Alt1.ToList<string>());
                            if (tokens.Count > 0 && (this.currentToken == string.Empty || tokens[0] == this.currentToken))
                            {
                                tokens.RemoveAt(0);
                                Trie tt = new Trie();
                                tt.value = new List<ActiveItem>() { pitem };
                                this.currentAcc = this.currentAcc.insertChain1(tokens, tt);
                            }
                        }
                    }
                    else if (sym is SymbolVar)
                    {
                        var newSym = (SymbolVar)sym;
                        // TODO Not implemented
                        throw new NotImplementedException();
                    }
                }
                else
                {
                    int? tempfid = this.chart.lookupPC(item.FId, item.Lbl, item.Offset);
                    if (!tempfid.HasValue)
                    {
                        int fid = this.chart.nextId++;

                        var items = this.chart.lookupACo(item.Offset, item.FId, item.Lbl);
                        if (items != null)
                        {
                            foreach (ActiveItem pitem in items)
                            {
                                var temp = pitem.Seq[pitem.Dot];
                                if (temp is SymbolCat)
                                {
                                    var arg = ((SymbolCat)temp).Arg;
                                    agenda.Add(pitem.ShiftOverArg(arg, fid));

                                }
                                else if (temp is SymbolLit)
                                {
                                    var arg = ((SymbolLit)temp).Arg;
                                    agenda.Add(pitem.ShiftOverArg(arg, fid));
                                }
                            }
                        }

                        this.chart.insertPC(item.FId, item.Lbl, item.Offset, fid);
                        var newProd = new ProductionApply(this.chart.nextId, item.Fun, item.Args.ToArray<int>());
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
                            var newAI = new ActiveItem(this.chart.offset, 0, item.Fun, item.Fun.Sequences[k].ToList<Symbol>(), item.Args, fid, k);
                            agenda.Add(newAI);
                        }

                        var rules = this.chart.forest[fid];
                        var rule = new ProductionApply(this.chart.nextId, item.Fun, item.Args.ToArray<int>());
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
