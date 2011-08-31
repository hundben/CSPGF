//-----------------------------------------------------------------------
// <copyright file="Linearizer2.cs" company="None">
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

namespace CSPGF
{
    using System.Collections.Generic;
    using CSPGF.Reader;
    using CSPGF.Trees.Absyn;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>

    public class Linearizer2
    {
        private PGF pgf;
        private Concrete concrete;
        private Dictionary<string, Dictionary<int, HashSet<Production>>> linProds;

        public Linearizer2(PGF pgf, Concrete concrete)
        {
            this.pgf = pgf;
            this.concrete = concrete;
            this.CreateProductions();
        }

        public Linearizer2(PGF pgf, string language)
        {
            this.pgf = pgf;
            this.concrete = pgf.GetConcrete(language);
            this.CreateProductions();
        }

        public string Linearize(Tree tree)
        {
            List<Tree> trees = new List<Tree>();
            if (tree is Application)
            {
                do
                {
                    trees.Add(((Application)tree).Tree_2);
                    tree = ((Application)tree).Tree_1;
                } while (tree is Application);
            }

            if (tree is Function)
            {
                this.Apply(tree); 
                System.Console.WriteLine(((Function)tree).Ident_);
            }
            else
            {
                throw new LinearizerException("Unhandled tree");
            }
            return string.Empty;
        }

        public void SetProductions(Dictionary<string, Dictionary<int, HashSet<Production>>> prods)
        {
            linProds = prods;
        }

        private void CreateProductions() 
        {
            // RemoveProds(this.concrete.GetSetOfProductions());
        }

        private void Apply(Tree tree)
        {

        }

























        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// </summary>
        /// <param name="prods"></param>
        /// <returns></returns>


        private Dictionary<int, HashSet<Production>> RemoveProds(Dictionary<int, HashSet<Production>> prods)
        {
            Dictionary<int, HashSet<Production>> prods0 = new Dictionary<int, HashSet<Production>>();
            HashSet<Production> set = new HashSet<Production>();
            foreach (KeyValuePair<int, HashSet<Production>> kvp in prods)
            {
                foreach (Production p in kvp.Value)
                {
                    if (p is ApplProduction)
                    {
                        foreach (int i in p.Domain())
                        {
                            if (this.IsLiteral(i) || prods.ContainsKey(i))
                            {
                                set.Add(p);
                            }
                        }
                    }
                    else if (p is CoerceProduction)
                    {
                        // if (prods.ContainsKey(p.FId))
                        if(FailCoerce(p, prods))
                        {
                            set.Add(p);
                        }
                    }
                }
                prods0.Add(kvp.Key, set);
                set = new HashSet<Production>();
            }

            Dictionary<int, HashSet<Production>> prods123 = new Dictionary<int, HashSet<Production>>();
            set = new HashSet<Production>();
            foreach (KeyValuePair<int, HashSet<Production>> kvp in prods)
            {
                foreach (Production p in kvp.Value)
                {
                    if (p is ApplProduction)
                    {
                        foreach (int i in p.Domain())
                        {
                            if (this.IsLiteral(i) || prods.ContainsKey(i))
                            {
                                set.Add(p);
                            }
                        }
                    }
                    else if (p is CoerceProduction)
                    {
                        // if (prods.ContainsKey(p.FId))
                        if (FailCoerce(p, prods))
                        {
                            set.Add(p);
                        }
                    }
                }
                prods123.Add(kvp.Key, set);
                set = new HashSet<Production>();
            }



            Dictionary<int, HashSet<Production>> productions = new Dictionary<int, HashSet<Production>>();
            foreach (KeyValuePair<int, HashSet<Production>> kvp in prods0)
            {
                productions.Add(kvp.Key, kvp.Value);
            }

            HashSet<Production> ert = new HashSet<Production>();
            foreach (KeyValuePair<int, HashSet<Production>> kvp in prods0)
            {
                foreach (Production p234 in kvp.Value)
                {
                    ert.Add(p234);
                }
                foreach (Production p in kvp.Value)
                {
                    if (p is CoerceProduction)
                    {
                        if (FailCoerce(p,prods))
                        {
                            ert.Remove(p);
                        }
                    }
                }
                if (ert.Count == 0)
                {
                    productions.Remove(kvp.Key);
                }
                ert = new HashSet<Production>();
            }

            foreach (KeyValuePair<int, HashSet<Production>> kvp in productions)
            {
            }

            return productions;
        }

        private bool FailCoerce(Production p, Dictionary<int, HashSet<Production>> prods)
        {
            if (p is CoerceProduction)
            {
                HashSet<Production> set;
                foreach (int i in p.Domain())
                {
                    if (prods.TryGetValue(i, out set))
                    {
                        foreach (Production p2 in set)
                        {
                            return FailCoerce(p2, prods);
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            else if (p is ApplProduction)
            {
                return false;
            }
            throw new LinearizerException("FailCoerce failar");
        }

        private bool IsLiteral(int i) {
            if (i >= -4 && i < 0)
            {
                return true;
            }
            return false;
        }
    }
}
