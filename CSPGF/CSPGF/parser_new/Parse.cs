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


namespace CSPGF.parser_new
{
    class Parser
    {
        //Notice, the language might be bad here? better to use it when we want to parse a text?
        private PGF pgf;
        private List<reader.Production> prods;
        private reader.Abstract abs;
        private List<reader.CncCat> cncCat;
        private List<reader.CncFun> cncFun;
        private reader.Concrete concrete;
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
            cncFun = concrete.cncFuns;
            String startCatName = abs.StartCat();
            //This code might be unnecessary, but then we have the startcategory saved at least ;P
            reader.CncCat startCat = null;
            foreach (reader.CncCat cat in cncCat) {
                if (cat.name == startCatName) {
                    startCat = cat;
                    break;
                }
            }
            if (startCat == null) {
                throw new Exception("Start category " + startCatName + " not found!");
            }
            prods = concrete.GetProductions();  //load productions

            Predict(startCat.firstFID);
            System.Console.WriteLine("whaaa");
            //Map.Map CId CncCat = Map(Map CId CncCat whatever... :D
        }

        public void ParseWithRecovery(String language, String text)
        {

        }
        private void Predict(int cat)
        {
            //TODO use the categoreader.Prate
            //TRY to predict the legal nextstates
            foreach (reader.ApplProduction p in GetProductions(cat, prods))
            {
                foreach (int dom in p.Domain()) //Loop over all productions that we want
                {
                    foreach (reader.ApplProduction p2 in GetProductions(dom, prods))
                    {
                        foreach (reader.Sequence s in p2.function.sequences)
                        {
                            Console.WriteLine(GetSymbols(s.symbs));
                        }
                    }
                }
            }
        }
        //Returns all application productions in category cat 
        private List<reader.ApplProduction> GetProductions(int cat, List<reader.Production> _prods)
        {
            List<reader.ApplProduction> appList = new List<reader.ApplProduction>();
            foreach (reader.Production p in _prods) 
            {
                if (p.fId == cat)
                {
                    if (p is reader.ApplProduction)
                    {
                        appList.Add((reader.ApplProduction)p);
                    }
                    else if (p is reader.CoerceProduction)
                    {
                        appList.AddRange(UnCoerse(cat, _prods));
                    }
                }
            }
            return RemoveDoubles(appList);
        }
        //Tries to remove coersions... should work :)
        private List<reader.ApplProduction> UnCoerse(int cat, List<reader.Production> _prods)
        {
            List<reader.ApplProduction> appList = new List<reader.ApplProduction>();

            foreach (reader.Production p in _prods)
            {
                if (p.fId == cat)
                {
                    if (p is reader.CoerceProduction)
                    {
                        reader.CoerceProduction cop = (reader.CoerceProduction)p;
                        appList.AddRange(UnCoerse(cop.initId, _prods));
                    }
                    else if (p is reader.ApplProduction)
                    {
                        appList.Add((reader.ApplProduction)p);
                    }
                }
            }
            //Remove doubles?, could distinct be used here?
            return appList;
        }
        //Returns a string with the token symbols, only used for debug
        private String GetSymbols(List<reader.Symbol> seq)
        {
            String all= " ";
            foreach (reader.Symbol s in seq)
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

        private List<reader.ApplProduction> RemoveDoubles(List<reader.ApplProduction> _prods)
        {
            HashSet<reader.ApplProduction> appList = new HashSet<reader.ApplProduction>();
            foreach(reader.ApplProduction p in _prods)
            {
                appList.Add(p);
            }
            return appList.ToList<reader.ApplProduction>();
        }
    }
}



