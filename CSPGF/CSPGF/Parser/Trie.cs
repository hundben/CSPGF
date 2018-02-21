//-----------------------------------------------------------------------
// <copyright file="Trie.cs" company="None">
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
    using Grammar;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The parse tree.
    /// </summary>
    internal class Trie
    {
        /// <summary>
        /// Initializes a new instance of the Trie class.
        /// </summary>
        public Trie()
        {
            this.Value = new List<ActiveItem>();
            this.Items = new Dictionary<string, Trie>();
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public List<ActiveItem> Value { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public Dictionary<string, Trie> Items { get; set; }

        /// <summary>
        /// Inserts a new tree for each token in keys.
        /// </summary>
        /// <param name="keys">The list of keys.</param>
        /// <param name="obj">The tree.</param>
        /// <returns>The new tree.</returns>
        public Trie InsertChain(List<string> keys, Trie obj)
        {
            var node = this;        // TODO check if correct
            foreach (string key in keys)
            {
                Trie nnode;
                if (this.Items.ContainsKey(key))
                {
                    nnode = node.Items[key];
                }
                else
                {
                    nnode = new Trie();
                    node.Items[key] = nnode;
                }

                node = nnode;
            }

            node.Value = obj.Value; 
            return node;
        }

        /// <summary>
        /// Inserts a chain of trees.
        /// </summary>
        /// <param name="keys">The list of tokens.</param>
        /// <param name="obj">The tree used.</param>
        /// <returns>The new tree.</returns>
        public Trie InsertChain1(List<string> keys, Trie obj)
        {
            var node = this;    // TODO this is wrong? copy?
            foreach (string key in keys)
            {
                Trie nnode;
                if (this.Items.ContainsKey(key))
                {
                    nnode = node.Items[key];
                }
                else
                {
                    nnode = new Trie();
                    node.Items[key] = nnode;
                }

                node = nnode;
            }
            
            if (node.Value.Count == 0)
            {
                node.Value = new List<ActiveItem>();
                node.Value.AddRange(obj.Value);
            }
            else
            {
                node.Value.AddRange(obj.Value);
            }

            return node;
        }

        /// <summary>
        /// Looks for a tree for the corresponding token.
        /// </summary>
        /// <param name="key">The token.</param>
        /// <returns>The corresponding tree.</returns>
        public Trie Lookup(string key)
        {
            if (this.Items.ContainsKey(key))
            {
                return this.Items[key];
            }
            else
            {
                return new Trie();
            }
        }

        /// <summary>
        /// Checks if the tree is empty.
        /// </summary>
        /// <returns>Returns true if empty.</returns>
        public bool IsEmpty()
        {
            // TODO value should not be null
            if (this.Value.Count != 0)
            {
                return false;
            }

            if (this.Items.Count > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Converts a literal argument to a string, only for predict
        /// </summary>
        /// <param name="arg">Argument to convert</param>
        /// <returns>Argument converted to string</returns>
        private String LitToString(int arg)
        {
            if (arg == -1)
            {
                // String
                return "\"STRING\"";
            }
            else if (arg == -2)
            {
                // Int
                return "123";
            }
            else if (arg == -3)
            {
                // Float
                return "3.14";
            }
            else if (arg == -4)
            {
                // Var
                return "\"VARIABLE\"";
            }
            else
            {
                return "\"UNKNOWN\"";
            }
        }

        /// <summary>
        /// Checks symbols and adds tokens for the corresponding ones and adds arguments to check
        /// </summary>
        /// <param name="symbol">The symbol to check</param>
        /// <param name="oldargs">List of arguments for active item</param>
        /// <returns></returns>
        private Tuple<List<String>, Tuple<int, int>> ExpandSymbol(Symbol symbol, List<int> oldargs)
        {
            int arg = -100;
            int cons = -100;
            var tokens = new List<String>();
            if (symbol is SymbolKP)
            {
                var symkp = symbol as SymbolKP;
                // TODO
                // Has a list of symbols... check how to use it
            }
            else if (symbol is SymbolKS)
            {
                var symks = symbol as SymbolKS;
                var temp = new StringBuilder();
                foreach (var token in symks.Tokens)
                {
                    temp.Append(token);
                    temp.Append(" ");
                }

                tokens.Add(temp.ToString().Trim());
            }
            else if (symbol is SymbolCat)
            {
                var symcat = symbol as SymbolCat;
                arg  = symcat.Arg;
                cons = symcat.Label;
            }
            else if (symbol is SymbolLit)
            {
                var symlit = symbol as SymbolLit;
                tokens.Add(LitToString(oldargs[symlit.Arg]));
            }
            else if (symbol is SymbolAllCapit)
            {
                tokens.Add("&|");
            }
            else if (symbol is SymbolBind)
            {
                tokens.Add("&+");
            }
            else if (symbol is SymbolCapit)
            {
                tokens.Add("&|");
            }
            else if (symbol is SymbolNE)
            {
                // TODO check haskell version and the correct one
            }
            else if (symbol is SymbolSoftBind)
            {
                // TODO check haskell version and add the correct one
            }
            else if (symbol is SymbolSoftSpace)
            {
                // TODO check haskell version and add the correct one
            }
            else if (symbol is SymbolVar)
            {
                // TODO check haskell version and add the correct one
            }

            return new Tuple<List<string>, Tuple<int, int>>(tokens, new Tuple<int, int>(arg, cons));
        }

        /// <summary>
        /// Creates a list of predicted next tokens
        /// </summary>
        /// <param name="chart">The current chart</param>
        /// <returns>A list of tokens</returns>
        public List<string> Predict(Chart chart)
        {
            List<string> tokens = new List<string>();

            var allArgs = new HashSet<int>();

            foreach(ActiveItem ai in this.Value)
            {
                if (ai.Seq.Count == 0)
                {
                    continue;
                }

                var temp = ExpandSymbol(ai.Seq[ai.Dot], ai.Args);
                tokens.AddRange(temp.Item1);

                var currentArgs = new List<int>();
                var currentCons = new List<int>();

                // If we have arguments add them to stack TODO maybe use a real stack here
                if (temp.Item2.Item1 >= 0) {
                    currentArgs.Add(ai.Args[temp.Item2.Item1]);
                    currentCons.Add(temp.Item2.Item2);
                }

                // Check if we have symbolcat to check
                while ( currentArgs.Count > 0 )
                {
                    var arg = currentArgs[0];
                    currentArgs.RemoveAt(0);
                    var cons = currentCons[0];
                    currentCons.RemoveAt(0);

                    // Skipp arg if we have already checked it
                    if (allArgs.Contains(arg)) {
                        continue;
                    }
                    // Add to list of checked arguments
                    allArgs.Add(arg);

                    // Get all the Functions that corresponds to the argument
                    var tempForest = chart.ExpandForest(arg);
                    foreach(Production p in tempForest)
                    {
                        if (p is ProductionApply) {
                            var papp = p as ProductionApply;
                            var seqs = papp.Function.Sequences;
                            var domain = papp.Domain();
                            var seq = seqs[cons];
                            var symbol = seq[0];
                            var tup = ExpandSymbol(symbol, ai.Args);
                            tokens.AddRange(tup.Item1);
                            if (tup.Item2.Item1 >= 0)
                            {
                                currentArgs.Add(domain[tup.Item2.Item1]);
                                currentCons.Add(tup.Item2.Item2);
                            }
                        }
                        else if (p is ProductionConst)
                        {
                            var pconst = p as ProductionConst;
                            // TODO check the rest of the production
                        }
                        else
                        {
                            throw new System.InvalidOperationException("Unhandled production of type: " + p.GetType());
                        }
                    }
                }

            }

            return tokens;
        }
    }
}
