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

namespace CSPGF.reader
{
    public class CncCat
    {
        /**
         * Concrete category are a maping from category names (abstract-categories)
         * to multiple, conjoint, concrete categories.
         * They are represented in the pgf binary by :
         *  - the name of the abstract category (ex: Adj)
         *  - the first concrete categoy (ex : C18)
         *  - the last corresponding concrete category (ex : C21)
         *  - a list of labels (names of fields in the pmcfg tuples)
         * Here we will keep only the indices.
         */
        public String name { get; private set; }
        public int firstFID { get; private set; }
        public int lastFID { get; private set; }
        private String[] labels; //TODO: was commented out, have to check later.

        public CncCat(String _name, int _firstFId, int _lastFId, String[] _labels)
        {
            name = _name;
            firstFID = _firstFId;
            lastFID = _lastFId;
            labels = _labels; // was also commented out.
        }

        public override String ToString()
        {
            return name + " [C" + firstFID + " ... C" + lastFID + "]";
        }
    }
}
