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

namespace CSPGF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CSPGF.Linearize;
    using CSPGF.Reader;
    using CSPGF.Trees.Absyn;

    /// <summary>
    /// Linearizer for use with the Parser
    /// </summary>
    internal class Linearizer
    {
        /// <summary>
        /// The current PGF-file
        /// </summary>
        private PGF pgf;

        /// <summary>
        /// The current Concrete grammar
        /// </summary>
        private Concrete cnc;

        /// <summary>
        /// Linearization productions
        /// </summary>
        private Dictionary<string, Dictionary<int, HashSet<Production>>> linProd;

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
        /// Linearizes a tree into a list of tokens.
        /// </summary>
        /// <param name="absyn">Tree to linearize</param>
        /// <returns>List of strings</returns>
        public List<string> LinearizeAll(Tree absyn)
        {
            List<List<string>> tmp = this.RenderAllLins(this.Linearize(absyn));
            List<string> tmp2 = new List<string>();
            foreach (List<string> lstr in tmp)
            {
                tmp2.AddRange(lstr);
                tmp2.Add("\n");
            }

            return tmp2;
        }

        /// <summary>
        /// Linearize a tree to a string.
        /// </summary>
        /// <param name="absyn">Tree to linearize</param>
        /// <returns>Linearized string</returns>
        public string LinearizeString(Tree absyn)
        {
            string sb = string.Empty;
            foreach (string w in this.LinearizeTokens(absyn))
            {
                sb += w + " ";
            }

            return sb.Trim();
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
                int res = kvp.Key;
                foreach (Production prod in kvp.Value)
                {
                    List<string> vs = this.GetFunctions(prod, productions);
                    if (vs.Count != 0)
                    {
                        foreach (string str in vs)
                        {
                            Dictionary<int, HashSet<Production>> htemp = new Dictionary<int, HashSet<Production>>();
                            HashSet<Production> singleton = new HashSet<Production>();
                            singleton.Add(prod);
                            htemp.Add(res, singleton);
                            if (vtemp.ContainsKey(str))
                            {
                                Dictionary<int, HashSet<Production>> obj = vtemp[str];
                                if (obj.ContainsKey(res))
                                {
                                    HashSet<Production> ttemp = obj[res];
                                    ttemp.Add(prod);
                                    vtemp[str] = obj;
                                }
                                else
                                {
                                    obj.Add(res, singleton);
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
            if (p is ApplProduction)
            {
                rez.Add(((ApplProduction)p).Function.Name);
            }
            else
            {
                HashSet<Production> prods;
                if (productions.TryGetValue(((CoerceProduction)p).InitId, out prods))
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
            HashSet<Production> set1 = new HashSet<Production>();
            foreach (Production prod in set)
            {
                if (this.FilterRule(prods0, prod))
                {
                    set1.Add(prod);
                }
            }

            return set1;
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
            bool are_diff = false;

            foreach (KeyValuePair<int, HashSet<Production>> kvp in prods)
            {
                HashSet<Production> setProd = kvp.Value;
                HashSet<Production> intermRez = this.FilterProdSet1(prods0, setProd);
                if (intermRez.Count != 0)
                {
                    tempRez.Add(kvp.Key, intermRez);
                }
            }

            Dictionary<int, HashSet<Production>> prods1 = new Dictionary<int, HashSet<Production>>(prods0);

            foreach (KeyValuePair<int, HashSet<Production>> kvp in tempRez)
            {
                int index = kvp.Key;
                HashSet<Production> hp = kvp.Value;
                if (prods0.ContainsKey(index))
                {
                    if (!prods0[index].SetEquals(hp))
                    {
                        foreach (Production p in prods0[index])
                        {
                            hp.Add(p);
                        }

                        prods1[index] = hp;
                        are_diff = true;
                    }
                }
                else
                {
                    prods1[index] = hp;
                    are_diff = true;
                }
            }

            if (are_diff)
            {
                return this.FilterProductions(prods1, prods);
            }
            else
            {
                return prods0;
            }
        }

        /// <summary>
        /// Checks if a production satisfies conditionProd recursively
        /// </summary>
        /// <param name="prods">Productions to check</param>
        /// <param name="p">Production to use</param>
        /// <returns>True if true</returns>
        private bool FilterRule(Dictionary<int, HashSet<Production>> prods, Production p)
        {
            if (p is ApplProduction)
            {
                ApplProduction ap = (ApplProduction)p;
                foreach (int i in ap.Domain())
                {
                    if (!this.ConditionProd(i, prods))
                    {
                        return false;
                    }
                }

                return true;
            }

            if (p is CoerceProduction)
            {
                return this.ConditionProd(((CoerceProduction)p).InitId, prods);
            }

            throw new LinearizerException("The production wasn't either an ApplProduction or a CoerceProduction");
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
                string[] d = ((LeafKS)bt).Tokens;
                for (int i = d.Length - 1; i >= 0; i--)
                {
                    rez.Add(d[i]);
                }

                return rez;
            }
            else if (bt is LeafKP)
            {
                string[] d = ((LeafKP)bt).DefaultTokens;
                foreach (Alternative alt in ((LeafKP)bt).Alternatives)
                {
                    string[] ss2 = alt.Alt2;
                    foreach (string str in ss2)
                    {
                        if (after.StartsWith(str))
                        {
                            string[] ss1 = alt.Alt1;
                            for (int k = ss1.Length - 1; k >= 0; k--)
                            {
                                rez.Add(ss1[k]);
                            }

                            return rez;
                        }
                    }
                }

                for (int i = d.Length - 1; i >= 0; i--)
                {
                    rez.Add(d[i]);
                }

                return rez;
            }
            else
            {
                List<BracketedTokn> bs = ((Bracket)bt).Bracketedtoks;
                for (int i = bs.Count - 1; i >= 0; i--)
                {
                    rez.AddRange(this.Untokn(bs.ElementAt(i), after));
                    after = rez.Last();
                }

                return rez;
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
        /// Flatten a list of LinTriples
        /// </summary>
        /// <param name="v">List to flatten</param>
        /// <returns>List of list of strings</returns>
        private List<List<string>> RenderAllLins(List<LinTriple> v)
        {
            List<List<string>> rez = new List<List<string>>();
            foreach (LinTriple lt in v)
            {
                rez.Add(this.RenderLin(lt));
            }

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
        /// <param name="mbfid">First id</param>
        /// <param name="tree">Tree to linearize</param>
        /// <returns>List of all possible linearized tuples</returns>
        private List<LinTriple> Lin0(List<string> xs, List<string> ys, CncType mbcty, int mbfid, Tree tree)
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
                List<Tree> exprs = new List<Tree>();
                exprs.Add(tree);
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
        private List<LinTriple> Apply(List<string> xs, CncType mbcty, int nextfid, string f, List<Tree> es)
        {
            Dictionary<int, HashSet<Production>> prods;
            if (!this.linProd.TryGetValue(f, out prods))
            {
                List<Tree> newes = new List<Tree>();
                newes.Add(new Literal(new StringLiteral(f)));
                System.Console.WriteLine("Function " + f + " does not have a linearization !");
                return this.Apply(xs, mbcty, nextfid, "_V", newes);
            }
            else
            {
                List<AppResult> listApp = this.GetApps(prods, mbcty, f);
                List<LinTriple> rez = new List<LinTriple>();
                foreach (AppResult appr in listApp)
                {
                    List<CncType> copy_ctys = appr.CncTypes;
                    List<CncType> ctys = new List<CncType>();
                    for (int ind = copy_ctys.Count - 1; ind >= 0; ind--)
                    {
                        ctys.Add(copy_ctys.ElementAt(ind));
                    }

                    if (es.Count != ctys.Count)
                    {
                        throw new LinearizerException("Lengths of es and ctys don't match" + es.ToString() + " -- " + ctys.ToString());
                    }

                    Symbol[][] lins = appr.CncFun.Sequences;
                    string cat = appr.CncType.CId;
                    List<Tree> copy_expr = new List<Tree>(es);
                    List<RezDesc> rezDesc = this.Descend(nextfid, ctys, copy_expr, xs);
                    foreach (RezDesc rez2 in rezDesc)
                    {
                        List<List<BracketedTokn>> linTab = new List<List<BracketedTokn>>();
                        foreach (Symbol[] seq in lins)
                        {
                            linTab.Add(this.ComputeSeq(seq, rez2.CncTypes, rez2.Bracketedtokn));
                        }

                        rez.Add(new LinTriple(nextfid + 1, new CncType(cat, nextfid), linTab));
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
        private List<AppResult> GetApps(Dictionary<int, HashSet<Production>> prods, CncType mbcty, string f)
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
                            rez.AddRange(this.ToApp(new CncType("_", fid), prod, f, prods));
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
        private List<AppResult> ToApp(CncType cty, Production p, string f, Dictionary<int, HashSet<Production>> prods)
        {
            List<AppResult> rez = new List<AppResult>();
            if (p is ApplProduction)
            {
                int[] args = ((ApplProduction)p).Domain();
                CncFun cncFun = ((ApplProduction)p).Function;
                List<CncType> vtype = new List<CncType>();
                if (f.Equals("_V"))
                {
                    for (int i = 0; i < args.Length; i++)
                    {
                        vtype.Add(new CncType("__gfVar", args[i]));
                    }

                    rez.Add(new AppResult(cncFun, cty, vtype));
                    return rez;
                } 
                else if (f.Equals("_B"))
                {
                    vtype.Add(new CncType(cty.CId, args[0]));
                    for (int i = 1; i < args.Length; i++)
                    {
                        vtype.Add(new CncType("__gfVar", args[i]));
                    }

                    rez.Add(new AppResult(cncFun, cty, vtype));
                    return rez;
                }
                else
                {
                    CSPGF.Reader.Type t = null;
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
                        throw new LinearizerException(" f not found in the abstract syntax");
                    }

                    List<string> catSkel = this.CatSkeleton(t);
                    string res = catSkel.ElementAt(0);
                    for (int i = 0; i < args.Length; i++)
                    {
                        vtype.Add(new CncType(catSkel.ElementAt(i + 1), args[i]));
                    }

                    rez.Add(new AppResult(cncFun, new CncType(res, cty.FId), vtype));
                    return rez;
                }
            }
            else
            {
                HashSet<Production> setProds;
                if (prods.TryGetValue(((CoerceProduction)p).InitId, out setProds))
                {

                    foreach (Production prod in setProds)
                    {
                        rez.AddRange(this.ToApp(cty, prod, f, prods));
                    }

                    return rez;
                }
                else
                {
                    throw new LinearizerException("Couldn't find the production with InitId: " + ((CoerceProduction)p).InitId);
                }
            }
        }

        /// <summary>
        /// Computes the types of the arguments of a function type
        /// </summary>
        /// <param name="t">Type to use</param>
        /// <returns>List of strings</returns>
        private List<string> CatSkeleton(CSPGF.Reader.Type t)
        {
            List<string> rez = new List<string>();
            rez.Add(t.Name);
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
        private List<BracketedTokn> Compute(Symbol s, List<CncType> cncTypes, List<List<List<BracketedTokn>>> linTables)
        {
            if (s is ArgConstSymbol)
            {
                int arg = ((ArgConstSymbol)s).Arg;
                int cons = ((ArgConstSymbol)s).Cons;
                return this.GetArg(arg, cons, cncTypes, linTables);
            }
            else if (s is AlternToksSymbol)
            {
                string[] toks = ((AlternToksSymbol)s).Tokens;
                Alternative[] alts = ((AlternToksSymbol)s).Alts;
                List<BracketedTokn> v = new List<BracketedTokn>();
                v.Add(new LeafKP(toks, alts));
                return v;
            }
            else
            {
                string[] toks = ((ToksSymbol)s).Tokens;
                List<BracketedTokn> v = new List<BracketedTokn>();
                v.Add(new LeafKS(toks));
                return v;
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
        private List<BracketedTokn> GetArg(int d, int r, List<CncType> cncTypes, List<List<List<BracketedTokn>>> linTables)
        {
            if (d != 0)
            {
                d = d * (cncTypes.Count / 2);
            }
            if (cncTypes.Count <= d)
            {
                return new List<BracketedTokn>();
            }

            CncType cncType = cncTypes.ElementAt(d);
            string cat = cncType.CId;
            int fid = cncType.FId;
            List<BracketedTokn> arg_lin = linTables.ElementAt(d).ElementAt(r);
            if (arg_lin.Count == 0)
            {
                return arg_lin;
            }

            List<BracketedTokn> bt = new List<BracketedTokn>();
            bt.Add(new Bracket(cat, fid, r, arg_lin));
            return bt;
        }

        /// <summary>
        /// Computes a sequence of bracketed tokens from the sequence of symbols of a concrete function
        /// </summary>
        /// <param name="seqId">Sequence id</param>
        /// <param name="cncTypes">List of Concrete types</param>
        /// <param name="linTables">List of list of list of BracketedTokns</param>
        /// <returns>List of BracketedTokns</returns>
        private List<BracketedTokn> ComputeSeq(Symbol[] seqId, List<CncType> cncTypes, List<List<List<BracketedTokn>>> linTables)
        {
            List<BracketedTokn> bt = new List<BracketedTokn>();
            foreach (Symbol sym in seqId)
            {
                foreach (BracketedTokn btn in this.Compute(sym, cncTypes, linTables))
                {
                    bt.Add(btn);
                }
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
        private List<RezDesc> Descend(int nextfid, List<CncType> cncTypes, List<Tree> exps, List<string> xs)
        {
            List<RezDesc> rez = new List<RezDesc>();
            if (exps.Count == 0)
            {
                rez.Add(new RezDesc(nextfid, new List<CncType>(), new List<List<List<BracketedTokn>>>()));
                return rez;
            }
            else
            {
                CncType cncType = cncTypes.First();
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
            if (i >= -4 && i <= -1)
            {
                return true;
            }

            return false;
        }

        /****************************************DEBUG ONLY*************************************************/
        private string PrintProduction(Production prod)
        {
            string tmp = string.Empty;
            if (prod is ApplProduction)
            {
                ApplProduction pro = (ApplProduction)prod;
                tmp += "FID: " + pro.FId + " CncFun: " + pro.Function.Name + " Sel: " + pro.Sel;
            }
            else if (prod is CoerceProduction)
            {
                CoerceProduction pro = (CoerceProduction)prod;
                tmp += "FID: " + pro.FId + " InitID: " + pro.InitId + " Sel:" + pro.Sel;
            }

            return tmp;
        }

        private string PrintProductionSet(HashSet<Production> prods)
        {
            string tmp = string.Empty;
            foreach (Production prod in prods)
            {
                tmp += this.PrintProduction(prod) + " ";
            }

            return tmp;
        }

        private string PrintListString(List<string> strs)
        {
            string tmp = string.Empty; 
            foreach (string str in strs)
            {
                tmp += str;
            }

            return tmp;
        }

        private string PrintListString(string[] strs)
        {
            string tmp = string.Empty;
            foreach (string str in strs)
            {
                tmp += str;
            }

            return tmp;
        }

        private string PrintBracketedTokn(BracketedTokn tokn)
        {
            if (tokn is Bracket)
            {
                Bracket tok = (Bracket)tokn;
                return "CID: " + tok.CId + " FID: " + tok.FId + " LIndex: " + tok.LIndex + " (" + this.PrintListBracketedTokn(tok.Bracketedtoks) + " ) ";
            }
            else if (tokn is LeafKP)
            {
                LeafKP leaf = (LeafKP)tokn;
                return "Tokens: " + this.PrintListString(leaf.DefaultTokens) + " Alts: " + this.PrintAlternatives(leaf.Alternatives);
            }
            else if (tokn is LeafKS)
            {
                LeafKS leaf = (LeafKS)tokn;
                return "Tokens: " + this.PrintListString(leaf.Tokens);
            }

            throw new Exception("Failade att skriva ut bracketed tokn: " + tokn.ToString());
        }

        private string PrintListBracketedTokn(List<BracketedTokn> toks)
        {
            string tmp = string.Empty;
            foreach (BracketedTokn tok in toks)
            {
                tmp += this.PrintBracketedTokn(tok);
            }

            return tmp;
        }

        private string PrintAlternatives(Alternative[] alts)
        {
            string tmp = string.Empty;
            foreach (Alternative alt in alts)
            {
                tmp += "Alt1: " + this.PrintListString(alt.Alt1) + " Alt2: " + this.PrintListString(alt.Alt2) + " ";
            }

            return tmp;
        }

        private string PrintLinTriple(LinTriple lintri)
        {
            string tmp = "FID: " + lintri.FId + " Cnctype cid: " + lintri.CncType.CId + " Cnctype fid: " + lintri.FId;
            foreach (List<BracketedTokn> list in lintri.LinTable)
            {
                foreach (BracketedTokn tokn in list)
                {
                    tmp += this.PrintBracketedTokn(tokn) + " ";
                }
            }

            return tmp;
        }

        private string PrintCncType(CncType type)
        {
            return "CID: " + type.CId + " FID: " + type.FId;
        }

        private string PrintListCncType(List<CncType> cnctypes)
        {
            string tmp = string.Empty;
            foreach (CncType type in cnctypes)
            {
                tmp += this.PrintCncType(type) + "\n";
            }

            return tmp;
        }
    }
}