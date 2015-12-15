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
        public void NormalTest1()
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

        [TestMethod]
        public void LiteralTest1()
        {
            AdvancedTranslator at = new AdvancedTranslator("../../pgf examples/MiniLit.pgf");
            at.SetInputLanguage("MiniLitCnc");
            at.SetOutputLanguage("MiniLitCnc");
            at.Scan("flt");
            at.Scan("(");
            at.Scan("1.2");
            at.Scan(")");
            string check = at.Translate();

            // TODO write this one
            Assert.IsTrue(false);

        }

        [TestMethod]
        public void TranslationTest1()
        {
            AdvancedTranslator at = new AdvancedTranslator("../../pgf examples/Phrasebook.pgf");
            at.SetInputLanguage("PhrasebookEng");
            at.Scan("this");
            at.Scan("wine");
            at.Scan("is");
            at.Scan("delicious");
            at.Scan(".");
            at.SetOutputLanguage("PhrasebookSwe");
            string check = at.Translate();
            Assert.IsTrue(check.Equals("det här vinet är läckert ."));
        }
    }
}

