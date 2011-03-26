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
    public class Abstract
    {
        public String name { get; private set; } 
        private Dictionary<String, RLiteral> flags;
        public AbsFun[] absFuns { get; private set; }
        public AbsCat[] absCats { get; private set; }

        public Abstract(String _name, Dictionary<String, RLiteral> _flags, AbsFun[] _absFuns, AbsCat[] _absCats)
        {
            name = _name;
            flags = _flags;
            absFuns = _absFuns;
            absCats = _absCats;
        }

        public String StartCat()
        {
            RLiteral cat = flags["startcat"];
            if (cat == null)
                return "Sentence";
            else
                return ((StringLit)cat).value;

        }

        public override String ToString()
        {
            String ss = "Name : " + name + " , Flags : (";
            // TODO: Är bortkommenterat i javakoden också kanske borde fixa?
            // for(int i=0; i<flags.length;i++)
            // 	ss+=(" "+flags[i].toString());
            ss += ") , Abstract Functions : (";
            foreach (AbsFun a in absFuns) {
                ss += " " + a.ToString();
            }
            ss += ") , Abstract Categories : (";
            foreach (AbsCat a in absCats) {
                ss += " " + a.ToString();
            }
            ss += ")";
            return ss;
        }
    }
}
