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

namespace CSPGF.Test
{
    using System.Collections.Generic;
    using System.Diagnostics;
    
    public class FoodsPredictTest : PGFTestCase
    {
        private PGF pgf;

        public FoodsPredictTest(string name) : base(name)
        {
        }

        public void SetUp()
        {
            this.pgf = GetPGF("Foods.pgf");
        }

        public void TestFoodsEng()
        {
            Parser parser = new Parser(this.pgf, "FoodsEng");
            string[] words = new string[] { "that", "these", "this", "those" };
            List<string> predictions = parser.Parse().Predict();
            predictions.Sort();
            Debug.Assert(words.Length == predictions.Count, "Check if the lengths are equal");
            for (int i = 0; i < words.Length; i++)
            {
                Debug.Assert(words[i].Equals(predictions[i]), "Check if the two are equal");
            }
        }

        public void TestFoodsSwe()
        {
            Parser parser = new Parser(this.pgf, "FoodsSwe");
            string[] words = new string[] { "de", "den", "det" };
            List<string> predictions = parser.Parse().Predict();
            predictions.Sort();
            Debug.Assert(words.Length == predictions.Count, "Check if the lengths are equal");
            for (int i = 0; i < words.Length; i++)
            {
                Debug.Assert(words[i].Equals(predictions[i]), "Check if the two are equal");
            }
        }

        public void TestFoodsIta()
        {
            Parser parser = new Parser(this.pgf, "FoodsIta");

            string[] words = new string[] { "quei", "quel", "quella", "quelle", "questa", "queste", "questi", "questo" };

            List<string> predictions = parser.Parse().Predict();
            predictions.Sort();
            Debug.Assert(words.Length == predictions.Count, "Check if the number of elements is equal");
            for (int i = 0; i < words.Length; i++)
            {
                Debug.Assert(words[i].Equals(predictions[i]), "Check if the two are equal");
            }
        }

        public void TearDown()
        {
            this.pgf = null;
        }
    }
}