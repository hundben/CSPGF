using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CSPGF.test
{
    class FoodsPredictTest : PGFTestCase
    {
        public FoodsPredictTest(String name) : base(name)
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
            String[] words = new String[] { "that", "these", "this", "those" };
            String[] predictions = parser.Parse().Predict();
            Array.Sort(predictions);
            Debug.Assert(words.Length == predictions.Length);
            for (int i = 0; i < words.Length; i++)
                Debug.Assert(words[i].Equals(predictions[i]));
        }

        public void testFoodsSwe()
        {
            Parser parser = new Parser(pgf, "FoodsSwe");
            String[] words = new String[] { "de", "den", "det" };
            String[] predictions = parser.Parse().Predict();
            Array.Sort(predictions);
            Debug.Assert(words.Length == predictions.Length);
            for (int i = 0; i < words.Length; i++)
                Debug.Assert(words[i].Equals(predictions[i]));
        }

        public void testFoodsIta()
        {
            Parser parser = new Parser(pgf, "FoodsIta");

            String[] words = new String[] {"quei", "quel",	"quella", "quelle",
					"questa", "queste", "questi", "questo"};

            String[] predictions = parser.Parse().Predict();
            Array.Sort(predictions);
            Debug.Assert(words.Length == predictions.Length);
            for (int i = 0; i < words.Length; i++)
                Debug.Assert(words[i].Equals(predictions[i]));
        }


        public void tearDown()
        {
            pgf = null;
        }
    }
}
