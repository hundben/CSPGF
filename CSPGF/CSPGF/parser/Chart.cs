//-----------------------------------------------------------------------
// <copyright file="Chart.cs" company="None">
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
    /// The chart. Stores production sets and new categories.
    /// </summary>
    internal class Chart
    {
        /// <summary>
        /// The set of productions
        /// </summary>
        private Dictionary<int, HashSet<Production>> productionSets = new Dictionary<int, HashSet<Production>>();

        /// <summary>
        /// Contains all categories with an index as a value.
        /// This is just a fun test :D Is faster than old version but probably not near a good solution.
        /// </summary>
        private Dictionary<string, int> categoryBookKeeperHash = new Dictionary<string, int>();

        /// <summary>
        /// The next category index to use
        /// </summary>
        private int nextCat;

        /// <summary>
        /// Last production
        /// </summary>
        private int currentProd;

        /// <summary>
        /// Handles productions, used to remove newer productions.
        /// </summary>
        private Stack<int> lastProduction = new Stack<int>();

        /// <summary>
        /// Initializes a new instance of the Chart class.
        /// </summary>
        /// <param name="nextCat">The next category index to use.</param>
        public Chart(int nextCat)
        {
            this.nextCat = nextCat;
            this.currentProd = 0;
        }

        /// <summary>
        /// Removes categories to save some memory.
        /// </summary>
        public void NextToken()
        {
            this.categoryBookKeeperHash = new Dictionary<string, int>();
            this.lastProduction.Push(this.currentProd);
        }

        /// <summary>
        /// Removes all productions associated with the last production
        /// </summary>
        public void RemoveToken()
        {
            int remove = this.lastProduction.Pop();
            HashSet<int> temp = new HashSet<int>(this.productionSets.Keys);

            foreach (int key in temp)
            {
                if (key > remove)
                {
                    this.productionSets.Remove(key);
                }
            }
        }

        /// <summary>
        /// Adds a production to the production set.
        /// </summary>
        /// <param name="p">The production to add.</param>
        /// <returns>Returns true if production was added.</returns>
        public bool AddProduction(Production p)
        {
            HashSet<Production> prodSet;
            if (this.productionSets.TryGetValue(p.FId, out prodSet))
            {
                if (prodSet.Contains(p)) 
                { 
                    return false; 
                }

                prodSet.Add(p);
            }
            else
            {
                prodSet = new HashSet<Production> { p };
                this.productionSets.Add(p.FId, prodSet);
            }

            this.currentProd = p.FId;
            return true;
        }

        /// <summary>
        /// Add a production to the production set.
        /// </summary>
        /// <param name="cat">Category index.</param>
        /// <param name="fun">The function.</param>
        /// <param name="domain">A list of domains.</param>
        /// <returns>Returns true if production was added.</returns>
        public bool AddProduction(int cat, CncFun fun, int[] domain)
        {
            return this.AddProduction(new ProductionApply(cat, fun, domain));
        }

        /// <summary>
        /// Returns all the productions with the index resultCat.
        /// </summary>
        /// <param name="resultCat">Category index</param>
        /// <returns>List of all productions with the index provided.</returns>
        public List<Production> GetProductions(int resultCat)
        {
            HashSet<Production> prod;

            // Check if category exists, if not return empty productionset
            if (this.productionSets.TryGetValue(resultCat, out prod))
            {
                List<Production> applProd = new List<Production>();
                foreach (Production p in prod)
                {
                    applProd.AddRange(this.Uncoerce(p));
                }

                return applProd;
            }
            else
            {
                return new List<Production>();
            }
        }
        
        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="oldCat">Old category index</param>
        /// <param name="l">The constituent value.</param>
        /// <param name="j">Start index</param>
        /// <param name="k">End index</param>
        /// <returns>New category index.</returns>
        public int GetFreshCategory(int oldCat, int l, int j, int k)
        {
            string hash = this.GenerateHash(oldCat, l, j, k);
            if (this.categoryBookKeeperHash.ContainsKey(hash))
            {
                return this.categoryBookKeeperHash[hash];
            }
            else
            {
                return this.GenerateFreshCategory(hash);
            }
        }

        /// <summary>
        /// Get a category.
        /// </summary>
        /// <param name="oldCat">The old category.</param>
        /// <param name="cons">Add description for cons.</param>
        /// <param name="begin">Where it begins.</param>
        /// <param name="end">Where it ends.</param>
        /// <returns>Returns the category.</returns>
        public int? GetCategory(int oldCat, int cons, int begin, int end)
        {
            string hash = this.GenerateHash(oldCat, cons, begin, end);

            if (this.categoryBookKeeperHash.ContainsKey(hash))
            {
                return this.categoryBookKeeperHash[hash];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Generates a fresh category
        /// </summary>
        /// <param name="oldCat">Old category.</param>
        /// <param name="l">Where to begin.</param>
        /// <param name="j">Where to end.</param>
        /// <param name="k">The position.</param>
        /// <returns>The new category.</returns>
        public int GenerateFreshCategory(int oldCat, int l, int j, int k)
        {
            return this.GenerateFreshCategory(this.GenerateHash(oldCat, l, j, k));
        }

        /// <summary>
        /// Creates a string with data for debugging.
        /// </summary>
        /// <returns>Pretty print of all the important data for debugging.</returns>
        public override string ToString()
        {
            string s = string.Empty; 
            s = "=== Productions: ===\n";
            foreach (int i in this.productionSets.Keys) 
            {
                s += " < PRODUCTION SET :" + i + " >\n";
                foreach (Production p in this.productionSets[i])
                {
                    s += p + "\n";
                }
            }

            s += "=== passive items: ===\n";
            foreach (KeyValuePair<string, int> ints in this.categoryBookKeeperHash) 
            {
                // TODO add ToString on Category I guess? :D
                s += "[" + ints.Key + "] = " + ints.Value + '\n';
            }

            return s;
        }

        /// <summary>
        /// Generate a fresh category.
        /// </summary>
        /// <param name="hash">The old category.</param>
        /// <returns>The new category</returns>
        private int GenerateFreshCategory(string hash)
        {
            int cat = this.nextCat;
            this.nextCat++;
            this.categoryBookKeeperHash[hash] = cat;
            return cat;
        }

        /// <summary>
        /// Converts from CoerceProduction to ApplyProduction.
        /// </summary>
        /// <param name="p">Production to convert.</param>
        /// <returns>List of ApplyProductions</returns>
        private List<Production> Uncoerce(Production p)
        {
            List<Production> prodList = new List<Production>();
            
            if (p is ProductionCoerce)
            {
                ProductionCoerce cp = (ProductionCoerce)p;
                foreach (Production prod in this.GetProductions(cp.InitId)) 
                {
                    prodList.AddRange(this.Uncoerce(prod));
                }
            }
            else
            {
                prodList.Add(p);
            }

            return prodList;
        }

        /// <summary>
        /// Generates a hash for the dictionary.
        /// </summary>
        /// <param name="oldCat">The old category.</param>
        /// <param name="cons">The constituent.</param>
        /// <param name="begin">Where it begins.</param>
        /// <param name="end">Where it ends.</param>
        /// <returns>The hash as a string.</returns>
        private string GenerateHash(int oldCat, int cons, int begin, int end)
        {
            return oldCat + " " + cons + " " + begin + " " + end;
        }
    }
}
