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
    using System.Collections.Generic;
    using Grammar;

    /// <summary>
    /// The parsestate class.
    /// </summary>
    internal class ParseState
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
        /// List of position in the parsetrie (new for each new token).
        /// </summary>
        private Stack<ParseTrie> listOfTries;

        /// <summary>
        /// Stack of tokens.
        /// </summary>
        private Stack<string> tokens;

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
            this.trie = new ParseTrie();

            this.listOfTries = new Stack<ParseTrie>();
            this.listOfTries.Push(this.trie);
            this.tokens = new Stack<string>();

            this.chart = new Chart(grammar.FId + 1);

            this.agenda = new Stack<ActiveItem>();
            this.position = 0;
            this.active = new List<Dictionary<int, Dictionary<int, HashSet<ActiveItem>>>>();

            // Initial Predict
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
        public List<Trees.Absyn.Tree> GetTrees()
        {
            TreeBuilder tb = new TreeBuilder();
            TreeConverter tc = new TreeConverter();
            List<Trees.Absyn.Tree> trees = new List<Trees.Absyn.Tree>();
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
        /// <returns>Returns true if scan is successful.</returns>
        public bool Scan(string token)
        {

            //TODO check for literal

            ParseTrie newTrie = this.trie.GetSubTrie(token);

            if (newTrie != null)
            {
                this.listOfTries.Push(newTrie);
                Stack<ActiveItem> newAgenda = newTrie.Lookup(new List<string>());
                if (newAgenda != null)
                {
                    this.tokens.Push(token);

                    this.chart.NextToken();
                    this.trie = newTrie;
                    this.position++;
                    this.agenda = newAgenda;
                    this.Compute();

                    return true;
                }
            }
            else
            {
                newTrie = new ParseTrie();

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
        /// Removes all tokens but keeps initial state.
        /// </summary>
        public void Reset()
        {
            while (this.RemoveToken())
            {
            }
        }

        /// <summary>
        /// Remove the last scanned token.
        /// </summary>
        /// <returns>Return true if removed successfully.</returns>
        public bool RemoveToken()
        {
            if (this.position > 0)
            {
                this.chart.RemoveToken();
                ParseTrie t = this.listOfTries.Pop();
                this.trie = this.listOfTries.Peek();
                string token = this.tokens.Pop();
                this.trie.ResetChild(token);
                
                this.active.RemoveAt(this.position);
                this.position--;
                return true;
            }
            else
            {
                return false;
            }
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
            int[] b = item.Domain;
            int l = item.Constituent;
            int p = item.Position;

            // TempLog.LogMessageToFile("Processing active item: " + item + " ");
            Symbol sym = item.CurrentSymbol();

            if (sym is ToksSymbol)
            {
                // TempLog.LogMessageToFile("Case before s in T");
                ToksSymbol tok = (ToksSymbol)sym;
                string[] tokens = tok.Tokens;
                ActiveItem i = new ActiveItem(j, a, f, b, l, p + 1);

                // SCAN
                Stack<ActiveItem> newAgenda = this.trie.Lookup(tokens);
                if (newAgenda == null) 
                {
                    Stack<ActiveItem> sai = new Stack<ActiveItem>();
                    this.trie.Add(tokens, sai);
                    newAgenda = sai;
                }

                newAgenda.Push(i);
            }
            else if (sym is ArgConstSymbol) 
            {
                // TempLog.LogMessageToFile("Case before <d,r>");
                ArgConstSymbol arg = (ArgConstSymbol)sym;
                int d = arg.Arg;
                int r = arg.Cons;
                int bd = item.Domain[d];

                // PREDICT
                if (this.active.Count >= this.position) 
                {
                    if (this.AddActiveSet(bd, r, item, this.active[this.position])) 
                    {
                        foreach (ApplProduction prod in this.chart.GetProductions(bd)) 
                        {
                            ActiveItem it = new ActiveItem(this.position, bd, prod.Function, prod.Domain(), r, 0);
                            this.agenda.Push(it);
                        }
                    }

                    // COMBINE
                    int cat = this.chart.GetCategory(bd, r, this.position, this.position);

                    if (cat != -1) 
                    {
                        List<int> newDomain = new List<int>(b);
                        newDomain[d] = cat;

                        // TODO: FIX!
                        ActiveItem it = new ActiveItem(j, a, f, newDomain.ToArray(), l, p + 1);
                        this.agenda.Push(it);

                        // TempLog.LogMessageToFile("Adding to agenda: " + it.ToString());
                    }
                }
            }
            else if (sym is LitSymbol)
            {
                // TempLog.LogMessageToFile("Case before {d,r}");
                LitSymbol arg = (LitSymbol)sym;
                int d = arg.Arg;
                int r = arg.Cons;
                int bd = item.Domain[d];

                // LITERAL
                // TODO check if this is even close to correct :D

                // TODO add function (below is just a test)
                Symbol[][] symb = { };
                CncFun freshFun = new CncFun("new:" + bd, symb);

                //COMBINE (LIT VERSION)
                int n = this.chart.GetCategory(bd, r, this.position, this.position);


                //COMBINE 
                if (n == -1)
                {
                    n = this.chart.GenerateFreshCategory(bd, r, this.position, this.position); //??
                    List<int> newDomain = new List<int>(b);
                    newDomain[d] = n;

                    ActiveItem it = new ActiveItem(j, a, f, newDomain.ToArray(), l, p + 1);
                    this.agenda.Push(it);

                    // TempLog.LogMessageToFile("Adding to agenda: " + it.ToString());
                }
            }
            else if (sym is VarSymbol)
            {
                // TODO implement
                // High-order argument
                // TempLog.LogMessageToFile("Case before <d,$r>");
            }
            else
            {
                // TempLog.LogMessageToFile("Case at the end");
                int cat = this.chart.GetCategory(a, l, j, this.position);
                if (cat == -1)
                {
                    // COMPLETE
                    int n = this.chart.GenerateFreshCategory(a, l, j, this.position);

                    // COMBINE
                    foreach (ActiveItem ai in this.GetActiveSet(a, l, this.active[j]))
                    {
                        ActiveItem ip = ai;
                        int d = ((ArgConstSymbol)ai.CurrentSymbol()).Arg;
                        List<int> domain = new List<int>(ip.Domain);

                        // TempLog.LogMessageToFile("Combine with " + ip.ToString() + "(" + domain[d] + ")");
                        domain[d] = n;

                        // TODO: FIX!
                        ActiveItem i = new ActiveItem(ip.Begin, ip.Category, ip.Function, domain.ToArray(), ip.Constituent, ip.Position + 1);
                        this.agenda.Push(i);
                    }

                    this.chart.AddProduction(n, f, b);
                }
                else
                {
                    // PREDICT
                    HashSet<ActiveItem> items = this.GetActiveSet(cat, this.active[this.position]);
                    foreach (ActiveItem ai in items)
                    {
                        int r = ((ArgConstSymbol)ai.CurrentSymbol()).Cons;  // Cons?
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

        /// <summary>
        /// Gets an active set from the list of current active sets..
        /// </summary>
        /// <param name="cat">The category.</param>
        /// <param name="cons">The constituent.</param>
        /// <param name="currentActive">The dictionary with the sets.</param>
        /// <returns>A hashset with the active items.</returns>
        private HashSet<ActiveItem> GetActiveSet(int cat, int cons, Dictionary<int, Dictionary<int, HashSet<ActiveItem>>> currentActive)
        {
            HashSet<ActiveItem> ai = new HashSet<ActiveItem>();
            Dictionary<int, HashSet<ActiveItem>> map;
            if (currentActive.TryGetValue(cat, out map))
            {
                HashSet<ActiveItem> aitmp;
                if (map.TryGetValue(cons, out aitmp))
                {
                    return aitmp;
                }
            }

            return ai;
        }
    }
}
