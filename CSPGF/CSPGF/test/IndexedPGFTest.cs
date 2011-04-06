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

namespace CSPGF.test
{
    class IndexedPGFTest
    {
        public IndexedPGFTest(String name)
        {

        }

        public void testIndexedPhrasebookSelect()
        {
            //String filename = this.getClass().getResource("PhrasebookIndexed.pgf").getFile();
            List<String> tmp = new List<String>();
            tmp.Add("PhrasebookEng");
            tmp.Add("PhrasebookFre");
            PGF pgf = PGFBuilder.FromFile("PhrasebookIndexed.pgf", tmp);
            
            Debug.Assert(pgf.HasConcrete("PhrasebookEn"));
            Debug.Assert(pgf.HasConcrete("PhrasebookFre"));
            Debug.Assert(!pgf.HasConcrete("PhrasebookIta"));
        }

        public void testIndexedPhrasebookAll()
        {
            //String filename = this.getClass().getResource("PhrasebookIndexed.pgf").getFile();
            PGF pgf = PGFBuilder.FromFile("PhrasebookIndexed.pgf");
            Debug.Assert(pgf.HasConcrete("PhrasebookEn"));
            Debug.Assert(pgf.HasConcrete("PhrasebookFre"));
            Debug.Assert(pgf.HasConcrete("PhrasebookIta"));
        }

        public void testUnknownLanguage()
        {
            //String filename = this.getClass().getResource("Phrasebook.pgf").getFile();
            try {
                List<String> tmp = new List<String>();
                tmp.Add("PhrasebookEng");
                tmp.Add("PhrasebookBORK");
                PGF pgf = PGFBuilder.FromFile("Phrasebook.pgf", tmp);
                Debug.Fail("PGFBuilder failed to raise an exception when an unknown language is selected.");
            }
            catch (UnknownLanguageException e) { System.Console.WriteLine(e.ToString()); }
        }

        public void testUninexedFoodsSelect()
        {
            //String filename = this.getClass().getResource("Foods.pgf").getFile();
            List<String> tmp = new List<String>();
            tmp.Add("FoodsIta");
            PGF pgf = PGFBuilder.FromFile("Foods.pgf", tmp);
            Debug.Assert(pgf.HasConcrete("FoodsIta"));
            Debug.Assert(!pgf.HasConcrete("FoodsFre"));
        }

        public void testUninexedFoodsAll()
        {
            //String filename = this.getClass().getResource("Foods.pgf").getFile();
            PGF pgf = PGFBuilder.FromFile("Foods.pgf");
            Debug.Assert(pgf.HasConcrete("FoodsIta"));
            Debug.Assert(pgf.HasConcrete("FoodsFre"));
        }
    }
}
