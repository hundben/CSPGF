using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CSPGF.trees.Absyn;

namespace CSPGF.test
{
    class FoodsLinearizeTest : PGFTestCase
    {
        public FoodsLinearizeTest(String name) : base(name)
        {
        }

        PGF pgf;

        public void setUp()
        {
            pgf = getPGF("Foods.pgf");
        }

        public void testFoodsEng()
        {
            Linearizer linearizer = new Linearizer(pgf, "FoodsEng");

            String ex1 = "this fresh pizza is Italian";
            Tree tree1 = parseTree("((Pred (This ((Mod Fresh) Pizza))) Italian)");
            String lin1 = linearizer.LinearizeString(tree1);
            Debug.Assert(ex1.Equals(lin1));

            String ex2 = "those boring fish are expensive";
            Tree tree2 = parseTree("((Pred (Those ((Mod Boring) Fish))) Expensive)");
            String lin2 = linearizer.LinearizeString(tree2);
            Debug.Assert(ex2.Equals(lin2));
        }

        public void testFoodsSwe()
        {
            Linearizer linearizer = new Linearizer(pgf, "FoodsSwe");

            Tree tree1 = parseTree("((Pred (This ((Mod Delicious) Pizza))) Fresh)");
            String ex1 = "den här läckra pizzan är färsk";
            String lin1 = linearizer.LinearizeString(tree1);
            Debug.Assert(ex1.Equals(lin1));
        }

        public void testFoodsIta()
        {
            Linearizer linearizer = new Linearizer(pgf, "FoodsIta");

            String ex1 = "questa pizza deliziosa è fresca";
            Tree tree1 = parseTree("((Pred (This ((Mod Delicious) Pizza))) Fresh)");
            String lin1 = linearizer.LinearizeString(tree1);
            Debug.Assert(ex1.Equals(lin1));
        }


        public void tearDown()
        {
            pgf = null;
        }
    }
}
