//-----------------------------------------------------------------------
// <copyright file="Tests.cs" company="None">
//  Copyright (c) 2016, Christian Ståhlfors (christian.stahlfors@gmail.com), 
//   Erik Bergström (erktheorc@gmail.com) 
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without
//  modification, are permitted provided that the following conditions are met:
//   * Redistributions of source code must retain the above copyright
//     notice, this list of conditions and the following disclaimer.
//   * Redistributions in binary form must reproduce the above copyright
//     notice, this list of conditions and the following disclaimer in the
//     documentation and/or other materials provided with the distribution.
//   * Neither the name of the &lt;organization&gt; nor the
//     names of its contributors may be used to endorse or promote products
//     derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS &quot;AS IS&quot; AND
//  ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
//  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
//  DISCLAIMED. IN NO EVENT SHALL &lt;COPYRIGHT HOLDER&gt; BE LIABLE FOR ANY
//  DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
//  (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
//  LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
//  ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
//  (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// </copyright>
//-----------------------------------------------------------------------

namespace CSPGF
{
    using Grammar;
    using Parse;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Class containing unit tests.
    /// </summary>
    public class Tests
    {
        /// <summary>
        /// Simple test to try translation.
        /// </summary>
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
            string check = at.Translate().Trim();

            // TODO write this one
            Assert.Equal(check, "flt ( 1.2 )");
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

        [Fact]
        public void TestNewParser()
        {
            PGFReader pr = new PGFReader("../../PGF examples/MiniLit.pgf");
            PGF pgf = pr.ReadPGF();
            
            Concrete language = pgf.GetConcrete("MiniLitCnc");
            ParseState2 pstate = new ParseState2(language, language.GetStartCat());
            pstate.next("flt");
            pstate.next("(");
            pstate.next("1.2");
            pstate.next(")");

            var currentLin = new Linearize.Linearizer(pgf, language);
            List<Trees.Absyn.Tree> lt = pstate.GetTrees();
            Trees.Absyn.Tree t = lt[0];
            string check =  currentLin.LinearizeString(t);


            Assert.Equal(check, "flt ( 1.2 )");
        }
    }
}
