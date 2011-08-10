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

    public class IndexedPGFTest
    {
        public IndexedPGFTest(string name)
        {
        }

        public void TestIndexedPhrasebookSelect()
        {
            //String filename = this.getClass().getResource("PhrasebookIndexed.pgf").getFile();
            List<string> tmp = new List<string>();
            tmp.Add("PhrasebookEng");
            tmp.Add("PhrasebookFre");
            PGF pgf = PGFBuilder.FromFile("PhrasebookIndexed.pgf", tmp);
            
            Debug.Assert(pgf.HasConcrete("PhrasebookEn"), "Check if the pgf has the concrete we're after");
            Debug.Assert(pgf.HasConcrete("PhrasebookFre"), "Check if the pgf has the concrete we're after");
            Debug.Assert(!pgf.HasConcrete("PhrasebookIta"), "Check that we don't have this concrete");
        }

        public void TestIndexedPhrasebookAll()
        {
            //String filename = this.getClass().getResource("PhrasebookIndexed.pgf").getFile();
            PGF pgf = PGFBuilder.FromFile("PhrasebookIndexed.pgf");
            Debug.Assert(pgf.HasConcrete("PhrasebookEn"), "Check if the pgf has the concrete we're after");
            Debug.Assert(pgf.HasConcrete("PhrasebookFre"), "Check if the pgf has the concrete we're after");
            Debug.Assert(pgf.HasConcrete("PhrasebookIta"), "Check if the pgf has the concrete we're after");
        }

        public void TestUnknownLanguage()
        {
            //String filename = this.getClass().getResource("Phrasebook.pgf").getFile();
            try 
            {
                List<string> tmp = new List<string>();
                tmp.Add("PhrasebookEng");
                tmp.Add("PhrasebookBORK");
                PGF pgf = PGFBuilder.FromFile("Phrasebook.pgf", tmp);
                Debug.Fail("PGFBuilder failed to raise an exception when an unknown language is selected.");
            }
            catch (UnknownLanguageException e) 
            {
                System.Console.WriteLine(e.ToString()); 
            }
        }

        public void TestUninexedFoodsSelect()
        {
            //String filename = this.getClass().getResource("Foods.pgf").getFile();
            List<string> tmp = new List<string>();
            tmp.Add("FoodsIta");
            PGF pgf = PGFBuilder.FromFile("Foods.pgf", tmp);
            Debug.Assert(pgf.HasConcrete("FoodsIta"), "Check if the pgf has the concrete we're after");
            Debug.Assert(!pgf.HasConcrete("FoodsFre"), "Check if the pgf has the concrete we're after");
        }

        public void TestUninexedFoodsAll()
        {
            //String filename = this.getClass().getResource("Foods.pgf").getFile();
            PGF pgf = PGFBuilder.FromFile("Foods.pgf");
            Debug.Assert(pgf.HasConcrete("FoodsIta"), "Check if the pgf has the concrete we're after");
            Debug.Assert(pgf.HasConcrete("FoodsFre"), "Check if the pgf has the concrete we're after");
        }
    }
}
