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
            PGF pgf = PGFBuilder.FromFile("PhrasebookIndexed.pgf", new String[] { "PhrasebookEng", "PhrasebookFre" });
            
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
                PGF pgf = PGFBuilder.FromFile("Phrasebook.pgf", new String[] { "PhrasebookEng", "PhrasebookBORK" });
                Debug.Fail("PGFBuilder failed to raise an exception when an unknown language is selected.");
            }
            catch (UnknownLanguageException e) { System.Console.WriteLine(e.ToString()); }
        }

        public void testUninexedFoodsSelect()
        {
            //String filename = this.getClass().getResource("Foods.pgf").getFile();
            PGF pgf = PGFBuilder.FromFile("Foods.pgf", new String[] { "FoodsIta" });
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
