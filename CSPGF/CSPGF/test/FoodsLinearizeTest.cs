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
using System.Diagnostics;
using CSPGF.Trees.Absyn;

namespace CSPGF.Test
{
    class FoodsLinearizeTest : PGFTestCase
    {
        public FoodsLinearizeTest(String name) : base(name)
        {
        }

        PGF pgf;

        public void SetUp()
        {
            pgf = GetPGF("Foods.pgf");
        }

        public void TestFoodsEng()
        {
            Linearizer linearizer = new Linearizer(pgf, "FoodsEng");

            String ex1 = "this fresh pizza is Italian";
            Tree tree1 = ParseTree("((Pred (This ((Mod Fresh) Pizza))) Italian)");
            String lin1 = linearizer.LinearizeString(tree1);
            Debug.Assert(ex1.Equals(lin1));

            String ex2 = "those boring fish are expensive";
            Tree tree2 = ParseTree("((Pred (Those ((Mod Boring) Fish))) Expensive)");
            String lin2 = linearizer.LinearizeString(tree2);
            Debug.Assert(ex2.Equals(lin2));
        }

        public void TestFoodsSwe()
        {
            Linearizer linearizer = new Linearizer(pgf, "FoodsSwe");

            Tree tree1 = ParseTree("((Pred (This ((Mod Delicious) Pizza))) Fresh)");
            String ex1 = "den här läckra pizzan är färsk";
            String lin1 = linearizer.LinearizeString(tree1);
            Debug.Assert(ex1.Equals(lin1));
        }

        public void TestFoodsIta()
        {
            Linearizer linearizer = new Linearizer(pgf, "FoodsIta");

            String ex1 = "questa pizza deliziosa è fresca";
            Tree tree1 = ParseTree("((Pred (This ((Mod Delicious) Pizza))) Fresh)");
            String lin1 = linearizer.LinearizeString(tree1);
            Debug.Assert(ex1.Equals(lin1));
        }


        public void TearDown()
        {
            pgf = null;
        }
    }
}
