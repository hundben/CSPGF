using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser_new
{
    class ParseState
    {
        private Chart chart;
        private reader.Concrete cnc;
        private reader.Abstract abs;
        private Dictionary<int, int> trie;  //TODO check datatypes for this one...

        public ParseState(reader.Concrete _cnc, reader.Abstract _abs)
        {
            cnc = _cnc;
            abs = _abs;
        }
        /*
         * The things below should be here :)
         in PState abs
                    cnc
                    (Chart emptyAC [] emptyPC (pproductions cnc) (totalCats cnc) 0) //emptyPC = Map.empty emptytAC = IntMap.empty (key = int)
                    (TrieMap.compose (Just (Set.fromList items)) acc)
         *          AC = active chart
         *          PC = passive chart
         *          AK = active key
         *          abs = abstract
         *          cnc = concrete
         *          
         */
    }
    
}
