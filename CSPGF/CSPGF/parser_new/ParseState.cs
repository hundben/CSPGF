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

    public class ParseState
    {
        private Chart Chart;
        private Reader.Concrete Cnc;
        private Reader.Abstract Abs;
        private Dictionary<string, object> Trie;  // TODO check datatypes for this one...
        private List<int> ActiveState;

        public ParseState(Reader.Concrete cnc, Reader.Abstract abs)
        {
            this.Cnc = cnc;
            this.Abs = abs;
            this.ActiveState = new List<int>();  //what to do? :'(
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
