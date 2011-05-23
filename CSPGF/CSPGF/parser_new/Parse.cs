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
        //Methods we need
        /* Do we need type?
         *  data Type =
                DTyp [Hypo] CId [Expr]
                deriving (Eq,Ord,Show)
 
         */
        //TODO should return tree
        public void ParseText(String language, String text)
        {
            //Check if the language exists
            concrete = pgf.GetConcrete(language);
            String[] tokens = text.Split(' ');  //tokenize
            //Parse.parse dp = depth? 4 is used in haskell, but nothing works also?
            /*
             * parse :: PGF -> Language -> Type -> Maybe Int -> [Token] -> (ParseOutput,BracketedString)
                    parse pgf lang typ dp toks = loop (initState pgf lang typ) toks
                    where
                        loop ps []     = getParseOutput ps typ dp
                        loop ps (t:ts) = case nextState ps (simpleParseInput t) of
                            Left  es -> case es of
                                     EState _ _ chart -> (ParseFailed (offset chart),snd (getParseOutput ps typ dp))
                            Right ps -> loop ps ts
             */
            //we need initstate
            /*
             initState :: PGF -> Language -> Type -> ParseState
                initState pgf lang (DTyp _ start _) =
                let (acc,items) = case Map.lookup start (cnccats cnc) of
                      Just (CncCat s e labels) ->   //vi är här, s=start? fid? och e=end? lastid? labels = labels :D
                              let keys = do fid <- range (s,e)  //range mellan s och e 
                                            lbl <- indices labels //tar ut alla indices ur labels
                                            return (AK fid lbl)
                              in foldl' (\(acc,items) key -> predict flit ftok cnc
                                                                     (pproductions cnc)
                                                                     key key 0
                                                                     acc items)
                                        (Map.empty,[])
                                        keys
                      Nothing -> (Map.empty,[])
                in PState abs
                    cnc
                    (Chart emptyAC [] emptyPC (pproductions cnc) (totalCats cnc) 0) //emptyPC = Map.empty emptytAC = IntMap.empty (key = int)
                    (TrieMap.compose (Just (Set.fromList items)) acc)
            where
                abs = abstract pgf
                cnc = lookConcrComplete pgf lang    //my concrete? yup

                flit _ = Nothing

                ftok = Map.unionWith (TrieMap.unionWith Set.union)

             */
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
        private List<reader.ApplProduction> GetProductions(int cat, List<reader.Production> prods)
        {
            List<reader.ApplProduction> appList = new List<reader.ApplProduction>();
            foreach (reader.Production p in prods)
            {
                if (p.fId == cat && p is reader.ApplProduction)
                {
                    appList.Add((reader.ApplProduction)p);
                }
            }
            return appList;
        }
        private String GetSymbols(List<reader.Symbol> seq)
        {
            String all= " ";
            foreach (reader.Symbol s in seq)
            {
                all += s + ", ";
            }
            return all;
        }

    }
}


/*
predict flit ftok cnc forest key0 key@(AK fid lbl) k acc items =
  let (acc1,items1) = case IntMap.lookup fid forest of
                        Nothing  -> (acc,items)
                        Just set -> Set.fold foldProd (acc,items) set

      (acc2,items2) = case IntMap.lookup fid (lexicon cnc) >>= IntMap.lookup lbl of
                        Just tmap -> let (mb_v,toks) = TrieMap.decompose (TrieMap.map (toItems key0 k) tmap)
                                         acc1'   = ftok toks acc1
                                         items1' = maybe [] Set.toList mb_v ++ items1
                                     in (acc1',items1')
                        Nothing   -> (acc1,items1)
  in (acc2,items2)
  where
    foldProd (PCoerce fid)         (acc,items) = predict flit ftok cnc forest key0 (AK fid lbl) k acc items
    foldProd (PApply funid args)   (acc,items) = (acc,Active k 0 funid (rhs funid lbl) args key0 : items)
    foldProd (PConst _ const toks) (acc,items) = (acc,items)

    rhs funid lbl = unsafeAt lins lbl
      where
        CncFun _ lins = unsafeAt (cncfuns cnc) funid

    toItems key@(AK fid lbl) k funids =
      Set.fromList [Active k 1 funid (rhs funid lbl) [] key | funid <- IntSet.toList funids]

*/