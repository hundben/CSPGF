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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSPGF.reader;
using CSPGF.trees.Absyn;

namespace CSPGF
{
    class Generator
    {
        private Random random;
        private PGF pgf;
        private Dictionary<String, HashSet<String>> dirRules;
        private Dictionary<String, HashSet<String>> indirRules;

        /** generates a random expression of a given category
         * does not handle dependent categories or categories with implicit arguments
         **/
        public Generator(PGF _pgf)
        {
            random = new Random();
            pgf = _pgf;
            dirRules = new Dictionary<String, HashSet<String>>();
            indirRules = new Dictionary<String, HashSet<String>>();
            List<AbsCat> absCats = pgf.GetAbstract().absCats;
            List<AbsFun> absFuns = pgf.GetAbstract().absFuns;
            HashSet<String> dirFuns = new HashSet<String>();
            HashSet<String> indirFuns = new HashSet<String>();
            foreach(AbsCat abc in absCats) {
            //for (int i = 0 ; i < absCats.Count ; i++) {
                dirFuns = new HashSet<String>();
                indirFuns = new HashSet<String>();
                List<WeightedIdent> functions = abc.functions;
                foreach (WeightedIdent weid in functions) {
                    foreach (AbsFun ab in absFuns) {
                        if (weid.ident.Equals(ab.name)) {
                            if(ab.type.hypos.Count == 0) {
                                dirFuns.Add(weid.ident);
                            } else {
                                indirFuns.Add(weid.ident);
                            }
                            break;
                        }
                    }
                }
                /*for (int j = 0 ; j < functions.Count ; j++)
                    for (int k = 0 ; k < absFuns.Count ; k++)
                        if (functions[j].ident.Equals(absFuns[k].name)) {
                            if (absFuns[k].type.hypos.Count == 0)
                                dirFuns.Add(functions[j].ident);
                            else
                                indirFuns.Add(functions[j].ident);
                            break;
                        }*/
                dirRules.Add(abc.name, dirFuns);
                indirRules.Add(abc.name, indirFuns);
            }
        }


        public Tree Gen()
        {
            return Gen(pgf.GetAbstract().StartCat());
        }

        /** generates a category with a random direct rule
         * suitable for simple expressions
         **/
        // FIXME what is 'type' for ???
        // FIXME couldn't dirFuns be an array ?
        public Tree GetDirect(String type, HashSet<String> dirFuns)
        {
            int rand = this.random.Next(dirFuns.Count());
            return new Function((String)dirFuns.ToArray()[rand]);
        }

        /** generates a category with a random indirect rule
         * creates more complex expressions
         **/
        public Tree GetIndirect(String type, HashSet<String> indirFuns)
        {
            //Iterator<String> it = indirFuns.iterator();
            //Vector<String> vs = new Vector<String>();
            /*while (it.hasNext())
            {
                vs.add(it.next());
            }*/
            List<String> vs = new List<String>();
            foreach (String it in indirFuns) {
                vs.Add(it);
            }

            int rand = random.Next(vs.Count());
            String funcName = vs.ElementAt(rand);
            List<AbsFun> absFuns = pgf.GetAbstract().absFuns;
            foreach (AbsFun a in absFuns)
            //for (int i = 0 ; i < absFuns.Length ; i++)
            {
                if (a.name.Equals(funcName)) {
                    List<Hypo> hypos = a.type.hypos;
                    String[] tempCats = new String[hypos.Count];
                    Tree[] exps = new Tree[hypos.Count];
                    // TODO: Går detta att göra om?
                    for (int k = 0 ; k < hypos.Count ; k++) {
                        tempCats[k] = hypos[k].type.name;
                        exps[k] = Gen(tempCats[k]);
                        if (exps[k] == null) {
                            return null;
                        }
                    }
                    Tree rez = new Function(funcName);
                    foreach (Tree t in exps) {
                    //for (int j = 0 ; j < exps.Length ; j++)
                        rez = new Application(rez, t);
                    }
                    return rez;
                }
            }
            return null;
        }


        /** generates a random expression of a given category
         * the empirical probability of using direct rules is 60%
         * this decreases the probability of having infinite trees for infinite grammars
         **/
        public Tree Gen(String type)
        {
            if (type.Equals("Integer")) {
                return new Literal(new IntLiteral(GenerateInt()));
            } else if (type.Equals("Float")) {
                return new Literal(new FloatLiteral(GenerateFloat()));
            } else if (type.Equals("String")) {
                return new Literal(new StringLiteral(GenerateString()));
            }
            int depth = random.Next(5); //60% constants, 40% functions
            HashSet<String> dirFuns = dirRules[type];
            HashSet<String> indirFuns = indirRules[type];
            // TODO: Check if it should be inverted?
            Boolean isEmptyDir = dirFuns.Any();
            Boolean isEmptyIndir = indirFuns.Any();
            if (isEmptyDir && isEmptyIndir) {
                throw new Exception("Cannot generate any expression of type " + type);
            }
            if (isEmptyDir) {
                return GetIndirect(type, indirFuns);
            }
            if (isEmptyIndir) {
                return GetDirect(type, dirFuns);
            }
            if (depth <= 2) {
                return GetDirect(type, dirFuns);
            }
            return GetIndirect(type, indirFuns);
        }


        /** generates a number of expressions of a given category
        * the expressions are independent
        * the probability of having simple expressions is higher
        **/
        public List<Tree> GenerateMany(String type, int count)
        {
            int contor = 0;
            List<Tree> rez = new List<Tree>();
            if (contor >= count) {
                return rez;
            }
            HashSet<String> dirFuns = dirRules[type];
            HashSet<String> indirFuns = indirRules[type];
            foreach (String it in dirFuns) {
                Tree interm = GetDirect(type, dirFuns);
                if (interm != null) {
                    contor++;
                    rez.Add(interm);
                    if (contor == count) {
                        return rez;
                    }
                }
            }
            foreach (String it in indirFuns) {
                Tree interm = GetIndirect(type, indirFuns);
                if (interm != null) {
                    contor++;
                    rez.Add(interm);
                    if (contor == count) {
                        return rez;
                    }
                }
            }
            return rez;
        }


        /** generates a random string
        **/

        public String GenerateString()
        {
            String[] ss = { "x", "y", "foo", "bar" };
            return ss[random.Next(ss.Length)];
        }

        /** generates a random integer
         **/
        public int GenerateInt()
        {
            return random.Next(100000);
        }

        /** generates a random float
         **/
        public double GenerateFloat()
        {
            return random.NextDouble();
        }
    }
}
