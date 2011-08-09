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

namespace CSPGF.Linearize
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CSPGF.Reader;
    /**
     * This class represent a 'pre' object.
     * that is either an alternative between multiple lists of tokens
     * with condition on the following words and a default alternative.
     *
     * Example: pre( "parce que", "parce qu'"/"il", "parce qu'"/"on")
     * will be represented by a LeafKP with
     *   defaultTokens = ["parce","que"]
     *   alternatives = [ (["parce", "qu'"], ["il"])
     *                  , (["parce", "qu'"], ["on"]) ]
     **/
    public class LeafKP : BracketedTokn
    {
        public LeafKP(List<string> strs, List<Alternative> alts)
        {
            this.DefaultTokens = strs;
            this.Alternatives = alts;
        }

        public List<string> DefaultTokens { get; private set; }
        public List<Alternative> Alternatives { get; private set; }


        public override string ToString()
        {
            string rez = "string names : [";
            foreach (string str in this.DefaultTokens) 
            {
                rez += " " + str;
            }

            rez += "] , Alternatives : [";
            foreach (Alternative a in this.Alternatives) 
            {
                rez += " " + a.ToString();
            }

            rez += "]";
            return rez;
        }
    }
}
