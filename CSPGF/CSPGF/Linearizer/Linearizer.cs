//-----------------------------------------------------------------------
// <copyright file="Linearizer.cs" company="None">
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

namespace CSPGF.Linearize
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Grammar;
    using Trees.Absyn;

    /// <summary>
    /// Linearizer for use with the Parser
    /// </summary>
    internal class Linearizer
    {
        /// <summary>
        /// The current PGF-file
        /// </summary>
        private readonly PGF pgf;

        /// <summary>
        /// The current Concrete grammar
        /// </summary>
        private readonly Concrete cnc;

        /// <summary>
        /// Linearization productions
        /// </summary>
        private readonly Dictionary<string, Dictionary<int, HashSet<Production>>> linProd;

        /// <summary>
        /// Initializes a new instance of the Linearizer class.
        /// Linearizes an expression to a bracketed token
        /// and further on to a string
        /// not implemented to dependent categories, implicit argument,
        /// and higher-order abstract syntax
        /// </summary>
        /// <param name="pgf">PGF-file containing the concrete grammar</param>
        /// <param name="concrete">Concrete grammar</param>
        public Linearizer(PGF pgf, Concrete concrete)
        {
            this.pgf = pgf;
            this.cnc = concrete;
            this.linProd = this.GetLProductions();
        }

        /// <summary>
        /// Linearizes a tree to a list of tokens.
        /// </summary>
        /// <param name="absyn">Tree to linearize</param>
        /// <returns>List of strings</returns>
        public List<string> LinearizeTokens(Tree absyn)
        {
            return this.RenderLin(this.Linearize(absyn).ElementAt(0));
        }

        /// <summary>
        /// Linearize a tree to a string.
        /// </summary>
        /// <param name="absyn">Tree to linearize</param>
        /// <returns>Linearized string</returns>
        public string LinearizeString(Tree absyn)
        {
            return string.Join(" ", this.LinearizeTokens(absyn)).Trim();
        }

        /// <summary>
        /// Constructs the l-productions of the concrete syntax for a given language
        /// </summary>
        /// <returns>Dictionary containing the l-productions</returns>
        private Dictionary<string, Dictionary<int, HashSet<Production>>> GetLProductions()
        {
            return this.LinIndex(this.FilterProductions(new Dictionary<int, HashSet<Production>>(), this.cnc.GetSetOfProductions()));
        }

        /// <summary>
        /// Aligns the indexes for the l-productions
        /// </summary>
        /// <param name="productions">Productions to align</param>
        /// <returns>Aligned l-productions</returns>
        private Dictionary<string, Dictionary<int, HashSet<Production>>> LinIndex(Dictionary<int, HashSet<Production>> productions)
        {
            Dictionary<string, Dictionary<int, HashSet<Production>>> vtemp = new Dictionary<string, Dictionary<int, HashSet<Production>>>();
            foreach (KeyValuePair<int, HashSet<Production>> kvp in productions)
            {
                foreach (Production prod in kvp.Value)
                {
                    List<string> vs = this.GetFunctions(prod, productions);
                    if (vs.Count != 0)
                    {
                        foreach (string str in vs)
                        {
                            Dictionary<int, HashSet<Production>> htemp = new Dictionary<int, HashSet<Production>>();
                            HashSet<Production> singleton = new HashSet<Production> { prod };
                            htemp.Add(kvp.Key, singleton);
                            if (vtemp.ContainsKey(str))
                            {
                                Dictionary<int, HashSet<Production>> obj = vtemp[str];
                                if (obj.ContainsKey(kvp.Key))
                                {
                                    HashSet<Production> ttemp = obj[kvp.Key];
                                    ttemp.Add(prod);
                                    vtemp[str] = obj;
                                }
                                else
                                {
                                    obj.Add(kvp.Key, singleton);
                                    vtemp[str] = obj;
                                }
                            }
                            else
                            {
                                vtemp.Add(str, htemp);
                            }
                        }
                    }
                }
            }

            return vtemp;
        }

        /// <summary>
        /// This function computes the list of abstract function corresponding to
        /// a given production. This is easy for standard productions but less for
        /// coercions because then you have to search reccursively.
        /// </summary>
        /// <param name="p">The production</param>
        /// <param name="productions">Procutions to use</param>
        /// <returns>List of strings</returns>
        private List<string> GetFunctions(Production p, Dictionary<int, HashSet<Production>> productions)
        {
            List<string> rez = new List<string>();
            if (p is ProductionApply)
            {
                rez.Add(((ProductionApply)p).Function.Name);
            }
            else
            {
                HashSet<Production> prods;
                if (productions.TryGetValue(((ProductionCoerce)p).InitId, out prods))
                {
                    foreach (Production pp in prods)
                    {
                        rez.AddRange(this.GetFunctions(pp, productions));
                    }
                }
            }

            return rez;
        }

        /// <summary>
        /// Checks if i is the index of a literal or a valid set of productions
        /// </summary>
        /// <param name="i">Index to check for</param>
        /// <param name="prods">Set of productions</param>
        /// <returns>True if true</returns>
        private bool ConditionProd(int i, Dictionary<int, HashSet<Production>> prods)
        {
            if (this.IsLiteral(i))
            {
                return true;
            }

            return prods.ContainsKey(i);
        }

        /// <summary>
        /// Filters a set of productions according to filterRule
        /// </summary>
        /// <param name="prods0">Dictionary containing sets of productions</param>
        /// <param name="set">Set of productions</param>
        /// <returns>Set of Productions</returns>
        private HashSet<Production> FilterProdSet1(Dictionary<int, HashSet<Production>> prods0, HashSet<Production> set)
        {
            HashSet<Production> tempset = new HashSet<Production>();
            foreach (Production prod in set)
            {
                if (this.FilterRule(prods0, prod))
                {
                    tempset.Add(prod);
                }
            }

            return tempset;
        }

        /// <summary>
        /// Filters an IntMap of productions according to filterProdsSet1
        /// </summary>
        /// <param name="prods0">Dictionary containing sets of productions</param>
        /// <param name="prods">Set of productions</param>
        /// <returns>Dictionary contining sets of productions</returns>
        private Dictionary<int, HashSet<Production>> FilterProductions(Dictionary<int, HashSet<Production>> prods0, Dictionary<int, HashSet<Production>> prods)
        {
            Dictionary<int, HashSet<Production>> tempRez = new Dictionary<int, HashSet<Production>>();
            bool areDiff = false;

            foreach (KeyValuePair<int, HashSet<Production>> kvp in prods)
            {
                HashSet<Production> intermRez = this.FilterProdSet1(prods0, kvp.Value);
                if (intermRez.Count != 0)
                {
                    tempRez.Add(kvp.Key, intermRez);
                }
            }

            Dictionary<int, HashSet<Production>> prods1 = new Dictionary<int, HashSet<Production>>(prods0);

            foreach (KeyValuePair<int, HashSet<Production>> kvp in tempRez)
            {
                HashSet<Production> hp = kvp.Value;
                if (prods0.ContainsKey(kvp.Key))
                {
                    if (!prods0[kvp.Key].SetEquals(hp))
                    {
                        foreach (Production p in prods0[kvp.Key])
                        {
                            hp.Add(p);
                        }

                        prods1[kvp.Key] = hp;
                        areDiff = true;
                    }
                }
                else
                {
                    prods1[kvp.Key] = hp;
                    areDiff = true;
                }
            }

            return areDiff ? this.FilterProductions(prods1, prods) : prods0;
        }

        /// <summary>
        /// Checks if a production satisfies conditionProd recursively
        /// </summary>
        /// <param name="prods">Productions to check</param>
        /// <param name="p">Production to use</param>
        /// <returns>True if true</returns>
        private bool FilterRule(Dictionary<int, HashSet<Production>> prods, Production p)
        {
            if (p is ProductionApply)
            {
                ProductionApply ap = (ProductionApply)p;
                foreach (int i in ap.Domain())
                {
                    if (!this.ConditionProd(i, prods))
                    {
                        return false;
                    }
                }

                return true;
            }
            else if (p is ProductionCoerce)
            {
                return this.ConditionProd(((ProductionCoerce)p).InitId, prods);
            }
            else
            {
                throw new LinearizerException("The production wasn't either an ProductionApply or a ProductionCoerce");
            }
        }

        /// <summary>
        /// Flattens a bracketed token
        /// </summary>
        /// <param name="bt">BracketedTokn to use</param>
        /// <param name="after">String to use</param>
        /// <returns>List of strings</returns>
        private List<string> Untokn(BracketedTokn bt, string after)
        {
            List<string> rez = new List<string>();
            if (bt is LeafKS)
            {
                foreach (string i in ((LeafKS)bt).Tokens.Reverse())
                {
                    rez.Add(i);
                }

                return rez;
            }
            else if (bt is LeafKP)
            {
                foreach (Alternative alt in ((LeafKP)bt).Alternatives)
                {
                    foreach (string str in alt.Alt2)
                    {
                        if (after.StartsWith(str))
                        {
                            foreach (string i in alt.Alt1.Reverse())
                            {
                                rez.Add(i);
                            }

                            return rez;
                        }
                    }
                }
                
                foreach (string i in ((LeafKP)bt).DefaultTokens.Reverse())
                {
                    rez.Add(i);
                }

                return rez;
            }
            else if (bt is Bracket)
            {
                foreach (BracketedTokn bs in (((Bracket)bt).Bracketedtoks).Reverse<BracketedTokn>())
                {
                    rez.AddRange(this.Untokn(bs, after));
                    after = rez.Last();
                }

                return rez;
            }
            else
            {
                throw new LinearizerException("Token was of unknown type.");
            }
        }

        /// <summary>
        /// Flattens the result of the linearization
        /// </summary>
        /// <param name="v">LinTriple to flatten</param>
        /// <returns>List of flattened strings</returns>
        private List<string> RenderLin(LinTriple v)
        {
            List<string> rez = new List<string>();
            List<List<BracketedTokn>> vtemp = v.LinTable;
            string after = string.Empty;
            for (int k = vtemp.ElementAt(0).Count - 1; k >= 0; k--)
            {
                rez.AddRange(this.Untokn(vtemp.ElementAt(0).ElementAt(k), after));
                after = rez.Last();
            }

            rez.Reverse();
            return rez;
        }


        /// <summary>
        /// Linearize a tree
        /// </summary>
        /// <param name="e">Tree to linearize</param>
        /// <returns>List of LinTriples</returns>
        private List<LinTriple> Linearize(Tree e)
        {
            return this.Lin0(new List<string>(), new List<string>(), null, 0, e);
        }

        /// <summary>
        /// Main Linearization function
        /// </summary>
        /// <param name="xs">list of bound variables (from lambdas)</param>
        /// <param name="ys">list of strings?</param>
        /// <param name="mbcty">Concrete type</param>
        /// <param name="mbfid">Function id</param>
        /// <param name="tree">Tree to linearize</param>
        /// <returns>List of all possible linearized tuples</returns>
        private List<LinTriple> Lin0(List<string> xs, List<string> ys, ConcreteType mbcty, int mbfid, Tree tree)
        {
            // if tree is a lambda, we add the variable to the list of bound
            // variables and we linearize the subtree.
            if (tree is Lambda)
            {
                xs.Add(((Lambda)tree).Ident_);
                List<LinTriple> tmp = this.Lin0(xs, ys, mbcty, mbfid, ((Lambda)tree).Tree_);
                return tmp;
            }
            else if (xs.Count == 0)
            {
                List<Tree> es = new List<Tree>();
                if (tree is Application)
                {
                    do
                    {
                        es.Add(((Application)tree).Tree_2);
                        tree = ((Application)tree).Tree_1;
                    }
                    while (tree is Application);
                }

                if (tree is Function)
                {
                    return this.Apply(xs, mbcty, mbfid, ((Function)tree).Ident_, es);
                }
                else
                {
                    throw new Exception("Undefined construction for expressions !!!");
                }
            }
            else
            {
                xs.AddRange(ys);
                List<Tree> exprs = new List<Tree> { tree };
                foreach (string str in xs)
                {
                    exprs.Add(new Literal(new StringLiteral(str)));
                }

                return this.Apply(xs, mbcty, mbfid, "_B", exprs);
            }
        }

        /// <summary>
        /// Intermediate linearization for complex expressions. 
        /// </summary>
        /// To linearize the application of the function "f" to the arguments (trees) a, b and c use : apply(???,???,??? "f", [a,b,c]).
        /// 'apply' will linearize the argument and then use the concrete function for "f" to glue them together.
        /// <param name="xs">List of bound variables</param>
        /// <param name="mbcty">Concrete type</param>
        /// <param name="nextfid">Insert comment</param>
        /// <param name="f">Name of the function to be applied</param>
        /// <param name="es">The argument of the function to linearize</param>
        /// <returns>All possible linearizations for the application of f to es</returns>
        private List<LinTriple> Apply(List<string> xs, ConcreteType mbcty, int nextfid, string f, List<Tree> es)
        {
            Dictionary<int, HashSet<Production>> prods;
            if (!this.linProd.TryGetValue(f, out prods))
            {
                List<Tree> newes = new List<Tree> { new Literal(new StringLiteral(f)) };
                Console.WriteLine("Function " + f + " does not have a linearization !");
                return this.Apply(xs, mbcty, nextfid, "_V", newes);
            }
            else
            {
                List<AppResult> listApp = this.GetApps(prods, mbcty, f);
                List<LinTriple> rez = new List<LinTriple>();
                foreach (AppResult appr in listApp)
                {
                    List<ConcreteType> ctys = new List<ConcreteType>();
                    for (int ind = appr.CncTypes.Count - 1; ind >= 0; ind--)
                    {
                        ctys.Add(appr.CncTypes.ElementAt(ind));
                    }

                    if (es.Count != ctys.Count)
                    {
                        throw new LinearizerException("Lengths of es and ctys don't match" + es + " -- " + ctys);
                    }

                    Symbol[][] lins = appr.CncFun.Sequences;
                    string cat = appr.CncType.CId;
                    List<Tree> copyExpr = new List<Tree>(es);
                    List<RezDesc> rezDesc = this.Descend(nextfid, ctys, copyExpr, xs);
                    foreach (RezDesc rez2 in rezDesc)
                    {
                        List<List<BracketedTokn>> linTab = new List<List<BracketedTokn>>();
                        foreach (Symbol[] seq in lins)
                        {
                            linTab.Add(this.ComputeSeq(seq, rez2.CncTypes, rez2.Bracketedtokn));
                        }

                        rez.Add(new LinTriple(nextfid + 1, new ConcreteType(cat, nextfid), linTab));
                    }
                }

                return rez;
            }
        }

        /// <summary>
        /// Find AppResults
        /// </summary>
        /// <param name="prods">Productions to use</param>
        /// <param name="mbcty">Concrete type</param>
        /// <param name="f">Name of the function</param>
        /// <returns>List of AppResults</returns>
        private List<AppResult> GetApps(Dictionary<int, HashSet<Production>> prods, ConcreteType mbcty, string f)
        {
            if (mbcty == null)
            {
                if (f.Equals("_V") || f.Equals("_B"))
                {
                    return new List<AppResult>();
                }
                else
                {
                    List<AppResult> rez = new List<AppResult>();
                    foreach (KeyValuePair<int, HashSet<Production>> it in prods)
                    {
                        int fid = it.Key;
                        foreach (Production prod in it.Value)
                        {
                            rez.AddRange(this.ToApp(new ConcreteType("_", fid), prod, f, prods));
                        }
                    }

                    return rez;
                }
            }
            else
            {
                HashSet<Production> setProd;
                List<AppResult> rez = new List<AppResult>();
                if (!prods.TryGetValue(mbcty.FId, out setProd))
                {
                    return rez;
                }
                else
                {
                    foreach (Production prod in setProd)
                    {
                        rez.AddRange(this.ToApp(mbcty, prod, f, prods));
                    }

                    return rez;
                }
            }
        }

        /// <summary>
        /// :D :D :D ? 
        /// </summary>
        /// <param name="cty">Concrete type</param>
        /// <param name="p">Production to use</param>
        /// <param name="f">Function to use</param>
        /// <param name="prods">Productions to use</param>
        /// <returns>List of AppResults</returns>
        private List<AppResult> ToApp(ConcreteType cty, Production p, string f, Dictionary<int, HashSet<Production>> prods)
        {
            List<AppResult> rez = new List<AppResult>();
            if (p is ProductionApply)
            {
                ProductionApply ap = (ProductionApply)p;
                int[] args = ap.Domain();
                CncFun cncFun = ap.Function;
                List<ConcreteType> vtype = new List<ConcreteType>();
                if (f.Equals("_V"))
                {
                    foreach (int j in args)
                    {
                        vtype.Add(new ConcreteType("__gfVar", j));
                    }

                    rez.Add(new AppResult(cncFun, cty, vtype));
                    return rez;
                } 
                else if (f.Equals("_B"))
                {
                    vtype.Add(new ConcreteType(cty.CId, args[0]));
                    for (int i = 1; i < args.Length; i++)
                    {
                        vtype.Add(new ConcreteType("__gfVar", args[i]));
                    }

                    rez.Add(new AppResult(cncFun, cty, vtype));
                    return rez;
                }
                else
                {
                    Grammar.Type t = null;
                    foreach (AbsFun abs in this.pgf.GetAbstract().AbsFuns)
                    {
                        if (f.Equals(abs.Name))
                        {
                            t = abs.Type;
                            break;
                        }
                    }

                    if (t == null)
                    {
                        throw new LinearizerException("Abstract function: " + f + " not found in the abstract syntax");
                    }

                    List<string> catSkel = this.CatSkeleton(t);
                    string res = catSkel.ElementAt(0);
                    for (int i = 0; i < args.Length; i++)
                    {
                        vtype.Add(new ConcreteType(catSkel.ElementAt(i + 1), args[i]));
                    }

                    rez.Add(new AppResult(cncFun, new ConcreteType(res, cty.FId), vtype));
                    return rez;
                }
            }
            else
            {
                HashSet<Production> setProds;
                if (prods.TryGetValue(((ProductionCoerce)p).InitId, out setProds))
                {
                    foreach (Production prod in setProds)
                    {
                        rez.AddRange(this.ToApp(cty, prod, f, prods));
                    }

                    return rez;
                }

                throw new LinearizerException("Couldn't find the production with InitId: " + ((ProductionCoerce)p).InitId);
            }
        }

        /// <summary>
        /// Computes the types of the arguments of a function type
        /// </summary>
        /// <param name="t">Type to use</param>
        /// <returns>List of strings</returns>
        private List<string> CatSkeleton(Grammar.Type t)
        {
            List<string> rez = new List<string> { t.Name };
            foreach (Hypo h in t.Hypos)
            {
                rez.Add(h.Type.Name);
            }

            return rez;
        }

        /// <summary>
        /// Computes the sequence of bracketed tokens associated to a symbol
        /// </summary>
        /// <param name="s">Symbol to use</param>
        /// <param name="cncTypes">List of Concrete types</param>
        /// <param name="linTables">List of list of list of BracketedTokns</param>
        /// <returns>List of BracketedTokn</returns>
        private List<BracketedTokn> Compute(Symbol s, List<ConcreteType> cncTypes, List<List<List<BracketedTokn>>> linTables)
        {
            if (s is ArgConstSymbol)
            {
                return this.GetArg(((ArgConstSymbol)s).Arg, ((ArgConstSymbol)s).Cons, cncTypes, linTables);
            }
            else if (s is AlternToksSymbol)
            {
                return new List<BracketedTokn> { new LeafKP(((AlternToksSymbol)s).Tokens, ((AlternToksSymbol)s).Alts) };
            }
            else if (s is LitSymbol)
            {
                // TODO: Fix? D:
                return this.GetArg(((LitSymbol)s).Arg, ((LitSymbol)s).Cons, cncTypes, linTables);
            }
            else
            {
                return new List<BracketedTokn> { new LeafKS(((ToksSymbol)s).Tokens) };
            }
        }

        /// <summary>
        /// Retrieves a sequence of bracketed tokens from an intermediate result of the linearization 
        /// according to 2 indices from a production.
        /// </summary>
        /// <param name="d">Insert comment here!</param>
        /// <param name="r">Insert comment here?</param>
        /// <param name="cncTypes">List of concrete types</param>
        /// <param name="linTables">List of list of list of BracketedTokns</param>
        /// <returns>List of BracketedTokns</returns>
        private List<BracketedTokn> GetArg(int d, int r, List<ConcreteType> cncTypes, List<List<List<BracketedTokn>>> linTables)
        {
            if (d != 0)
            {
                d = d * (cncTypes.Count / 2);
            }

            if (cncTypes.Count <= d)
            {
                return new List<BracketedTokn>();
            }
            
            List<BracketedTokn> argLin = linTables.ElementAt(d).ElementAt(r);
            if (argLin.Count == 0)
            {
                return argLin;
            }

            return new List<BracketedTokn> { new Bracket(cncTypes.ElementAt(d).CId, cncTypes.ElementAt(d).FId, r, argLin) };
        }

        /// <summary>
        /// Computes a sequence of bracketed tokens from the sequence of symbols of a concrete function
        /// </summary>
        /// <param name="seqId">Sequence id</param>
        /// <param name="cncTypes">List of Concrete types</param>
        /// <param name="linTables">List of list of list of BracketedTokns</param>
        /// <returns>List of BracketedTokns</returns>
        private List<BracketedTokn> ComputeSeq(Symbol[] seqId, List<ConcreteType> cncTypes, List<List<List<BracketedTokn>>> linTables)
        {
            List<BracketedTokn> bt = new List<BracketedTokn>();
            foreach (Symbol sym in seqId)
            {
                bt.AddRange(this.Compute(sym, cncTypes, linTables));
            }

            return bt;
        }

        /// <summary>
        /// Shuffles the results of of the intermediate linearization,
        /// for generating all the possible combinations.
        /// </summary>
        /// <param name="nextfid">Next fid</param>
        /// <param name="cncTypes">List of Concrete types</param>
        /// <param name="exps">List of trees</param>
        /// <param name="xs">List of strings</param>
        /// <returns>List of RezDesc</returns>
        private List<RezDesc> Descend(int nextfid, List<ConcreteType> cncTypes, List<Tree> exps, List<string> xs)
        {
            List<RezDesc> rez = new List<RezDesc>();
            if (exps.Count == 0)
            {
                rez.Add(new RezDesc(nextfid, new List<ConcreteType>(), new List<List<List<BracketedTokn>>>()));
                return rez;
            }

            ConcreteType cncType = cncTypes.First();
            cncTypes.RemoveAt(0);
            Tree exp = exps.First();
            exps.RemoveAt(0);
            List<LinTriple> rezLin = this.Lin0(new List<string>(), xs, cncType, nextfid, exp);
            List<RezDesc> rezDesc = this.Descend(nextfid, cncTypes, exps, xs);
            foreach (LinTriple lin in rezLin)
            {
                foreach (RezDesc res in rezDesc)
                {
                    res.CncTypes.Add(lin.CncType);
                    res.Bracketedtokn.Add(lin.LinTable);
                    rez.Add(new RezDesc(nextfid, res.CncTypes, res.Bracketedtokn));
                }
            }

            return rez;
        }

        /// <summary>
        /// Checks if an integer is the index of a literal
        /// </summary>
        /// <param name="i">Integer to check</param>
        /// <returns>True if integer is a literal</returns>
        private bool IsLiteral(int i)
        {
            // LiteralVar = -4
            // LiteralFloat = -3
            // LiteralInt = -2
            // LiteralString = -1
            return i >= -4 && i <= -1;
        }
    }
}