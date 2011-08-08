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
using CSPGF.Trees.Absyn;
using System.Diagnostics;
using CSPGF.Parse;

namespace CSPGF.Test
{
    class FoodsParseTest : PGFTestCase
    {

        public FoodsParseTest(String name)
            : base(name)
        {

        }

        PGF pgf;

        public void setUp()
        {
            pgf = GetPGF("Foods.pgf");
        }

        public void TestFoodsEng()
        {
            Parser parser = new Parser(pgf, "FoodsEng");

            String ex1 = "this fresh pizza is Italian";
            //Tree tree1 = ParseTree("((Pred (This ((Mod Fresh) Pizza))) Italian)");
            //List<Tree> trees1 = parser.Parse(ex1).GetTrees()
            ParseState ps = parser.Parse(ex1);
            List<CSPGF.Trees.Absyn.Tree> trees1 = ps.GetTrees();
            Debug.Assert(trees1.Count == 1);
            //Debug.Assert(trees1[0].Equals(tree1));

            String ex2 = "those boring fish are expensive";
            //Tree tree2 = ParseTree("((Pred (Those ((Mod Boring) Fish))) Expensive)");
            List<CSPGF.Trees.Absyn.Tree> trees2 = parser.Parse(ex2).GetTrees();
            Debug.Assert(trees2.Count == 1);
            //Debug.Assert(trees2[0].Equals(tree2));
        }

        public void TestFoodsSwe()
        {
            Parser parser = new Parser(pgf, "FoodsSwe");

            String ex1 = "den här läckra pizzan är färsk";
            CSPGF.Trees.Absyn.Tree tree1 = ParseTree("((Pred (This ((Mod Delicious) Pizza))) Fresh)");
            List<CSPGF.Trees.Absyn.Tree> trees1 = parser.Parse(ex1).GetTrees();
            Debug.Assert(trees1.Count == 1);
            Debug.Assert(trees1[0].Equals(tree1));
        }

        public void TestFoodsIta()
        {
            Parser parser = new Parser(pgf, "FoodsIta");

            String ex1 = "questa pizza deliziosa è fresca";
            CSPGF.Trees.Absyn.Tree tree1 = ParseTree("((Pred (This ((Mod Delicious) Pizza))) Fresh)");
            List<CSPGF.Trees.Absyn.Tree> trees1 = parser.Parse(ex1).GetTrees();
            Debug.Assert(trees1.Count == 1);
            Debug.Assert(trees1[0].Equals(tree1));
        }


        public void TearDown()
        {
            pgf = null;
        }
    }
}
