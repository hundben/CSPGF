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
using CncFun = CSPGF.reader.CncFun;
using Symbol = CSPGF.reader.Symbol;

namespace CSPGF.parser
{
    class ActiveItem
    {
        public int begin;
        public int category;
        public CncFun function;
        public List<int> domain;
        public int constituent;
        public int position;
        public ActiveItem(int _begin, int _category, CncFun _function, List<int> _domain, int _constituent, int _position)
        {
            begin = _begin;
            category = _category;
            function = _function;
            domain = _domain;
            constituent = _constituent;
            position = _position;
        }
        //Notice that Option and Same is used together in scala, so ignore :D
        public Symbol NextSymbol()
        {
            if (position < function.GetSequence(constituent).symbs.Count) {
                Symbol sym = function.GetSequence(constituent).GetSymbol(position);
                return sym;
            }
            return null;    //this might be dangerous
        }
        //equals method
        public /*override*/ bool Equals(ActiveItem ai)
        {
            if (begin == ai.begin &&
                category == ai.category &&
                function == ai.function &&
                constituent == ai.constituent &&
                position == ai.position) {
                //Since there is no deep method in c# that we know of, use a crappy forloop
                if (domain.Count == ai.domain.Count) {
                    for (int i = 0; i < domain.Count; i++) {
                        if (domain[i] != ai.domain[i]) return false;
                    }
                    return true;
                }
            }

            return false;
        }
        //To string
        public override string ToString()
        {
            String str = "[" + begin.ToString() + ";" + category.ToString() +
                "->" + this.function.name + "[" + DomainToString() + "];" +
                constituent.ToString() + ";" + position.ToString() + "]";
            return str;
        }
        //Helper method
        public String DomainToString()
        {
            String tot = "";
            foreach (int d in domain) {
                tot += d.ToString();
            }
            return tot;
        }
    }
}
