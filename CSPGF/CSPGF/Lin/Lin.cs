// -----------------------------------------------------------------------
// <copyright file="Lin.cs" company="yay">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CSPGF.Lin
{
    using System.Collections.Generic;
    using CSPGF.Grammar;
    using CSPGF.Trees.Absyn;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Lin
    {
        /// <summary>
        ///  asdfjsad lfjk sldakjf lsafj
        /// </summary>
        private readonly PGF pgf;

        /// <summary>
        /// salkasldfas lkjsaf lkja sdf
        /// </summary>
        private readonly Concrete concrete;

        /// <summary>
        /// alkjölskdjölkjasdf sdfasdfsdf
        /// </summary>
        private readonly Dictionary<string, Dictionary<int, HashSet<Production>>> linProd;

        /// <summary>
        /// Initializes a new instance of the <see cref="Lin"/> class.
        /// </summary>
        /// <param name="pgf">
        /// The pgf.
        /// </param>
        /// <param name="concrete">
        /// The concrete.
        /// </param>
        internal Lin(PGF pgf, Concrete concrete)
        {
            this.pgf = pgf;
            this.concrete = concrete;
            this.linProd = this.GetLProductions();
        }

        /// <summary>
        /// asdöoiuasdf lkjsadf 
        /// </summary>
        /// <param name="tree">
        /// The tree.
        /// </param>
        /// <returns>
        /// something dfs
        /// </returns>
        public string Linearize(Tree tree)
        {
            this.Bottom(new List<string>(), tree);
            return string.Empty;
        }

        /// <summary>
        /// sdf sdf sadf
        /// </summary>
        /// <param name="xs">
        /// The xs.
        /// </param>
        /// <param name="tree">
        /// The tree.
        /// </param>
        /// <returns>
        /// sadfas dfs
        /// </returns>
        public List<object> Bottom(List<string> xs, Tree tree)
        {
            if (xs.Count == 0)
            {
                List<Tree> trees = new List<Tree>();
                if (tree is Application)
                {
                    do
                    {
                        trees.Add(((Application)tree).Tree_2);
                        tree = ((Application)tree).Tree_1;
                    }
                    while (tree is Application);
                } 
                
                if (tree is Function)
                {
                    return this.Apply(((Function)tree).Ident_, trees);
                }
                else
                {
                    throw new LinearizerException("We no has support for high order functions");
                }
            } 
            else
            {
                throw new LinearizerException("We no has support for high order functions");
            }
        }

        /// <summary>
        /// asdjfh asdf
        /// </summary>
        /// <param name="ident">
        /// The ident.
        /// </param>
        /// <param name="trees">
        /// The trees.
        /// </param>
        /// <returns>
        /// asdlfjlaskdjf sjdkafh
        /// </returns>
        /// <exception cref="LinearizerException">
        /// If fail
        /// </exception>
        private List<object> Apply(string ident, object trees)
        {
            Type type = null;
            foreach (AbsFun abs in this.pgf.GetAbstract().AbsFuns)
            {
                if (abs.Name == ident)
                {
                    type = abs.Type;
                }
            }

            if (type == null)
            {
                throw new LinearizerException("It fail atz findingz zeh abs fun");
            }

            Dictionary<int, HashSet<Production>> p = null;
            if (!this.linProd.TryGetValue(ident, out p))
            {
                throw new LinearizerException("I iz failed");
            }

            List<Hypo> hs = new List<Hypo>(type.Hypos);

            return null;
        }








        // OLD -----------------------------------------------------------------------------------------------

        /// <summary>
        /// lakjf aslkfj sladkfj
        /// </summary>
        /// <returns> asdjf alsfj </returns>
        private Dictionary<string, Dictionary<int, HashSet<Production>>> GetLProductions()
        {
            return this.LinIndex(this.FilterProductions(new Dictionary<int, HashSet<Production>>(), this.concrete.GetSetOfProductions()));
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
                            HashSet<Production> singleton = new HashSet<Production> { prod };
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
            bool areDiff = false;

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
                        areDiff = true;
                    }
                }
                else
                {
                    prods1[index] = hp;
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
            ApplProduction ap = p as ApplProduction;
            if (ap != null)
            {
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
