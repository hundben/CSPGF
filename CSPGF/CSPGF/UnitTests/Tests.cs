
using Xunit;

namespace CSPGF
{
    public class Tests
    {
        [Fact]
        public void NormalTest1()
        {
            AdvancedTranslator at = new AdvancedTranslator("../../PGF examples/Phrasebook.pgf");
            at.SetInputLanguage("PhrasebookEng");
            at.Scan("this");
            at.Scan("wine");
            at.Scan("is");
            at.Scan("delicious");
            at.Scan(".");
            at.SetOutputLanguage("PhrasebookEng");
            string check = at.Translate();
            Assert.Equal(check.Equals("this wine is delicious ."), true);
        }

        /// <summary>
        /// Simple test to test literals.
        /// </summary>
        [Fact]
        public void LiteralTest1()
        {
            AdvancedTranslator at = new AdvancedTranslator("../../PGF examples/MiniLit.pgf");
            at.SetInputLanguage("MiniLitCnc");
            at.SetOutputLanguage("MiniLitCnc");
            at.Scan("flt");
            at.Scan("(");
            at.Scan("1.2");
            at.Scan(")");
            string check = at.Translate();

            // TODO write this one
            Assert.Equal(false, true);
        }

        /// <summary>
        /// A simple translation test from English to Swedish.
        /// </summary>
        [Fact]
        public void TranslationTest1()
        {
            AdvancedTranslator at = new AdvancedTranslator("../../PGF examples/Phrasebook.pgf");
            at.SetInputLanguage("PhrasebookEng");
            at.Scan("this");
            at.Scan("wine");
            at.Scan("is");
            at.Scan("delicious");
            at.Scan(".");
            at.SetOutputLanguage("PhrasebookSwe");
            string check = at.Translate();
            Assert.Equal(check.Equals("det här vinet är läckert ."), true);
        }

        /// <summary>
        /// A simple test to try to parse the new version of the PGF file format.
        /// </summary>
        [Fact]
        public void TestNewPGF()
        {
            AdvancedTranslator at = new AdvancedTranslator("../../PGF examples/Phrasebook_new.pgf");
            at.SetInputLanguage("PhrasebookEng");
            at.Scan("this");
            at.Scan("wine");
            at.Scan("is");
            at.Scan("delicious");
            at.Scan(".");
            at.SetOutputLanguage("PhrasebookEng");
            string check = at.Translate();
            Assert.Equal(check.Equals("this wine is delicious ."), true);
        }
    }
}
