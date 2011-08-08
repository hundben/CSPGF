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


namespace CSPGF.Parser_new
{
    class Parser
    {
        //Notice, the language might be bad here? better to use it when we want to parse a text?
        private PGF pgf;
        private List<Reader.Production> prods;
        private Reader.Abstract abs;
        private List<Reader.CncCat> cncCat;
        private List<Reader.CncFun> cncFun;
        private Reader.Concrete concrete;
        private Chart chart;
        public Parser(PGF _pgf)
        {
            pgf = _pgf;
        }
        //TODO should return tree
        public void ParseText(String language, String text)
        {
            //Check if the language exists
            concrete = pgf.GetConcrete(language);
            String[] tokens = text.Split(' ');  //tokenize
            //lookConcrComplete :: PGF -> CId -> Concr
            abs = pgf.GetAbstract();        //TODO maybe not necessary...
            cncCat = concrete.GetCncCats();
            //calculate next free category
            int nextCat = 0;
            foreach (Reader.CncCat c in cncCat)
            {
                nextCat = Math.Max(c.lastFID, nextCat);
            }
            nextCat++;
            chart = new Chart(nextCat);

            cncFun = concrete.cncFuns;
            String startCatName = abs.StartCat();
            //This code might be unnecessary, but then we have the startcategory saved at least ;P
            Reader.CncCat startCat = null;
            foreach (Reader.CncCat cat in cncCat) {
                if (cat.name == startCatName) {
                    startCat = cat;
                    break;
                }
            }
            if (startCat == null) {
                throw new Exception("Start category " + startCatName + " not found!");
            }
            prods = concrete.GetProductions();  //load productions

            PredictTEMP(startCat.firstFID);

            String text2 = GetToken(Predict(0, prods)[0]);

            System.Console.WriteLine("whaaa");
            //Map.Map CId CncCat = Map(Map CId CncCat whatever... :D
        }

        //TODO merge this with parse later... just to keep it clean
        private void ParseText2(String text, ParseTrie tree, int cat)
        {
            foreach (String word in text.Split(' '))
            {
                foreach (Reader.ApplProduction ap in Predict(cat, prods))
                {

                }
            }
        }

        public void ParseWithRecovery(String language, String text)
        {

        }

        private void PredictTEMP(int cat)
        {
            //TRY to predict the legal nextstates
            foreach (Reader.ApplProduction p in Predict(cat, prods))
            {
                foreach (int dom in p.Domain()) //Loop over all productions that we want
                {
                    foreach (Reader.ApplProduction p2 in Predict(dom, prods))
                    {
                        foreach (Reader.Sequence s in p2.function.sequences)
                        {
                            Console.WriteLine(GetSymbols(s.symbs));
                        }
                    }
                }
            }
        }
        //Returns all application productions in category cat 
        private List<Reader.ApplProduction> Predict(int cat, List<Reader.Production> _prods)
        {
            List<Reader.ApplProduction> appList = new List<Reader.ApplProduction>();
            foreach (Reader.Production p in _prods) 
            {
                if (p.fId == cat)
                {
                    if (p is Reader.ApplProduction)
                    {
                        appList.Add((Reader.ApplProduction)p);
                    }
                    else if (p is Reader.CoerceProduction)
                    {
                        appList.AddRange(UnCoerse(cat, _prods));
                    }
                }
            }
            return RemoveDoubles(appList);
        }
        //Tries to remove coersions... should work :)
        private List<Reader.ApplProduction> UnCoerse(int cat, List<Reader.Production> _prods)
        {
            List<Reader.ApplProduction> appList = new List<Reader.ApplProduction>();

            foreach (Reader.Production p in _prods)
            {
                if (p.fId == cat)
                {
                    if (p is Reader.CoerceProduction)
                    {
                        Reader.CoerceProduction cop = (Reader.CoerceProduction)p;
                        appList.AddRange(UnCoerse(cop.initId, _prods));
                    }
                    else if (p is Reader.ApplProduction)
                    {
                        appList.Add((Reader.ApplProduction)p);
                    }
                }
            }
            //Remove doubles?, could distinct be used here?
            return appList;
        }
        //Returns a string with the token symbols, only used for debug
        private String GetSymbols(List<Reader.Symbol> seq)
        {
            String all= " ";
            foreach (Reader.Symbol s in seq)
            {
                all += s + ", ";
            }
            return all;
        }
        //Returns the categories for the next step
        private List<int> GetCat(int cat)
        {
            List<int> cats = new List<int>();



            return cats;
        }

        private List<Reader.ApplProduction> RemoveDoubles(List<Reader.ApplProduction> _prods)
        {
            HashSet<Reader.ApplProduction> appList = new HashSet<Reader.ApplProduction>();
            foreach(Reader.ApplProduction p in _prods)
            {
                appList.Add(p);
            }
            return appList.ToList<Reader.ApplProduction>();
        }

        private String GetToken(Reader.ApplProduction ap)
        {
            String token ="";
            foreach (Reader.Sequence seq in ap.function.sequences)
            {
                foreach (Reader.Symbol symb in seq.symbs)
                {
                    if (symb is Reader.ToksSymbol)
                    {
                        Reader.ToksSymbol t = (Reader.ToksSymbol)symb;
                        //TODO
                        foreach (String tok in t.tokens)
                        {
                            token += tok + " ";
                        }
                        
                    }
                    else if (symb is Reader.AlternToksSymbol)
                    {
                        Reader.AlternToksSymbol t = (Reader.AlternToksSymbol)symb;
                        //TODO there is a list of strings (alt1?)
                        //pre...
                    }
                }
            }
            return token.TrimEnd();
        }
    }
}



