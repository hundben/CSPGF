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
