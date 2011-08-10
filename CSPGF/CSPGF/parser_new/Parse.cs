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

namespace CSPGF.Parser_new
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Parser
    {
        // Notice, the language might be bad here? better to use it when we want to parse a text?
        private PGF pgf;
        private List<Reader.Production> prods;
        private Reader.Abstract abs;
        private List<Reader.CncCat> cncCat;
        private List<Reader.CncFun> cncFun;
        private Reader.Concrete concrete;
        private Chart chart;
        public Parser(PGF pgf)
        {
            this.pgf = pgf;
        }
        // TODO should return tree
        public void ParseText(string language, string text)
        {
            // Check if the language exists
            this.concrete = this.pgf.GetConcrete(language);
            string[] tokens = text.Split(' ');  //tokenize
            //lookConcrComplete :: PGF -> CId -> Concr
            this.abs = this.pgf.GetAbstract();        //TODO maybe not necessary...
            this.cncCat = this.concrete.GetCncCats();
            //calculate next free category
            int nextCat = 0;
            foreach (Reader.CncCat c in this.cncCat)
            {
                nextCat = Math.Max(c.LastFID, nextCat);
            }

            nextCat++;
            this.chart = new Chart(nextCat);
            this.cncFun = this.concrete.CncFuns;
            string startCatName = this.abs.StartCat();
            // This code might be unnecessary, but then we have the startcategory saved at least ;P
            Reader.CncCat startCat = null;
            foreach (Reader.CncCat cat in this.cncCat) 
            {
                if (cat.Name == startCatName) 
                {
                    startCat = cat;
                    break;
                }
            }

            if (startCat == null) 
            {
                throw new Exception("Start category " + startCatName + " not found!");
            }

            this.prods = this.concrete.GetProductions();  //load productions

            this.PredictTEMP(startCat.FirstFID);

            string text2 = this.GetToken(this.Predict(0, this.prods)[0]);

            System.Console.WriteLine("whaaa");
            // Map.Map CId CncCat = Map(Map CId CncCat whatever... :D
        }

        public void ParseWithRecovery(string language, string text)
        {
        }

        private void PredictTEMP(int cat)
        {
            // TRY to predict the legal nextstates
            foreach (Reader.ApplProduction p in this.Predict(cat, this.prods))
            {
                //Loop over all productions that we want
                foreach (int dom in p.Domain()) 
                {
                    foreach (Reader.ApplProduction p2 in this.Predict(dom, this.prods))
                    {
                        foreach (Reader.Sequence s in p2.Function.Sequences)
                        {
                            Console.WriteLine(this.GetSymbols(s.Symbs));
                        }
                    }
                }
            }
        }

        // TODO merge this with parse later... just to keep it clean
        private void ParseText2(string text, ParseTrie tree, int cat)
        {
            foreach (string word in text.Split(' ')) 
            {
                foreach (Reader.ApplProduction ap in this.Predict(cat, this.prods)) 
                {
                }
            }
        }

        // Returns all application productions in category cat 
        private List<Reader.ApplProduction> Predict(int cat, List<Reader.Production> prods)
        {
            List<Reader.ApplProduction> appList = new List<Reader.ApplProduction>();
            foreach (Reader.Production p in prods) 
            {
                if (p.FId == cat)
                {
                    if (p is Reader.ApplProduction)
                    {
                        appList.Add((Reader.ApplProduction)p);
                    }
                    else if (p is Reader.CoerceProduction)
                    {
                        appList.AddRange(this.UnCoerse(cat, prods));
                    }
                }
            }

            return this.RemoveDoubles(appList);
        }
        // Tries to remove coersions... should work :)
        private List<Reader.ApplProduction> UnCoerse(int cat, List<Reader.Production> prods)
        {
            List<Reader.ApplProduction> appList = new List<Reader.ApplProduction>();

            foreach (Reader.Production p in prods)
            {
                if (p.FId == cat)
                {
                    if (p is Reader.CoerceProduction)
                    {
                        Reader.CoerceProduction cop = (Reader.CoerceProduction)p;
                        appList.AddRange(this.UnCoerse(cop.InitId, prods));
                    }
                    else if (p is Reader.ApplProduction)
                    {
                        appList.Add((Reader.ApplProduction)p);
                    }
                }
            }
            // Remove doubles?, could distinct be used here?
            return appList;
        }
        // Returns a string with the token symbols, only used for debug
        private string GetSymbols(List<Reader.Symbol> seq)
        {
            string all = string.Empty;
            foreach (Reader.Symbol s in seq)
            {
                all += s + ", ";
            }

            return all;
        }
        // Returns the categories for the next step
        private List<int> GetCat(int cat)
        {
            List<int> cats = new List<int>();
            return cats;
        }

        private List<Reader.ApplProduction> RemoveDoubles(List<Reader.ApplProduction> prods)
        {
            HashSet<Reader.ApplProduction> appList = new HashSet<Reader.ApplProduction>();
            foreach (Reader.ApplProduction p in prods)
            {
                appList.Add(p);
            }

            return appList.ToList<Reader.ApplProduction>();
        }

        private string GetToken(Reader.ApplProduction ap)
        {
            string token = string.Empty;
            foreach (Reader.Sequence seq in ap.Function.Sequences)
            {
                foreach (Reader.Symbol symb in seq.Symbs)
                {
                    if (symb is Reader.ToksSymbol)
                    {
                        Reader.ToksSymbol t = (Reader.ToksSymbol)symb;
                        //TODO
                        foreach (string tok in t.tokens)
                        {
                            token += tok + " ";
                        } 
                    }
                    else if (symb is Reader.AlternToksSymbol)
                    {
                        Reader.AlternToksSymbol t = (Reader.AlternToksSymbol)symb;
                        // TODO there is a list of strings (alt1?)
                        // pre...
                    }
                }
            }

            return token.TrimEnd();
        }
    }
}