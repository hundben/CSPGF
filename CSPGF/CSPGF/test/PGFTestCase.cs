﻿/*
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
using CSPGF.trees;
using CSPGF.trees.Absyn;
using System.IO;

namespace CSPGF.test
{
    class PGFTestCase
    {
        String name;
        public PGFTestCase(String _name)
        {
            name = _name;
        }

        protected PGF getPGF(String filename)
        {
            //String fullname = this.getClass().getResource(filename).getFile();
            PGF pgf = PGFBuilder.FromFile(filename);
            return pgf;
        }

        protected Tree parseTree(String s)
        {
            Scanner l = new Scanner(new StreamReader(s).BaseStream);
            CSPGF.trees.Parser p = new CSPGF.trees.Parser(l);
            try {
                Tree parse_tree = p.ParseTree();
                return parse_tree;
            }
            catch (Exception e) {
                System.Console.WriteLine(e.ToString());
                return null;
            }
        }
    }
}