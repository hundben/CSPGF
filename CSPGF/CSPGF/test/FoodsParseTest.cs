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
using CSPGF.trees.Absyn;
using System.Diagnostics;

namespace CSPGF.test
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
            pgf = getPGF("Foods.pgf");
        }

        public void testFoodsEng()
        {
            Parser parser = new Parser(pgf, "FoodsEng");

            String ex1 = "this fresh pizza is Italian";
            Tree tree1 = parseTree("((Pred (This ((Mod Fresh) Pizza))) Italian)");
            Tree[] trees1 = parser.Parse(ex1).GetTrees();
            Debug.Assert(trees1.Length == 1);
            Debug.Assert(trees1[0].Equals(tree1));

            String ex2 = "those boring fish are expensive";
            Tree tree2 = parseTree("((Pred (Those ((Mod Boring) Fish))) Expensive)");
            Tree[] trees2 = parser.Parse(ex2).GetTrees();
            Debug.Assert(trees2.Length == 1);
            Debug.Assert(trees2[0].Equals(tree2));
        }

        public void testFoodsSwe()
        {
            Parser parser = new Parser(pgf, "FoodsSwe");

            String ex1 = "den här läckra pizzan är färsk";
            Tree tree1 = parseTree("((Pred (This ((Mod Delicious) Pizza))) Fresh)");
            Tree[] trees1 = parser.Parse(ex1).GetTrees();
            Debug.Assert(trees1.Length == 1);
            Debug.Assert(trees1[0].Equals(tree1));
        }

        public void testFoodsIta()
        {
            Parser parser = new Parser(pgf, "FoodsIta");

            String ex1 = "questa pizza deliziosa è fresca";
            Tree tree1 = parseTree("((Pred (This ((Mod Delicious) Pizza))) Fresh)");
            Tree[] trees1 = parser.Parse(ex1).GetTrees();
            Debug.Assert(trees1.Length == 1);
            Debug.Assert(trees1[0].Equals(tree1));
        }


        public void tearDown()
        {
            pgf = null;
        }
    }
}