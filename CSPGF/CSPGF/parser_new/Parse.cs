using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser_new
{
    class Parse
    {
        //Notice, the language might be bad here? better to use it when we want to parse a text?
        private PGF pgf;    
        public Parse(PGF _pgf)
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
        public void Parse(String language, String text)
        {
            //Check if the language exists
            reader.Concrete concrete = pgf.GetConcrete(language);
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
                      Just (CncCat s e labels) ->
                              let keys = do fid <- range (s,e)
                                            lbl <- indices labels
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
                (Chart emptyAC [] emptyPC (pproductions cnc) (totalCats cnc) 0)
                (TrieMap.compose (Just (Set.fromList items)) acc)
            where
                abs = abstract pgf
                cnc = lookConcrComplete pgf lang    //my concrete?

                flit _ = Nothing

                ftok = Map.unionWith (TrieMap.unionWith Set.union)

             */
            //lookConcrComplete :: PGF -> CId -> Concr
            reader.Abstract abs = pgf.GetAbstract();   //TODO maybe not necessary...
            List <reader.CncCat> tmp = concrete.GetCncCat();    //is this the same as cnccat cnc?
            //Map.Map CId CncCat = Map(Map CId CncCat whatever... :D
        }

        public void ParseWithRecovery(String language, String text)
        {

        }

    }
}
