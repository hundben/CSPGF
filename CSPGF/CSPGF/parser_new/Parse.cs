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
        private List<reader.Production> prods;  //<- THIS SHOULD BE PRIVATE ;P
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
            //TODO use the category to create the parsestate
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
        private List<reader.ApplProduction> GetProductions(int cat, List<reader.Production> prods)
        {
            List<reader.ApplProduction> appList = new List<reader.ApplProduction>();
            foreach (reader.Production p in prods) 
            {
                if (p.fId == cat)
                {
                    if (p is reader.ApplProduction)
                    {
                        appList.Add((reader.ApplProduction)p);
                    }
                    else if (p is reader.CoerceProduction)
                    {
                        appList.AddRange(UnCoerse(cat,prods));
                    }
            }
            return appList;
        }
        //Tries to remove coersions... should work :)
        private List<reader.ApplProduction> UnCoerse(int cat, List<reader.Production> prods)
        {
            List<reader.ApplProduction> appList = new List<reader.ApplProduction>();

            foreach (reader.Production p in prods)
            {
                if (p.fId == cat)
                {
                    if (p is reader.CoerceProduction)
                    {
                        reader.CoerceProduction cop = (reader.CoerceProduction)p;
                        //Can be optimized since there is only one domain in coerce
                        foreach (int domCat in cop.GetDomain())
                        {
                            appList.AddRange(UnCoerse(domCat, prods));
                        }
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
    }
}



