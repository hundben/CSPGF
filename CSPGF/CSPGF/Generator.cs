//-----------------------------------------------------------------------
// <copyright file="Generator.cs" company="None">
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
    using CSPGF.Reader;
    using CSPGF.Trees.Absyn;

    /// <summary>
    /// Used to generate random numbers and trees.
    /// </summary>
    public class Generator
    {
        /// <summary>
        /// Used to get random numbers.
        /// </summary>
        private Random random;

        /// <summary>
        /// The current PGF-file.
        /// </summary>
        private PGF pgf;

        /// <summary>
        /// Direct rules.
        /// </summary>
        private Dictionary<string, HashSet<string>> dirRules;

        /// <summary>
        /// Indirect rules.
        /// </summary>
        private Dictionary<string, HashSet<string>> indirRules;

        /// <summary>
        /// Initializes a new instance of the Generator class.
        /// Generates a random expression of a given category does not handle dependent categories or categories with implicit arguments
        /// </summary>
        /// <param name="pgf">PGF object to use</param>
        public Generator(PGF pgf)
        {
            this.random = new Random();
            this.pgf = pgf;
            this.dirRules = new Dictionary<string, HashSet<string>>();
            this.indirRules = new Dictionary<string, HashSet<string>>();
            List<AbsCat> absCats = this.pgf.GetAbstract().AbsCats;
            List<AbsFun> absFuns = this.pgf.GetAbstract().AbsFuns;
            HashSet<string> dirFuns = new HashSet<string>();
            HashSet<string> indirFuns = new HashSet<string>();
            foreach (AbsCat abc in absCats) 
            {
                dirFuns = new HashSet<string>();
                indirFuns = new HashSet<string>();
                List<WeightedIdent> functions = abc.Functions;
                foreach (WeightedIdent weid in functions) 
                {
                    foreach (AbsFun ab in absFuns) 
                    {
                        if (weid.Ident.Equals(ab.Name)) 
                        {
                            if (ab.Type.Hypos.Count == 0) 
                            {
                                dirFuns.Add(weid.Ident);
                            }
                            else 
                            {
                                indirFuns.Add(weid.Ident);
                            }

                            break;
                        }
                    }
                }

                this.dirRules.Add(abc.Name, dirFuns);
                this.indirRules.Add(abc.Name, indirFuns);
            }
        }

        /// <summary>
        /// Generates a tree
        /// </summary>
        /// <returns>Returns the tree</returns>
        public Tree Gen()
        {
            return Gen(this.pgf.GetAbstract().StartCat());
        }

        /// <summary>
        /// Generates a category with a random direct rule. Suitable for simple expressions
        /// </summary>
        /// <param name="dirFuns">Direct Functions</param>
        /// <returns>Generated tree</returns>
        public Tree GetDirect(HashSet<string> dirFuns)
        {
            int rand = this.random.Next(dirFuns.Count);
            return new Function((string)dirFuns.ToArray()[rand]);
        }

        /// <summary>
        /// Generates a category with a random indirect rule. Creates more complex expressions.
        /// </summary>
        /// <param name="indirFuns">Indirect Functions</param>
        /// <returns>Generated tree</returns>
        public Tree GetIndirect(HashSet<string> indirFuns)
        {
            List<string> vs = new List<string>();
            foreach (string it in indirFuns) 
            {
                vs.Add(it);
            }

            int rand = this.random.Next(vs.Count());
            string funcName = vs.ElementAt(rand);
            List<AbsFun> absFuns = this.pgf.GetAbstract().AbsFuns;
            foreach (AbsFun a in absFuns) 
            {
                if (a.Name.Equals(funcName)) 
                {
                    List<Hypo> hypos = a.Type.Hypos;
                    string[] tempCats = new string[hypos.Count];
                    Tree[] exps = new Tree[hypos.Count];

                    // TODO: Går detta att göra om?
                    for (int k = 0; k < hypos.Count; k++) 
                    {
                        tempCats[k] = hypos[k].Type.Name;
                        exps[k] = Gen(tempCats[k]);
                        if (exps[k] == null) 
                        {
                            return null;
                        }
                    }

                    Tree rez = new Function(funcName);
                    foreach (Tree t in exps) 
                    {
                        rez = new Application(rez, t);
                    }

                    return rez;
                }
            }

            return null;
        }

        /// <summary>
        /// generates a random expression of a given category
        /// the empirical probability of using direct rules is 60%
        /// this decreases the probability of having infinite trees for infinite grammars
        /// Joins a first name and a last name together into a single string.
        /// </summary>
        /// <param name="type">Type of tree to generate</param>
        /// <returns>Generated tree</returns>
        public Tree Gen(string type)
        {
            if (type.Equals("Integer")) 
            {
                return new Literal(new IntLiteral(this.GenerateInt()));
            }
            else if (type.Equals("Float")) 
            {
                return new Literal(new FloatLiteral(this.GenerateFloat()));
            }
            else if (type.Equals("String")) 
            {
                return new Literal(new StringLiteral(this.GenerateString()));
            }

            int depth = this.random.Next(5); // 60% constants, 40% functions
            HashSet<string> dirFuns = this.dirRules[type];
            HashSet<string> indirFuns = this.indirRules[type];

            // TODO: Check if it should be inverted?
            bool isEmptyDir = dirFuns.Any();
            bool isEmptyIndir = indirFuns.Any();
            if (isEmptyDir && isEmptyIndir) 
            {
                throw new Exception("Cannot generate any expression of type " + type);
            }

            if (isEmptyDir) 
            {
                return this.GetIndirect(indirFuns);
            }

            if (isEmptyIndir) 
            {
                return this.GetDirect(dirFuns);
            }

            if (depth <= 2) 
            {
                return this.GetDirect(dirFuns);
            }

            return this.GetIndirect(indirFuns);
        }

        /// <summary>
        /// Generates a number of expressions of a given category. The expressions are independent.
        /// The probability of having simple expressions is higher.
        /// </summary>
        /// <param name="type">Type to generate</param>
        /// <param name="count">Number to generate</param>
        /// <returns>List of trees</returns>
        public List<Tree> GenerateMany(string type, int count)
        {
            int contor = 0;
            List<Tree> rez = new List<Tree>();
            if (contor >= count) 
            {
                return rez;
            }

            HashSet<string> dirFuns = this.dirRules[type];
            HashSet<string> indirFuns = this.indirRules[type];
            foreach (string it in dirFuns) 
            {
                Tree interm = this.GetDirect(dirFuns);
                if (interm != null) 
                {
                    contor++;
                    rez.Add(interm);
                    if (contor == count) 
                    {
                        return rez;
                    }
                }
            }

            foreach (string it in indirFuns) 
            {
                Tree interm = this.GetIndirect(indirFuns);
                if (interm != null) 
                {
                    contor++;
                    rez.Add(interm);
                    if (contor == count) 
                    {
                        return rez;
                    }
                }
            }

            return rez;
        }

        /// <summary>
        /// Generates a string
        /// </summary>
        /// <returns>Returns the generated string</returns>
        public string GenerateString()
        {
            string[] ss = { "x", "y", "foo", "bar" };
            return ss[this.random.Next(ss.Length)];
        }

        /// <summary>
        /// Generates a random integer
        /// </summary>
        /// <returns>Generated integer</returns>
        public int GenerateInt()
        {
            return this.random.Next(100000);
        }

        /// <summary>
        /// Generates a random float
        /// </summary>
        /// <returns>Generated float</returns>
        public double GenerateFloat()
        {
            return this.random.NextDouble();
        }
    }
}
