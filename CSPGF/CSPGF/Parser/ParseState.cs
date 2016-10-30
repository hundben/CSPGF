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

namespace CSPGF.Parse
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using CSPGF.Grammar;

    /// <summary>
    /// The ParseState class that takes care of most of the parsing.
    /// </summary>
    internal class ParseState
    {
        /// <summary>
        /// The concrete grammar we want to use.
        /// </summary>
        private Concrete concrete;

        /// <summary>
        /// The start category.
        /// </summary>
        private ConcreteCategory startCat;

        /// <summary>
        /// The current parse tree.
        /// </summary>
        private Trie items;

        /// <summary>
        /// The chart used.
        /// </summary>
        private Chart chart;

        /// <summary>
        /// The current token, used for the literals.
        /// </summary>
        private string currentToken;

        /// <summary>
        /// The current Tree.
        /// </summary>
        private Trie currentAcc;

        /// <summary>
        /// Initializes a new instance of the ParseState class.
        /// </summary>
        /// <param name="concrete">The concrete used to parse.</param>
        public ParseState(Concrete concrete)
        {
            this.concrete = concrete;
            this.startCat = concrete.GetStartCat();

            this.items = new Trie();
            this.chart = new Chart(concrete);

            ConcreteCategory tempcat = concrete.GetStartCat();
            for (int fid = tempcat.FirstFID; fid <= tempcat.LastFID; fid++)
            {
                var prods = this.chart.ExpandForest(fid);
                foreach (ProductionApply k in prods)
                {
                    k.Function.FixSymbols();
                    int lbl = 0;
                    foreach (Symbol[] sym in k.Function.Sequences)
                    {
                        var activeItem = new ActiveItem(0, 0, k.Function, sym.ToList<Symbol>(), k.Domain().ToList<int>(), fid, lbl);
                        this.items.Value.Add(activeItem);
                        lbl++;
                    }
                }
            }

            this.items.InsertChain(new List<string>(), this.items);
        }

        /// <summary>
        /// Parse the next token.
        /// </summary>
        /// <param name="token">The token we want to parse.</param>
        /// <returns>Returns true if parsed correctly.</returns>
        public bool Next(string token)
        { 
            // Temporary store token instead of callback
            this.currentToken = token;
            this.currentAcc = this.items.Lookup(token);

            this.Process(this.items.Value);

            this.items = this.currentAcc;
            this.chart.Shift();

            return !this.items.IsEmpty();
        }
        
        /// <summary>
        /// Returns a list of predictions
        /// </summary>
        /// <returns>A list of predictions.</returns>
        public List<string> Predict()
        {
            // TODO check if correct
            return this.items.Predict(this.chart);
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
        /// <returns>Returns true if token was removed or false if no token was removed.</returns>
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
            this.Process(this.items.Value);

            TreeBuilder tb = new TreeBuilder();
            TreeConverter tc = new TreeConverter();
            List<Trees.Absyn.Tree> trees = new List<Trees.Absyn.Tree>();
            foreach (Tree t in tb.BuildTrees(this.chart, this.startCat))
            {
                trees.Add(tc.Intermediate2Abstract(t));
            }

            return trees;
        }

        /// <summary>
        /// Processes the current agenda.
        /// </summary>
        /// <param name="agenda">The agenda we want to go through.</param>
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
                        var items = this.chart.LookupAC(fid, label);
                        
                        if (items == null)
                        {
                            var rules = this.chart.ExpandForest(fid);
                            
                            foreach (ProductionApply rule in rules)
                            {
                                var newAI = new ActiveItem(this.chart.Offset, 0, rule.Function, rule.Function.Sequences[label].ToList<Symbol>(), rule.Domain().ToList<int>(), fid, label);
                                agenda.Add(newAI);
                            }

                            List<ActiveItem> temp = new List<ActiveItem>();
                            temp.Add(item);
                            this.chart.InsertAC(fid, label, temp);
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

                                var fid2 = this.chart.LookupPC(fid, label, this.chart.Offset);
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
                        if (this.chart.Forest.ContainsKey(fid))
                        {
                            rules = this.chart.Forest[fid];
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
                                List<string> tokens = new List<string>(pc.Tokens);
                                ActiveItem ai2 = item.ShiftOverTokn();
                                if (pc.Tokens.Count > 0 && (this.currentToken == string.Empty || tokens[0] == this.currentToken))
                                {
                                    tokens.RemoveAt(0);
                                    Trie tt = new Trie();
                                    tt.Value = new List<ActiveItem>() { ai2 };
                                    this.currentAcc = this.currentAcc.InsertChain1(tokens, tt);
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
                                newProd.Add(new ProductionConst(newFun, new List<string>() { token }, -1));    // nextId´+??
                            }
                            else if (fid == -2)
                            {
                                // If int
                                int i = 0;
                                if (int.TryParse(this.currentToken, out i))
                                {
                                    ConcreteFunction newFun = new ConcreteFunction(this.currentToken, syms);
                                    newProd.Add(new ProductionConst(newFun, new List<string>() { this.currentToken }, -2));
                                }
                            }
                            else if (fid == -3)
                            {
                                // If float
                                double f = 0;
                                if (double.TryParse(this.currentToken, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out f))
                                {
                                    ConcreteFunction newFun = new ConcreteFunction(this.currentToken, syms);
                                    newProd.Add(new ProductionConst(newFun, new List<string>() { this.currentToken }, -3));
                                }
                            }

                            if (newProd.Count > 0)
                            {
                                var currentProd = (ProductionConst)newProd[0];
                                fid = this.chart.NextId++;
                                this.chart.Forest[fid] = newProd;

                                var tokens2 = new List<string>(currentProd.Tokens); 
                                var item2 = item.ShiftOverArg(newSym.Arg, fid);

                                if (tokens2.Count > 0 && (this.currentToken == string.Empty || tokens2[0] == this.currentToken))
                                {
                                    tokens2.RemoveAt(0);
                                    Trie tt = new Trie();
                                    tt.Value = new List<ActiveItem>() { item2 };
                                    this.currentAcc = this.currentAcc.InsertChain1(tokens2, tt);
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
                            tt.Value = new List<ActiveItem>() { ai };
                            this.currentAcc = this.currentAcc.InsertChain1(tokens, tt);
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
                            tt.Value = new List<ActiveItem>() { pitem };
                            this.currentAcc = this.currentAcc.InsertChain1(tokens, tt);
                        }

                        foreach (Alternative alt in newSym.Alts)
                        {
                            tokens = new List<string>(alt.Alt1.ToList<string>());
                            if (tokens.Count > 0 && (this.currentToken == string.Empty || tokens[0] == this.currentToken))
                            {
                                tokens.RemoveAt(0);
                                Trie tt = new Trie();
                                tt.Value = new List<ActiveItem>() { pitem };
                                this.currentAcc = this.currentAcc.InsertChain1(tokens, tt);
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
                    int? tempfid = this.chart.LookupPC(item.FId, item.Lbl, item.Offset);
                    if (!tempfid.HasValue)
                    {
                        int fid = this.chart.NextId++;

                        var items = this.chart.LookupACo(item.Offset, item.FId, item.Lbl);
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

                        this.chart.InsertPC(item.FId, item.Lbl, item.Offset, fid);
                        var newProd = new ProductionApply(item.Fun, item.Args.ToArray<int>());
                        if (this.chart.Forest.ContainsKey(fid))
                        {
                            this.chart.Forest[fid].Add(newProd);
                        }
                        else
                        {
                            this.chart.Forest[fid] = new List<Production>() { newProd };
                        }
                    }
                    else
                    {
                        int fid = tempfid.Value;
                        var labels = this.chart.LabelsAC(fid);
                        foreach (int k in labels.Keys)
                        {
                            var newAI = new ActiveItem(this.chart.Offset, 0, item.Fun, item.Fun.Sequences[k].ToList<Symbol>(), item.Args, fid, k);
                            agenda.Add(newAI);
                        }

                        var rules = this.chart.Forest[fid];
                        var rule = new ProductionApply(item.Fun, item.Args.ToArray<int>());
                        bool isMember = false;
                        foreach (Production p in rules)
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
