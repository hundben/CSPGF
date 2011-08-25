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
    public class Linearizer
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
        /// TODO: What is this?
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
        /// Linearize a tree to a list of tokens.
        /// </summary>
        /// <param name="absyn">Tree to linearize</param>
        /// <returns>List of strings</returns>
        public List<string> LinearizeTokens(CSPGF.Trees.Absyn.Tree absyn)
        {
            return this.RenderLin(this.Linearize(absyn).ElementAt(0));
        }

        /// <summary>
        /// Linearize a tree to a string.
        /// </summary>
        /// <param name="absyn">Tree to linearize</param>
        /// <returns>Linearized string</returns>
        public string LinearizeString(CSPGF.Trees.Absyn.Tree absyn)
        {
            List<LinTriple> tmp = this.Linearize(absyn);
            if (tmp.Count == 0)
            {
                throw new LinearizerException("Failed to linearize");
            }

            LinTriple tmp2 = tmp.First<LinTriple>();
            List<string> words = this.RenderLin(tmp2);
            string sb = string.Empty;
            foreach (string w in words) 
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
            Dictionary<int, HashSet<Production>> emptyMap = new Dictionary<int, HashSet<Production>>();
            return this.LinIndex(this.FilterProductions(emptyMap, this.cnc.GetSetOfProductions()));
        }

        /// <summary>
        /// Aligns the indexes for the l-productions
        /// </summary>
        /// <param name="productions">Productions to align</param>
        /// <returns>Aligned l-productions</returns>
        private Dictionary<string, Dictionary<int, HashSet<Production>>> LinIndex(Dictionary<int, HashSet<Production>> productions)
        {
            Dictionary<string, Dictionary<int, HashSet<Production>>> vtemp = new Dictionary<string, Dictionary<int, HashSet<Production>>>();
            foreach (KeyValuePair<int, HashSet<Production>> i in productions) 
            {
                int res = i.Key;
                foreach (Production prod in i.Value) 
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
                                    obj.Remove(res);
                                    obj.Add(res, ttemp);
                                    vtemp.Remove(str);
                                    vtemp.Add(str, obj);
                                } 
                                else 
                                {
                                    obj.Add(res, singleton);
                                    vtemp.Remove(str);
                                    vtemp.Add(str, obj);
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
                int fid = ((CoerceProduction)p).InitId;
                HashSet<Production> prods;
                if (!productions.TryGetValue(fid, out prods)) 
                {
                    return new List<string>();
                } 
                else 
                {
                    foreach (Production pp in prods)
                    {
                        List<string> vrez = this.GetFunctions(pp, productions);
                        if (vrez.Count != 0) 
                        {
                            foreach (string str in vrez) 
                            {
                                rez.Add(str);
                            }
                        }
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
            foreach (int index in prods.Keys)
            {
                HashSet<Production> setProd = prods[index];
                HashSet<Production> intermRez = this.FilterProdSet1(prods0, setProd);
                if (intermRez.Count != 0)
                {
                    tempRez.Add(index, intermRez);
                }
            }

            Dictionary<int, HashSet<Production>> prods1 = new Dictionary<int,HashSet<Production>>();
            
            foreach (KeyValuePair<int, HashSet<Production>> cp in prods0) {
                prods1.Add(cp.Key, cp.Value);
            }
            
            foreach (KeyValuePair<int, HashSet<Production>> kvp in tempRez)
            {
                int index = kvp.Key;
                HashSet<Production> hp = kvp.Value;
                if (prods0.ContainsKey(index))
                {
                    if (!this.HashEquals(prods0[index], hp))
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
        /// Checks if two HashMaps containing Productions are equal
        /// </summary>
        /// <param name="set1">First set to compare</param>
        /// <param name="set2">Secons set to compare</param>
        /// <returns>True if equal</returns>
        private bool HashEquals(HashSet<Production> set1, HashSet<Production> set2)
        {
            foreach (Production p in set1)
            {
                if (!set2.Contains(p))
                {
                    return false;
                }
            }

            return true;
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

            throw new LinearizerException("Filter-fail");
        }
        
        /// <summary>
        /// Checks if a production just has a variable argument
        /// </summary>
        /// <param name="p">Production to check</param>
        /// <returns>True if true</returns>
        private bool Is_ho_prod(Production p)
        {
            if (p is ApplProduction) 
            {
                List<int> args = ((ApplProduction)p).Domain();
                if (args.Count == 1 && args[0] == -4)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets list of forest ids from the categories in ho_cats
        /// </summary>
        /// <returns>Set of integers</returns>
        private HashSet<int> Ho_fids()
        {
            HashSet<int> rezTemp = new HashSet<int>();
            List<string> hocats = this.Ho_cats();
            CncCat[] cncCats = this.cnc.CncCats.Values.ToArray<CncCat>();
            foreach (string hc in hocats) 
            {
                foreach (CncCat cncs in cncCats) 
                {
                    if (hc.Equals(cncs.Name)) 
                    {
                        for (int ind = cncs.FirstFID; ind <= cncs.LastFID; ind++) 
                        {
                            rezTemp.Add(ind);
                        }  
                    }
                }
            }

            return rezTemp;
        }

        /// <summary>
        /// Get all names of types from Concrete
        /// </summary>
        /// <returns>List of strings</returns>
        private List<string> Ho_cats()
        {
            List<string> rezTemp = new List<string>();
            Abstract abstr = this.pgf.GetAbstract();
            List<AbsFun> absFuns = abstr.AbsFuns;
            foreach (AbsFun af in absFuns) 
            {
                List<Hypo> hypos = af.Type.Hypos;
                foreach (Hypo hypo in hypos) 
                {
                    if (!rezTemp.Contains(hypo.Type.Name)) 
                    {
                        rezTemp.Add(hypo.Type.Name);
                    }
                }
            }

            return rezTemp;
        }
        
        /// <summary>
        /// Gets the types from the hypotheses of a type
        /// </summary>
        /// <param name="t">Type to check for</param>
        /// <returns>List of strings</returns>
        private List<string> HypoArgsOfType(CSPGF.Reader.Type t)
        {
            List<Hypo> hypos = t.Hypos;
            List<string> tmp = new List<string>();
            foreach (Hypo h in hypos) 
            {
                tmp.Add(h.Type.Name);
            }

            return tmp;
        }

        /// <summary>
        /// Flattens a bracketed token
        /// </summary>
        /// <param name="bt">BracketedTokn to use</param>
        /// <param name="after">String to use</param>
        /// <returns>List of strings</returns>
        private List<string> Untokn(BracketedTokn bt, string after)
        {
            if (bt is LeafKS) 
            {
                List<string> d = ((LeafKS)bt).Tokens;
                List<string> rez = new List<string>();
                for (int i = d.Count - 1; i >= 0; i--)
                {
                    rez.Add(d[i]);
                }

                return rez;
            } 
            else if (bt is LeafKP) 
            {
                List<string> d = ((LeafKP)bt).DefaultTokens;
                List<Alternative> alts = ((LeafKP)bt).Alternatives;
                List<string> rez = new List<string>();
                foreach (Alternative alt in alts) 
                {
                    List<string> ss2 = alt.Alt2;
                    foreach (string str in ss2) 
                    {
                        if (after.StartsWith(str)) 
                        {
                            List<string> ss1 = alt.Alt1;
                            for (int k = ss1.Count - 1; k >= 0; k--)
                            {
                                rez.Add(ss1[k]);
                            }

                            ss1.Reverse();
                            return rez;
                        }
                    }
                }

                for (int i = d.Count - 1; i >= 0; i--) 
                {
                    rez.Add(d[i]);
                }

                return rez;
            } 
            else 
            {
                List<string> rez = new List<string>();
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
            List<string> rezF = new List<string>();
            List<List<BracketedTokn>> vtemp = v.LinTable;
            string after = string.Empty;
            for (int k = vtemp.ElementAt(0).Count - 1; k >= 0; k--) 
            {
                rez.AddRange(this.Untokn(vtemp.ElementAt(0).ElementAt(k), after));
                after = rez.Last();
            }

            foreach (string str in rez) 
            {
                rezF.Insert(0, str);
            }

            return rezF;
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
        private List<LinTriple> Linearize(CSPGF.Trees.Absyn.Tree e)
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
        private List<LinTriple> Lin0(List<string> xs, List<string> ys, CncType mbcty, int mbfid, CSPGF.Trees.Absyn.Tree tree)
        {
            // if tree is a lambda, we add the variable to the list of bound
            // variables and we linearize the subtree.
            if (tree is CSPGF.Trees.Absyn.Lambda) 
            {
                xs.Add(((CSPGF.Trees.Absyn.Lambda)tree).Ident_);
                List<LinTriple> tmp = this.Lin0(xs, ys, mbcty, mbfid, ((CSPGF.Trees.Absyn.Lambda)tree).Tree_);
                return tmp;
            } 
            else if (xs.Count == 0) 
            {
                List<CSPGF.Trees.Absyn.Tree> es = new List<CSPGF.Trees.Absyn.Tree>();
                if (tree is CSPGF.Trees.Absyn.Application) 
                {
                    do 
                    {
                        es.Add(((CSPGF.Trees.Absyn.Application)tree).Tree_2);
                        tree = ((CSPGF.Trees.Absyn.Application)tree).Tree_1;
                    } 
                    while (tree is CSPGF.Trees.Absyn.Application);
                }

                if (tree is Function) 
                {
                    List<LinTriple> tmp = this.Apply(xs, mbcty, mbfid, ((Function)tree).Ident_, es);
                    
                    return tmp;
                } 
                else 
                {
                    // RuntimeException -> Exception
                    throw new Exception("Undefined construction for expressions !!!");
                }
            } 
            else 
            {
                xs.AddRange(ys);
                List<CSPGF.Trees.Absyn.Tree> exprs = new List<CSPGF.Trees.Absyn.Tree>();
                exprs.Add(tree);
                foreach (string str in xs)
                {
                    exprs.Add(new CSPGF.Trees.Absyn.Literal(new CSPGF.Trees.Absyn.StringLiteral(str)));
                } 

                return this.Apply(xs, mbcty, mbfid, "_B", exprs);
            }
        }

        /// <summary>
        /// Intermediate linearization for complex expressions. 
        /// </summary>
        /// To linearize the application of the function "f" to the arguments (trees) a, b and c use : apply(???,???,??? "f", [a,b,c]).
        /// 'apply' will linearize the argument and then use the concrete function for "f" to glue them together.
        /// <param name="xs">Insert comment here?</param>
        /// <param name="mbcty">Insert comment here</param>
        /// <param name="nextfid">Insert comment</param>
        /// <param name="f">Name of the function to be applied</param>
        /// <param name="es">The argiment of the function to linearize</param>
        /// <returns>All possible linearizations for the application of f to es</returns>
        private List<LinTriple> Apply(List<string> xs, CncType mbcty, int nextfid, string f, List<CSPGF.Trees.Absyn.Tree> es)
        {
            Dictionary<int, HashSet<Production>> prods;
            if (!this.linProd.TryGetValue(f, out prods)) 
            {
                List<CSPGF.Trees.Absyn.Tree> newes = new List<CSPGF.Trees.Absyn.Tree>();
                newes.Add(new CSPGF.Trees.Absyn.Literal(new CSPGF.Trees.Absyn.StringLiteral(f)));
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
                        throw new LinearizerException("lengths of es and ctys don't match" + es.ToString() + " -- " + ctys.ToString());
                    }

                    List<Sequence> lins = appr.CncFun.Sequences;
                    string cat = appr.CncType.CId;
                    List<CSPGF.Trees.Absyn.Tree> copy_expr = new List<CSPGF.Trees.Absyn.Tree>();
                    foreach (Tree tree in es)
                    {
                        copy_expr.Add(tree);
                    }

                    List<RezDesc> rezDesc = this.Descend(nextfid, ctys, copy_expr, xs);
                    foreach (RezDesc rez2 in rezDesc)
                    {
                        RezDesc intRez = rez2;
                        List<List<BracketedTokn>> linTab = new List<List<BracketedTokn>>();
                        foreach (Sequence seq in lins)
                        {
                            linTab.Add(this.ComputeSeq(seq, intRez.CncTypes, intRez.Bracketedtokn));
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
                        // Iterator<Entry<Integer, HashSet<Production>>> it = prods.entrySet().iterator();
                        // while (it.hasNext()) {
                        // Entry<Integer, HashSet<Production>> en = it.next();
                        int fid = it.Key;
                        foreach (Production prod in it.Value) 
                        {
                            // Iterator<Production> ip = en.getValue().iterator();
                            // while (ip.hasNext()) {
                            List<AppResult> appR = this.ToApp(new CncType("_", fid), prod, f, prods);
                            foreach (AppResult app in appR) 
                            {
                                rez.Add(app);
                            }
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
                    return new List<AppResult>();
                }
                else
                {
                    foreach (Production prod in setProd)
                    {
                        foreach (AppResult app in this.ToApp(mbcty, prod, f, prods))
                        {
                            rez.Add(app);
                        }
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
                List<int> args = ((ApplProduction)p).Domain();
                CncFun cncFun = ((ApplProduction)p).Function;
                List<CncType> vtype = new List<CncType>();
                if (f.Equals("V")) 
                {
                    foreach (int i in args)
                    {
                        vtype.Add(new CncType("__gfVar", args[i]));
                    }

                    rez.Add(new AppResult(cncFun, cty, vtype));
                    return rez;
                }

                if (f.Equals("_B")) 
                {
                    vtype.Add(new CncType(cty.CId, args[0]));
                    for (int i = 1; i < args.Count; i++) 
                    {
                        vtype.Add(new CncType("__gfVar", args[i]));
                    }

                    rez.Add(new AppResult(cncFun, cty, vtype));
                    return rez;
                } 
                else 
                {
                    List<AbsFun> absFuns = this.pgf.GetAbstract().AbsFuns;
                    CSPGF.Reader.Type t = null;
                    foreach (AbsFun abs in absFuns)
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
                    for (int i = 0; i < args.Count; i++) 
                    {
                        vtype.Add(new CncType(catSkel.ElementAt(i + 1), args[i]));
                    }

                    rez.Add(new AppResult(cncFun, new CncType(res, cty.FId), vtype));
                    return rez;
                }
            } 
            else 
            {
                int fid = ((CoerceProduction)p).InitId;
                HashSet<Production> setProds = prods[fid];
                foreach (Production prod in setProds) 
                {
                    foreach (AppResult app in this.ToApp(cty, prod, f, prods))
                    {
                        rez.Add(app);
                    }
                }

                return rez;
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
            List<Hypo> hypos = t.Hypos;
            foreach (Hypo h in hypos) 
            {
                rez.Add(h.Type.Name);
            }

            return rez;
        }

        /// <summary>
        /// Creates a simple vector of vectors of bracketed tokens containing a string value
        /// </summary>
        /// <param name="s">String to use</param>
        /// <returns>List of list of BracketedTokns</returns>
        private List<List<BracketedTokn>> SS(string s)
        {
            List<List<BracketedTokn>> bt = new List<List<BracketedTokn>>();
            List<BracketedTokn> v = new List<BracketedTokn>();
            List<string> sts = new List<string>();
            sts.Add(s);
            v.Add(new LeafKS(sts));
            bt.Add(v);
            return bt;
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
                List<string> toks = ((AlternToksSymbol)s).Tokens;
                List<Alternative> alts = ((AlternToksSymbol)s).Alts;
                List<BracketedTokn> v = new List<BracketedTokn>();
                v.Add(new LeafKP(toks, alts));
                return v;
            } 
            else 
            {
                List<string> toks = ((ToksSymbol)s).Tokens;
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
            if (cncTypes.Count <= d)
            {
                return new List<BracketedTokn>();
            }

            CncType cncType = cncTypes.ElementAt(d);
            List<List<BracketedTokn>> lin = linTables.ElementAt(d);
            string cat = cncType.CId;
            int fid = cncType.FId;
            List<BracketedTokn> arg_lin = lin.ElementAt(r);
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
        private List<BracketedTokn> ComputeSeq(Sequence seqId, List<CncType> cncTypes, List<List<List<BracketedTokn>>> linTables)
        {
            List<BracketedTokn> bt = new List<BracketedTokn>();
            List<Symbol> symbs = seqId.Symbs;
            foreach (Symbol sym in symbs)
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
        private List<RezDesc> Descend(int nextfid, List<CncType> cncTypes, List<CSPGF.Trees.Absyn.Tree> exps, List<string> xs)
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
                CSPGF.Trees.Absyn.Tree exp = exps.First();
                exps.RemoveAt(0);
                List<LinTriple> rezLin = this.Lin0(new List<string>(), xs, cncType, nextfid, exp);
                List<RezDesc> rezDesc = this.Descend(nextfid, cncTypes, exps, xs);
                foreach (LinTriple lin in rezLin)
                {
                    foreach (RezDesc res in rezDesc)
                    {
                        CncType c = lin.CncType;
                        List<CncType> vcnc = res.CncTypes;
                        vcnc.Add(c);
                        List<List<List<BracketedTokn>>> vbt = res.Bracketedtokn;
                        List<List<BracketedTokn>> bt = lin.LinTable;
                        vbt.Add(bt);
                        rez.Add(new RezDesc(nextfid, vcnc, vbt));
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
            else
            {
                return false;
            }
        }
    }
}