//-----------------------------------------------------------------------
// <copyright file="ParseState.cs" company="None">
//  Copyright (c) 2011, Christian Ståhlfors (christian.stahlfors@gmail.com), 
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
    using System.Linq;
    using System.Text;
    using CSPGF.Reader;

    /// <summary>
    /// The parsestate class.
    /// </summary>
    [Serializable]
    public class ParseState
    {
        /// <summary>
        /// Start category
        /// </summary>
        private CncCat startCat;

        /// <summary>
        /// The parse tree.
        /// </summary>
        private ParseTrie trie;

        /// <summary>
        /// The chart.
        /// </summary>
        private Chart chart;

        /// <summary>
        /// A stack of active items.
        /// </summary>
        private Stack<ActiveItem> agenda;

        /// <summary>
        /// Current position.
        /// </summary>
        private int position;

        /// <summary>
        /// List with sets of acctive items.
        /// </summary>
        private List<Dictionary<int, Dictionary<int, HashSet<ActiveItem>>>> active;

        /// <summary>
        /// Initializes a new instance of the ParseState class.
        /// </summary>
        /// <param name="grammar">The concrete grammar we would like to use.</param>
        public ParseState(Concrete grammar)
        {
            this.startCat = grammar.GetStartCat();
            this.trie = new ParseTrie(null);

            this.chart = new Chart(grammar.FId + 1);
            this.agenda = new Stack<ActiveItem>();
            this.position = 0;
            this.active = new List<Dictionary<int, Dictionary<int, HashSet<ActiveItem>>>>();

            // initiate
            foreach (Production k in grammar.GetProductions()) 
            {
                this.chart.AddProduction(k);
            }

            for (int id = this.startCat.FirstFID; id <= this.startCat.LastFID + 1; id++) 
            {
                foreach (ApplProduction prod in this.chart.GetProductions(id)) 
                {
                    ActiveItem ai = new ActiveItem(0, id, prod.Function, prod.Domain(), 0, 0);
                    this.agenda.Push(ai);
                }
            }

            this.Compute();
        }

        /// <summary>
        /// Get the trees.
        /// </summary>
        /// <returns>A list of the trees.</returns>
        public List<CSPGF.Trees.Absyn.Tree> GetTrees()
        {
            TreeBuilder tb = new TreeBuilder();
            TreeConverter tc = new TreeConverter();
            List<CSPGF.Trees.Absyn.Tree> trees = new List<CSPGF.Trees.Absyn.Tree>();
            foreach (Tree t in tb.BuildTrees(this.chart, this.startCat, this.position)) 
            {
                trees.Add(tc.Intermediate2Abstract(t));
            }

            return trees;
        }

        /// <summary>
        /// Scans a new token.
        /// </summary>
        /// <param name="token">The token to scan.</param>
        /// <returns>Returns true if scan i complete.</returns>
        public bool Scan(string token)
        {
            ParseTrie newTrie = this.trie.GetSubTrie(token);
            if (newTrie != null) 
            {
                string[] empt = new string[0];
                Stack<ActiveItem> newAgenda = newTrie.Lookup(empt);
                if (newAgenda != null) 
                {
                    this.trie = newTrie;
                    this.position++;
                    this.agenda = newAgenda;
                    this.Compute();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Predicts the next token.
        /// </summary>
        /// <returns>A list of valid tokens.</returns>
        public List<string> Predict()
        {
            return this.trie.Predict();
        }

        /// <summary>
        /// Computes the new trees.
        /// </summary>
        private void Compute()
        {
            this.active.Add(new Dictionary<int, Dictionary<int, HashSet<ActiveItem>>>());

            // redo this with iterator or something like that?
            while (this.agenda.Count != 0) 
            {
                ActiveItem e = this.agenda.Pop();
                this.ProcessActiveItem(e);
            }
        }

        /// <summary>
        /// Processes an active item.
        /// </summary>
        /// <param name="item">The item to be processed.</param>
        private void ProcessActiveItem(ActiveItem item)
        {
            int j = item.Begin;
            int a = item.Category;
            CncFun f = item.Function;
            List<int> b = item.Domain;
            int l = item.Constituent;
            int p = item.Position;

            Symbol sym = item.CurrentSymbol();

            if (sym is ToksSymbol) 
            {
                ToksSymbol tok = (ToksSymbol)sym;
                List<string> tokens = tok.Tokens;
                ActiveItem i = new ActiveItem(j, a, f, b, l, p + 1);

                // scan
                Stack<ActiveItem> newAgenda;
                Stack<ActiveItem> tempAgenda = this.trie.Lookup(tokens);
                if (tempAgenda == null || tempAgenda.Count == 0) 
                {
                    Stack<ActiveItem> sai = new Stack<ActiveItem>();
                    this.trie.Add(tokens, sai);
                    newAgenda = sai;
                }
                else 
                {
                    newAgenda = tempAgenda;
                }

                newAgenda.Push(i);
            }
            else if (sym is ArgConstSymbol) 
            {
                ArgConstSymbol arg = (ArgConstSymbol)sym;
                int d = arg.Arg;
                int r = arg.Cons;
                int bd = item.Domain[d];

                // TODO check if this is correct
                if (this.active.Count >= this.position) 
                {
                    // a bit strange, check if we should create an active set first...
                    if (this.AddActiveSet(bd, r, item, this.active[this.position])) 
                    {
                        foreach (ApplProduction prod in this.chart.GetProductions(bd)) 
                        {
                            ActiveItem it = new ActiveItem(this.position, bd, prod.Function, prod.Domain(), r, 0);
                            this.agenda.Push(it);
                        }
                    }

                    int cat = this.chart.GetCategory(bd, r, this.position, this.position);
                    
                    if (cat != -1) 
                    {
                        List<int> newDomain = new List<int>(b);
                        newDomain[d] = cat;
                        ActiveItem it = new ActiveItem(j, a, f, newDomain, l, p + 1);
                        this.agenda.Push(it);
                    }
                }
            }
            else 
            {
                int cat = this.chart.GetCategory(a, l, j, this.position);
                if (cat == -1) 
                {
                    int n = this.chart.GenerateFreshCategory(new Category(a, l, j, this.position));
                    foreach (ActiveItem ai in this.GetActiveSet(a, this.active[j]))
                    {
                        ActiveItem ip = ai;
                        int d = ((ArgConstSymbol)ai.CurrentSymbol()).Arg;    // TODO Cons?
                        List<int> domain = new List<int>(ip.Domain);
                        domain[d] = n;
                        ActiveItem i = new ActiveItem(ip.Begin, ip.Category, ip.Function, domain, ip.Constituent, ip.Position + 1);
                        this.agenda.Push(i);
                    }

                    this.chart.AddProduction(n, f, b);
                }
                else 
                {
                    HashSet<ActiveItem> items = this.GetActiveSet(cat, this.active[this.position]);
                    foreach (ActiveItem ai in items) 
                    {
                        int r = ((ArgConstSymbol)ai.CurrentSymbol()).Arg;  // Cons?
                        ActiveItem i = new ActiveItem(this.position, cat, f, b, r, 0);
                        this.agenda.Push(i);
                    }

                    this.chart.AddProduction(cat, f, b);
                }
            }
        }

        /// <summary>
        /// Ads an active item to the active set.
        /// </summary>
        /// <param name="cat">The category.</param>
        /// <param name="cons">The part.</param>
        /// <param name="item">The active item.</param>
        /// <param name="currentActive">Current active set.</param>
        /// <returns>True if added successfully.</returns>
        private bool AddActiveSet(int cat, int cons, ActiveItem item, Dictionary<int, Dictionary<int, HashSet<ActiveItem>>> currentActive)
        {
            Dictionary<int, HashSet<ActiveItem>> map;
            if (currentActive.TryGetValue(cat, out map))
            {
                HashSet<ActiveItem> activeItems;
                if (map.TryGetValue(cons, out activeItems))
                {
                    foreach (ActiveItem ai in activeItems)
                    {
                        if (ai.Equals(item))
                        {
                            return false;
                        }
                    }

                    activeItems.Add(item);
                    return true;
                }
                else
                {
                    activeItems = new HashSet<ActiveItem>();
                    activeItems.Add(item);
                    map.Add(cons, activeItems);
                }
            }
            else
            {
                map = new Dictionary<int, HashSet<ActiveItem>>();
                HashSet<ActiveItem> activeItems = new HashSet<ActiveItem>();
                activeItems.Add(item);
                map.Add(cons, activeItems);
                currentActive.Add(cat, map);
            }

            return true;
        }

        /// <summary>
        /// Gets the current active set.
        /// </summary>
        /// <param name="cat">Current category.</param>
        /// <param name="currentActive">The dictionary with ActiveItems</param>
        /// <returns>Returns a HashSet with active items.</returns>
        private HashSet<ActiveItem> GetActiveSet(int cat, Dictionary<int, Dictionary<int, HashSet<ActiveItem>>> currentActive)
        {
            HashSet<ActiveItem> ai = new HashSet<ActiveItem>();
            Dictionary<int, HashSet<ActiveItem>> map;
            if (currentActive.TryGetValue(cat, out map))
            {
                foreach (int key in map.Keys)
                {
                    foreach (ActiveItem i in map[key])
                    {
                        ai.Add(i);
                    }
                }
            }

            return ai;
        }
    }
}
