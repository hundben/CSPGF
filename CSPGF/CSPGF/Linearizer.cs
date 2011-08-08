/*
Copyright (c) 2011, Christian Ståhlfors (christian.stahlfors@gmail.com), Erik Bergström (erktheorc@gmail.com)
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

namespace CSPGF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CSPGF.Reader;
    // using CSPGF.trees;
    using CSPGF.Linearizer;
    using CSPGF.Trees.Absyn;

    class Linearizer
    {
        private PGF pgf;
        private Concrete cnc;
        private Dictionary<string, Dictionary<int, HashSet<Production>>> lProd;

        /** linearizes an expression to a bracketed token
         * and further on to a string
         * not implemented to dependent categories, implicit argument,
         * and higher-order abstract syntax
         * @param pgf the pgf object that contains the concrete grammar.
         * @param concrete the concrete grammar to use.
         **/
        public Linearizer(PGF _pgf, Concrete _concrete)
        {
            this.pgf = _pgf;
            this.cnc = _concrete;
            this.lProd = this.GetLProductions();
        }

        /** linearizes an expression to a bracketed token
         * and further on to a string
         * not implemented to dependent categories, implicit argument,
         * and higher-order abstract syntax
         * @param pgf the pgf object that contains the concrete grammar.
         * @param concrete the name of the concrete grammar to use.
         **/
        public Linearizer(PGF _pgf, string _concrete)
        {
            this.pgf = _pgf;
            this.pgf.GetConcrete(_concrete);
            // this(pgf, pgf.concrete(concrete));
        }

        /**
         * Linearize a tree to a vector of tokens.
         **/
        public List<string> LinearizeTokens(CSPGF.Trees.Absyn.Tree absyn)
        {
            return this.RenderLin(this.Linearize(absyn).ElementAt(0));
        }

        /**
         * Linearize a tree to a string.
         **/
        public string LinearizeString(CSPGF.Trees.Absyn.Tree absyn)
        {
            List<string> words = this.RenderLin(this.Linearize(absyn).ElementAt(0));
            string sb = string.Empty;
            foreach (string w in words) {
                sb += w + " ";
            }
        return sb.Trim();
        }

        /** constructs the l-productions of the concrete syntax for
         * a given language
         **/
        private Dictionary<string, Dictionary<int, HashSet<Production>>> GetLProductions()
        {
            Dictionary<int, HashSet<Production>> emptyMap = new Dictionary<int, HashSet<Production>>();
            return this.LinIndex(this.FilterProductions(emptyMap, this.cnc.GetSetOfProductions()));
        }

        /** aligns the indexes for the l-productions
     **/
        private Dictionary<string, Dictionary<int, HashSet<Production>>> LinIndex(Dictionary<int, HashSet<Production>> productions)
        {
            Dictionary<string, Dictionary<int, HashSet<Production>>> vtemp = new Dictionary<string, Dictionary<int, HashSet<Production>>>();
            // Iterator<Entry<Integer, HashSet<Production>>> i = productions.entrySet().iterator();
            foreach (KeyValuePair<int, HashSet<Production>> i in productions) {
                int res = i.Key;
                foreach (Production prod in i.Value) {
                    List<string> vs = this.GetFunctions(prod, productions);
                    if (vs != null) {
                        foreach (string str in vs) {
                            Dictionary<int, HashSet<Production>> htemp = new Dictionary<int, HashSet<Production>>();
                            HashSet<Production> hSingleton = new HashSet<Production>();
                            hSingleton.Add(prod);
                            string newl = str;
                            htemp.Add(res, hSingleton);
                            if (vtemp.ContainsKey(newl)) {
                                Dictionary<int, HashSet<Production>> obj = vtemp[newl];
                                if (obj.ContainsKey(res)) {
                                    HashSet<Production> ttemp = obj[res];
                                    ttemp.Add(prod);
                                    obj.Remove(res);
                                    obj.Add(res, ttemp);
                                    vtemp.Remove(newl);
                                    vtemp.Add(newl, obj);
                                } 
                                else 
                                {
                                    obj.Add(res, hSingleton);
                                    vtemp.Remove(newl);
                                    vtemp.Add(newl, obj);
                                }

                            } 
                            else 
                            {
                                vtemp.Add(newl, htemp);
                            }
                        }
                    }
                }
            }
            return vtemp;
        }



        /** This function computes the list of abstract function corresponding to
         * a given production. This is easy for standard productions but less for
         * coercions because then you have to search reccursively.
         * @param p the production
         * @param productions ???
         **/
        private List<string> GetFunctions(Production p, Dictionary<int, HashSet<Production>> productions)
        {
            List<string> rez = new List<string>();
            if (p is ApplProduction) {
                rez.Add(((ApplProduction)p).function.name);
            } 
            else 
            { // coercion 
                int fid = ((CoerceProduction)p).initId;
                HashSet<Production> prods = productions[fid];
                if (prods == null) 
                {
                    return null;
                } 
                else 
                {
                    foreach (Production pp in prods) {
                        List<string> vrez = this.GetFunctions((Production)pp, productions);
                        if (vrez != null) {
                            foreach (string str in vrez) {
                                rez.Add(str);
                            }
                        }
                    }
                }
            }
            if (rez.Count == 0 || rez == null) {
                return null;
            }
            return rez;
        }


        /** checks if i is the index of a literal or a valid set of productions **/
        private bool ConditionProd(int i, Dictionary<int, HashSet<Production>> prods)
        {
            if (this.isLiteral(i))
                return true;
            return prods.ContainsKey(i);
        }

        /** filters a set of productions according to filterRule
        **/
        private HashSet<Production> filterProdSet1(Dictionary<int, HashSet<Production>> prods0, HashSet<Production> set)
        {
            HashSet<Production> set1 = new HashSet<Production>();
            foreach (Production prod in set) {
                if (this.FilterRule(prods0, prod))
                    set1.Add(prod);
            }
            return set1;
        }



        /** filters an IntMap of productions according to filterProdsSet1
         *
         **/
        private Dictionary<int, HashSet<Production>> FilterProductions(Dictionary<int, HashSet<Production>> prods0, Dictionary<int, HashSet<Production>> prods)
        {
            Dictionary<int, HashSet<Production>> tempRez = new Dictionary<int, HashSet<Production>>();
            bool are_diff = false;
            foreach (int index in prods.Keys) 
            {
                HashSet<Production> setProd = prods[index];
                HashSet<Production> intermRez = this.filterProdSet1(prods0, setProd);
                if (!(intermRez.Count == 0)) 
                {
                    tempRez.Add(index, intermRez);
                }
            }
            Dictionary<int, HashSet<Production>> prods1 = new Dictionary<int, HashSet<Production>>();
            foreach (KeyValuePair<int, HashSet<Production>> kvp in prods0) 
            {
                prods1.Add(kvp.Key, kvp.Value);
            }
            foreach (KeyValuePair<int, HashSet<Production>> kvp in tempRez) 
            {
                int index = kvp.Key;
                HashSet<Production> hp = kvp.Value;
                if (prods0.ContainsKey(index)) 
                {
                    if (!prods0[index].Equals(hp)) 
                    {
                        foreach (Production prod in prods0[index]) 
                        {
                            hp.Add(prod);
                        }
                        prods1.Add(index, hp);
                        are_diff = true;
                    }
                } 
                else 
                {
                    prods1.Add(index, hp);
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



        /** checks if a production satisfies conditionProd recursively **/
        private bool FilterRule(Dictionary<int, HashSet<Production>> prods, Production p)
        {
            if (p is ApplProduction) 
            {
                ApplProduction ap = (ApplProduction)p;
                foreach (int i in ap.domain) 
                {
                    if (!this.ConditionProd(i, prods)) 
                    {
                        return false;
                    }
                    return true;
                }
            }
            return this.ConditionProd(((CoerceProduction)p).initId, prods);
        }
        
        /** checks if a production just has a variable argument **/
        private bool Is_ho_prod(Production p)
        {
            if (p is ApplProduction) {
                List<int> args = ((ApplProduction)p).domain;
                if (args.Count == 1 && args[0] == -4) {
                    return true;
                }
            }
            return false;
        }



        /** gets list of forest ids from the categories in ho_cats **/
        private HashSet<int> Ho_fids()
        {
            HashSet<int> rezTemp = new HashSet<int>();
            List<string> hocats = this.Ho_cats();
            CncCat[] cncCats = this.cnc.cncCats.Values.ToArray<CncCat>();
            foreach (string hc in hocats) 
            {
                // for (int i = 0 ; i < ho_cats.size() ; i++)
                foreach (CncCat cncs in cncCats) 
                {
                    // for (int j = 0 ; j < cncCats.length ; j++)
                    if (hc.Equals(cncs.name)) 
                    {
                        // if (ho_cats.elementAt(i).equals(cncCats[j].getName()))
                        for (int ind = cncs.firstFID; ind <= cncs.lastFID; ind++) 
                        {
                            rezTemp.Add(ind);
                        }
                        
                    }
                }
            }
            return rezTemp;
        }


        /**get all names of types from Concrete **/
        private List<string> Ho_cats()
        {
            List<string> rezTemp = new List<string>();
            Abstract abstr = this.pgf.GetAbstract();
            List<AbsFun> absFuns = abstr.absFuns;
            foreach (AbsFun af in absFuns) 
            {
            // for (int i = 0 ; i < absFuns.length ; i++) {
                List<Hypo> hypos = af.type.hypos;
                // Hypo[] hypos = absFuns[i].getType().getHypos();
                foreach (Hypo hypo in hypos) 
                {
                // for (int j = 0 ; j < hypos.length ; j++)
                    if (!rezTemp.Contains(hypo.type.name)) 
                    {
                    // if (!rezTemp.contains(hypos[j].getType().getName()))
                        rezTemp.Add(hypo.type.name);
                    }
                }
            }
            return rezTemp;
        }



        /**gets the types from the hypotheses of a type **/
        private List<string> HypoArgsOfType(CSPGF.Reader.Type t)
        {
            List<Hypo> hypos = t.hypos;
            List<string> tmp = new List<string>();
            foreach (Hypo h in hypos) 
            {
                // for (int i = 0 ; i < hypos.length ; i++)
                tmp.Add(h.type.name);
                // rez[i] = hypos[i].getType().getName();
            }
            return tmp;
        }

        /** flattens a bracketed token **/
        private List<string> Untokn(BracketedTokn bt, string after)
        {
            if (bt is LeafKS) 
            {
                List<string> d = ((LeafKS)bt).tokens;
                List<string> rez = new List<string>();
                foreach (string str in d) {
                    rez.Add(str);
                }
                return rez;
            } 
            else if (bt is LeafKP) 
            {
                List<string> d = ((LeafKP)bt).defaultTokens;
                List<Alternative> alts = ((LeafKP)bt).alternatives;
                List<string> rez = new List<string>();
                foreach (Alternative alt in alts) 
                {
                    List<string> ss2 = alt.alt2;
                    foreach (string str in ss2) 
                    {
                        // for (int j = 0; j<ss2.Length; j++)
                        if (after.StartsWith(str)) 
                        {
                            List<string> ss1 = alt.alt1;
                            foreach (string str2 in ss1) 
                            {
                                // for(int k = ss1.Length-1; k>=0; k--)
                                rez.Add(str2);
                            }
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
                List<BracketedTokn> bs = ((Bracket)bt).bracketedtoks;
                for (int i = bs.Count - 1; i >= 0; i--) 
                {
                    foreach (string str in this.Untokn(bs.ElementAt(i), after)) 
                    {
                        rez.Add(str);
                    }
                    //rez.addAll(untokn(bs.elementAt(i),after));
                    after = rez.Last();
                }
                return rez;
            }
        }


        /** flattens the result of the linearization **/

        private List<string> RenderLin(LinTriple v)
        {
            List<string> rez = new List<string>();
            List<string> rezF = new List<string>();
            List<List<BracketedTokn>> vtemp = v.linTable;
            string after = string.Empty;
            for (int k = vtemp.ElementAt(0).Count - 1; k >= 0; k--) 
            {
                //{rez.addAll(untokn(vtemp.elementAt(0).elementAt(k),after));
                foreach (string str in this.Untokn(vtemp.ElementAt(0).ElementAt(k), after)) 
                {
                    rez.Add(str);
                }
                after = rez.Last();
                //rez.addAll(untokn(vtemp.elementAt(0).elementAt(k), after));

            }
            foreach (string str in rez) 
            {
                rezF.Insert(0, str);
            }
            return rezF;
        }


        //---------------------------------------------------------

        private List<List<string>> RenderAllLins(List<LinTriple> v)
        {
            List<List<string>> rez = new List<List<string>>();
            foreach (LinTriple lt in v) 
            {
                rez.Add(this.RenderLin(lt));
            }
            return rez;
        }

        private List<LinTriple> Linearize(CSPGF.Trees.Absyn.Tree e)
        {
            return this.Lin0(new List<string>(), new List<string>(), null, 0, e);
        }

    /**
     * main linearization function
     * @param xs is the list of bound variables (from lambdas)
     * @param ys
     * @param mb_cty
     * @param mb_fid
     * @param e is the tree to linearize
     * @return all the possible linearized tuples for this tree.
     **/
        private List<LinTriple> Lin0(List<string> xs, List<string> ys, CncType mb_cty, int mb_fid, CSPGF.Trees.Absyn.Tree tree)
        {
            // if tree is a lambda, we add the variable to the list of bound
            // variables and we linearize the subtree.
            if (tree is CSPGF.Trees.Absyn.Lambda) 
            {
                xs.Add(((CSPGF.Trees.Absyn.Lambda)tree).Ident_);
                return this.Lin0(xs, ys, mb_cty, mb_fid, ((CSPGF.Trees.Absyn.Lambda)tree).Tree_);
            } 
            else if (xs.Count == 0) 
            {
                List<CSPGF.Trees.Absyn.Tree> es = new List<CSPGF.Trees.Absyn.Tree>();
                if (tree is CSPGF.Trees.Absyn.Application) 
                {
                    do {
                        es.Add(((CSPGF.Trees.Absyn.Application)tree).Tree_2);
                        tree = ((CSPGF.Trees.Absyn.Application)tree).Tree_1;
                    } while (tree is CSPGF.Trees.Absyn.Application);
                }
                if (tree is Function) 
                {
                    return this.Apply(xs, mb_cty, mb_fid, ((Function)tree).Ident_, es);
                } 
                else 
                {
                    //RuntimeException -> Exception
                    throw new Exception("Undefined construction for expressions !!!");
                }
            } 
            else 
            {
                foreach (string yss in ys) 
                {
                    //xs.addAll(ys);
                    xs.Add(yss);
                }
                List<CSPGF.Trees.Absyn.Tree> exprs = new List<CSPGF.Trees.Absyn.Tree>();
                exprs.Add(tree);
                for (int i = 0; i < xs.Count; i++) 
                {
                    exprs.Add(new CSPGF.Trees.Absyn.Literal(new CSPGF.Trees.Absyn.StringLiteral(xs.ElementAt(i))));
                } 
                return this.Apply(xs, mb_cty, mb_fid, "_B", exprs);
            }
        }




        /** intermediate linearization for complex expressions
     * Linearize function appliction.
     * To linearize the application of the function "f" to the arguments
     * (trees) a, b and c use :
     * apply(???,???,??? "f", [a,b,c])
     *
     * 'apply' will linearize the argument and then use the concrete function
     * for "f" to glue them together.
     * @param xs
     * @param mb_cty
     * @param n_fid
     * @param f the name of the function to be applied
     * @param es the argument of the function to linearize
     * @return All the possible linearization for the application of f to es
     **/
        private List<LinTriple> Apply(List<string> xs, CncType mb_cty, int n_fid, string f, List<CSPGF.Trees.Absyn.Tree> es)
        {
            Dictionary<int, HashSet<Production>> prods = this.lProd[f];
            if (prods == null) 
            {
                List<CSPGF.Trees.Absyn.Tree> newes = new List<CSPGF.Trees.Absyn.Tree>();
                newes.Add(new CSPGF.Trees.Absyn.Literal(new CSPGF.Trees.Absyn.StringLiteral(f)));
                System.Console.WriteLine("Function " + f + " does not have a linearization !");
                return this.Apply(xs, mb_cty, n_fid, "_V", newes);
            } 
            else 
            {
                List<AppResult> vApp = this.GetApps(prods, mb_cty, f);
                List<LinTriple> rez = new List<LinTriple>();
                for (int i = 0; i < vApp.Count; i++) 
                {
                    List<CncType> copy_ctys = vApp.ElementAt(i).cncTypes;
                    List<CncType> ctys = new List<CncType>();
                    for (int ind = copy_ctys.Count - 1; ind >= 0; ind--) 
                    {
                        ctys.Add(copy_ctys.ElementAt(ind));
                    }
                    if (es.Count != ctys.Count) 
                    {
                        //LinearizerException -> Exception
                        throw new Exception("lengths of es and ctys don't match" + es.ToString() + " -- " + ctys.ToString());
                    }
                    List<Sequence> lins = vApp.ElementAt(i).cncFun.sequences;
                    string cat = vApp.ElementAt(i).cncType.cId;
                    List<CSPGF.Trees.Absyn.Tree> copy_expr = new List<CSPGF.Trees.Absyn.Tree>();
                    for (int ind = 0; ind < es.Count; ind++) 
                    {
                        copy_expr.Add(es.ElementAt(ind));
                    }
                    List<RezDesc> rezDesc = this.Descend(n_fid, ctys, copy_expr, xs);
                    for (int k = 0; k < rezDesc.Count; k++) 
                    {
                        RezDesc intRez = rezDesc.ElementAt(k);
                        List<List<BracketedTokn>> linTab = new List<List<BracketedTokn>>();
                        for (int ind = 0; ind < lins.Count; ind++) 
                        {
                            linTab.Add(this.ComputeSeq(lins[ind], intRez.cncTypes, intRez.bracketedtokn));
                        }
                        rez.Add(new LinTriple(n_fid + 1, new CncType(cat, n_fid), linTab));
                    }
                }
                return rez;
            }
        }


        private List<AppResult> GetApps(Dictionary<int, HashSet<Production>> prods, CncType mb_cty, string f)
        {
            if (mb_cty == null) 
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
                        //Iterator<Entry<Integer, HashSet<Production>>> it = prods.entrySet().iterator();
                        //while (it.hasNext()) {
                        //Entry<Integer, HashSet<Production>> en = it.next();
                        int fid = it.Key;
                        foreach (Production prod in it.Value) 
                        {
                            //Iterator<Production> ip = en.getValue().iterator();
                            //while (ip.hasNext()) {
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
                int fid = mb_cty.fId;
                HashSet<Production> setProd = prods[fid];
                List<AppResult> rez = new List<AppResult>();
                if (setProd == null)
                {
                    return new List<AppResult>();
                }
                else
                {
                    foreach (Production prod in setProd)
                    {
                        //Iterator<Production> iter = setProd.iterator();
                        //while (iter.hasNext())
                        foreach (AppResult app in this.ToApp(mb_cty, prod, f, prods))
                        {
                            rez.Add(app);
                        }
                    }
                    return rez;
                }
            }
        }





        private List<AppResult> ToApp(CncType cty, Production p, string f, Dictionary<int, HashSet<Production>> prods)
        {
            List<AppResult> rez = new List<AppResult>();
            if (p is ApplProduction) 
            {
                List<int> args = ((ApplProduction)p).domain;
                CncFun cncFun = ((ApplProduction)p).function;
                List<CncType> vtype = new List<CncType>();
                if (f.Equals("V")) 
                {
                    for (int i = 0; i < args.Count; i++) 
                    {
                        vtype.Add(new CncType("__gfVar", args[i]));
                    }
                    rez.Add(new AppResult(cncFun, cty, vtype));
                    return rez;
                }
                if (f.Equals("_B")) {
                    vtype.Add(new CncType(cty.cId, args[0]));
                    for (int i = 1; i < args.Count; i++) {
                        vtype.Add(new CncType("__gfVar", args[i]));
                    }
                    rez.Add(new AppResult(cncFun, cty, vtype));
                    return rez;
                } 
                else 
                {
                    List<AbsFun> absFuns = this.pgf.GetAbstract().absFuns;
                    CSPGF.Reader.Type t = null;
                    for (int i = 0; i < absFuns.Count; i++) 
                    {
                        if (f.Equals(absFuns[i].name))
                            t = absFuns[i].type;
                    }
                    if (t == null) 
                    {
                        //LinearizerException -> Exception
                        throw new Exception(" f not found in the abstract syntax");
                    }
                    List<string> catSkel = this.CatSkeleton(t);
                    string res = catSkel.ElementAt(0);
                    for (int i = 0; i < args.Count; i++) 
                    {
                        vtype.Add(new CncType(catSkel.ElementAt(i + 1), args[i]));
                    }
                    rez.Add(new AppResult(cncFun, new CncType(res, cty.fId), vtype));
                    return rez;
                }
            } 
            else 
            {
                int fid = ((CoerceProduction)p).initId;
                HashSet<Production> setProds = prods[fid];
                foreach (Production prod in setProds) 
                {
                //Iterator<Production> it = setProds.iterator();
                //while (it.hasNext())
                    foreach (AppResult app in this.ToApp(cty, prod, f, prods))
                    {
                        rez.Add(app);
                    }
                }
                return rez;
            }
        }


        /** computes the types of the arguments of a function type **/
        private List<string> CatSkeleton(CSPGF.Reader.Type t)
        {
            List<string> rez = new List<string>();
            rez.Add(t.name);
            List<Hypo> hypos = t.hypos;
            foreach (Hypo h in hypos) {
                rez.Add(h.type.name);
            }
            return rez;
        }



        /** creates a simple vector of vectors of bracketed tokens containing a string value **/
        private List<List<BracketedTokn>> ss(string s)
        {
            List<List<BracketedTokn>> bt = new List<List<BracketedTokn>>();
            List<BracketedTokn> v = new List<BracketedTokn>();
            List<string> sts = new List<string>();
            sts.Add(s);
            v.Add(new LeafKS(sts));
            bt.Add(v);
            return bt;
        }



        /** computes the sequence of bracketed tokens associated to a symbol **/
        private List<BracketedTokn> Compute(Symbol s, List<CncType> cncTypes, List<List<List<BracketedTokn>>> linTables)
        {
            if (s is ArgConstSymbol) 
            {
                int arg = ((ArgConstSymbol)s).arg;
                int cons = ((ArgConstSymbol)s).cons;
                return this.GetArg(arg, cons, cncTypes, linTables);
            } 
            else if (s is AlternToksSymbol) 
            {
                List<string> toks = ((AlternToksSymbol)s).tokens;
                List<Alternative> alts = ((AlternToksSymbol)s).alts;
                List<BracketedTokn> v = new List<BracketedTokn>();
                v.Add(new LeafKP(toks, alts));
                return v;
            } 
            else 
            {
                List<string> toks = ((ToksSymbol)s).tokens;
                List<BracketedTokn> v = new List<BracketedTokn>();
                v.Add(new LeafKS(toks));
                return v;
            }
        }


        /** retrieves a sequence of bracketed tokens from an intermediate result of the linearization
        * according to 2 indices from a production **/
        private List<BracketedTokn> GetArg(int d, int r, List<CncType> cncTypes, List<List<List<BracketedTokn>>> linTables)
        {
            if (cncTypes.Count <= d)
            {
                return new List<BracketedTokn>();
            }
            CncType cncType = cncTypes.ElementAt(d);
            List<List<BracketedTokn>> lin = linTables.ElementAt(d);
            string cat = cncType.cId;
            int fid = cncType.fId;
            List<BracketedTokn> arg_lin = lin.ElementAt(r);
            if (arg_lin.Count == 0) 
            {
                return arg_lin;
            }
            List<BracketedTokn> bt = new List<BracketedTokn>();
            bt.Add(new Bracket(cat, fid, r, arg_lin));
            return bt;
        }





        /** computes a sequence of bracketed tokens from the sequence of symbols of a concrete function **/
        private List<BracketedTokn> ComputeSeq(Sequence seqId, List<CncType> cncTypes, List<List<List<BracketedTokn>>> linTables)
        {
            List<BracketedTokn> bt = new List<BracketedTokn>();
            List<Symbol> symbs = seqId.symbs;
            for (int j = 0; j < symbs.Count; j++)
                foreach (BracketedTokn btn in this.Compute(symbs[j], cncTypes, linTables))
                {
                    bt.Add(btn);
                }
            return bt;
        }





        /** shuffles the results of of the intermediate linearization,
     * for generating all the possible combinations
     **/
        private List<RezDesc> Descend(int n_fid, List<CncType> cncTypes, List<CSPGF.Trees.Absyn.Tree> exps, List<string> xs)
        {
            List<RezDesc> rez = new List<RezDesc>();
            if (exps.Count == 0) 
            {
                rez.Add(new RezDesc(n_fid, new List<CncType>(), new List<List<List<BracketedTokn>>>()));
                return rez;
            } 
            else 
            {
                CncType cncType = cncTypes.First();
                cncTypes.RemoveAt(0);
                CSPGF.Trees.Absyn.Tree exp = exps.First();
                exps.RemoveAt(0);
                List<LinTriple> rezLin = this.Lin0(new List<string>(), xs, cncType, n_fid, exp);
                List<RezDesc> rezDesc = this.Descend(n_fid, cncTypes, exps, xs);
                for (int i = 0; i < rezLin.Count; i++)
                {
                    for (int j = 0; j < rezDesc.Count; j++)
                    {
                        CncType c = rezLin.ElementAt(i).cncType;
                        List<CncType> vcnc = rezDesc.ElementAt(j).cncTypes;
                        vcnc.Add(c);
                        List<List<List<BracketedTokn>>> vbt = rezDesc.ElementAt(j).bracketedtokn;
                        List<List<BracketedTokn>> bt = rezLin.ElementAt(i).linTable;
                        vbt.Add(bt);
                        rez.Add(new RezDesc(n_fid, vcnc, vbt));
                    }
                }
            }
            return rez;
        }




        /**checks if a production is application production **/
        private bool IsApp(Production p) 
        {
            return p is ApplProduction;
        }


        /** checks if an integer is the index of an integer literal **/
        private bool isLiteralInt(int i)
        {
            return i == -2;
        }

        /** checks if an integer is the index of a string literal **/
        private bool isLiteralString(int i)
        {
            return i == -1;
        }

        /** checks if an integer is the index of a float literal **/
        private bool isLiteralFloat(int i)
        {
            return i == -3;
        }

        /** checks if an integer is the index of a variable literal **/
        private bool isLiteralVar(int i)
        {
            return i == -4;
        }

        /** checks if an integer is the index of a literal **/
        private bool isLiteral(int i)
        {
            if (this.isLiteralString(i) || this.isLiteralInt(i) || this.isLiteralFloat(i) || this.isLiteralVar(i)) 
            {
                return true;
            }
            return false;
        }



    }
}
