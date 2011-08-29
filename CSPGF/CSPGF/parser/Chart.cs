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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CSPGF.Reader;

    /// <summary>
    /// The chart.
    /// </summary>
    [Serializable]
    public class Chart
    {
        /// <summary>
        /// The set of productions
        /// </summary>
        private Dictionary<int, HashSet<Production>> productionSets = new Dictionary<int, HashSet<Production>>();

        /// <summary>
        /// Contains all categories with an index as a value.
        /// </summary>
        private Dictionary<Category, int> categoryBookKeeper = new Dictionary<Category, int>();

        /// <summary>
        /// The next category index to use
        /// </summary>
        private int nextCat;

        /// <summary>
        /// Initializes a new instance of the Chart class.
        /// </summary>
        /// <param name="nextCat">The next category index to use.</param>
        public Chart(int nextCat)
        {
            this.nextCat = nextCat;
        }

        /// <summary>
        /// Adds a production to the productionset.
        /// </summary>
        /// <param name="p">The production to add.</param>
        public void AddProduction(Production p)
        {
            HashSet<Production> prodSet;
            if (this.productionSets.TryGetValue(p.FId, out prodSet))
            {
                if (prodSet.Contains(p)) 
                { 
                    return; 
                }

                prodSet.Add(p);
            }
            else
            {
                prodSet = new HashSet<Production>();
                prodSet.Add(p);
                this.productionSets.Add(p.FId, prodSet);
            }

            this.nextCat = Math.Max(this.nextCat, p.FId + 1);
        }

        /// <summary>
        /// Add a production to the productionset.
        /// </summary>
        /// <param name="cat">Category index.</param>
        /// <param name="fun">The function.</param>
        /// <param name="domain">A list of domains.</param>
        public void AddProduction(int cat, CncFun fun, List<int> domain)
        {
            this.AddProduction(new ApplProduction(cat, fun, domain));
        }

        /// <summary>
        /// Returns all the productions with the index resultCat.
        /// </summary>
        /// <param name="resultCat">Category index</param>
        /// <returns>List of all productions with the index provided.</returns>
        public List<ApplProduction> GetProductions(int resultCat)
        {
            HashSet<Production> prod;

            // Check if category exists, if not return empty productionset
            if (this.productionSets.TryGetValue(resultCat, out prod))
            {
                List<ApplProduction> applProd = new List<ApplProduction>();
                foreach (Production p in prod)
                {
                    foreach (ApplProduction ap in this.Uncoerce(p)) 
                    {
                        applProd.Add(ap);
                    }
                }

                return applProd;
            }
            else
            {
                return new List<ApplProduction>();
            }
        }
        
        /// <summary>
        /// Creates a new category.
        /// </summary>
        /// <param name="oldCat">Old category index</param>
        /// <param name="l">The Cons value.</param>
        /// <param name="j">Start index</param>
        /// <param name="k">End index</param>
        /// <returns>New category index.</returns>
        public int GetFreshCategory(int oldCat, int l, int j, int k)
        {
            // TODO Optimize this, use something else instead of looping through everything
            Category cf = new Category(oldCat, l, j, k);
            foreach (Category c in this.categoryBookKeeper.Keys)
            {
                if (cf.Equals(c))
                {
                    int i = this.categoryBookKeeper[c];
                    if (i != -1)
                    {
                        return i;
                    }
                }
            }

            return this.GenerateFreshCategory(cf);
        }

        /// <summary>
        /// Get a category.
        /// </summary>
        /// <param name="oldCat">The old category.</param>
        /// <param name="cons">Add description for cons.</param>
        /// <param name="begin">Where it begins.</param>
        /// <param name="end">Where it ends.</param>
        /// <returns>Returns the category.</returns>
        public int GetCategory(int oldCat, int cons, int begin, int end)
        {
            Category cf = new Category(oldCat, cons, begin, end);
            foreach (Category c in this.categoryBookKeeper.Keys)
            {
                if (c.Equals(cf))
                {
                    return this.categoryBookKeeper[c];
                }
            }

            // TODO check consistency of this
            return -1;
        }

        /// <summary>
        /// Generate a fresh category.
        /// </summary>
        /// <param name="c">The old category.</param>
        /// <returns>The new category</returns>
        public int GenerateFreshCategory(Category c)
        {
            int cat = this.nextCat;
            this.nextCat++;
            this.categoryBookKeeper[c] = cat;    // TODO maybe add check here
            return cat;
        }

        /// <summary>
        /// Creates a string with data for debugging.
        /// </summary>
        /// <returns>Pretty print of all the important data for debugging.</returns>
        public override string ToString()
        {
            string s = "=== Productions: ===\n";
            foreach (int i in this.productionSets.Keys) 
            {
                s += this.productionSets[i].ToString() + '\n';
            }

            s += "=== passive items: ===\n";
            foreach (KeyValuePair<Category, int> ints in this.categoryBookKeeper) 
            {
                // TODO add ToString on Category I guess? :D
                s += ints.Key.ToString() + " -> " + ints.Value + '\n';
            }

            return s;
        }

        /// <summary>
        /// Converts from CoerceProduction to ApplProducion.
        /// </summary>
        /// <param name="p">Production to convert.</param>
        /// <returns>List of ApplProductions</returns>
        private List<ApplProduction> Uncoerce(object p)
        {
            List<ApplProduction> prodList = new List<ApplProduction>();
            if (p is ApplProduction) 
            {
                prodList.Add((ApplProduction)p);
            }
            else if (p is CoerceProduction) 
            {
                CoerceProduction cp = (CoerceProduction)p;
                foreach (Production prod in this.GetProductions(cp.InitId)) 
                {
                    foreach (ApplProduction prod2 in this.Uncoerce(prod)) 
                    {
                        prodList.Add(prod2);
                    }
                }
            }

            return prodList;
        }
    }
}
