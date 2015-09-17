using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CSPGF;

namespace CSPGFTest
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        /// Simple test to test a translation from english to english
        /// </summary>
        [TestMethod]
        public void TestMethod1()
        {
            AdvancedTranslator at = new AdvancedTranslator("../../pgf examples/Phrasebook.pgf");
            at.SetInputLanguage("PhrasebookEng");
            at.Scan("this");
            at.Scan("wine");
            at.Scan("is");
            at.Scan("delicious");
            at.Scan(".");
            at.SetOutputLanguage("PhrasebookEng");
            string check = at.Translate();
            Assert.IsTrue(check.Equals("this wine is delicious ."));
        }
    }
}

/*
            Thread.MemoryBarrier();
            long initialMemory = System.GC.GetTotalMemory(true);
            for (int i = 0; i < 10; i++)
            {

                
                AdvancedTranslator at = new AdvancedTranslator("../../pgf examples/Phrasebook.pgf");
                at.SetInputLanguage("PhrasebookEng");

                at.Scan("this");
                at.Scan("wine");
                at.Scan("is");
                at.Scan("delicious");
                at.Scan(".");
                Console.WriteLine(" --- Translation ---");
                at.SetOutputLanguage("PhrasebookEng");
                Console.WriteLine(at.Translate());
                
// Console.WriteLine(at.PrintTree(at.GetTrees()[0]));
// Wait for a keypress.

AdvancedTranslator at2 = new AdvancedTranslator("../../pgf examples/MiniLit.pgf");
at2.SetInputLanguage("MiniLitCnc");
                at2.SetOutputLanguage("MiniLitCnc");
                at2.Scan("flt");
                at2.Scan("(");
                at2.Scan("1.2");
                at2.Scan(")");
                Console.WriteLine(at2.Translate());
            }
            
            Thread.MemoryBarrier();
            long finalMemory = System.GC.GetTotalMemory(true);
Console.WriteLine(finalMemory - initialMemory);
            Console.ReadKey();
    */
