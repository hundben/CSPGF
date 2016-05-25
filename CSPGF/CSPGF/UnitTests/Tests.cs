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
    using System;
    using System.Collections.Generic;
    using Grammar;
    using Parse;
    using Xunit;
    using Xunit.Abstractions;

    /// <summary>
    /// Class containing unit tests.
    /// </summary>
    public class Tests
    {
        /// <summary>
        /// Used for output
        /// </summary>
        private readonly ITestOutputHelper output;

        /// <summary>
        /// Initializes a new instance of the Tests class.
        /// </summary>
        /// <param name="output">Used for xUnit output</param>
        public Tests(ITestOutputHelper output)
        {
            this.output = output;
        }

        /// <summary>
        /// A simple translation test from English to Swedish.
        /// </summary>
        [Fact]
        public void TranslationTest1()
        {
            string check = string.Empty;
            try
            {
                string filename = "../../PGF examples/Phrasebook.pgf";
                PGFReader pr = new PGFReader(filename);
                PGF pgf = pr.ReadPGF();
                Concrete language_from = pgf.GetConcrete("PhrasebookEng");
                Concrete language_to = pgf.GetConcrete("PhrasebookSwe");
                ParseState pstate = new ParseState(language_from);
                pstate.Next("this");
                pstate.Next("wine");
                pstate.Next("is");
                pstate.Next("delicious");
                pstate.Next(".");
                var currentLin = new Linearize.Linearizer(pgf, language_to);
                List<Trees.Absyn.Tree> lt = pstate.GetTrees();
                Trees.Absyn.Tree t = lt[0];
                check = currentLin.LinearizeString(t);
            }
            catch (Exception e)
            {
                this.output.WriteLine("Error:" + e.Message + " | " + e.StackTrace.ToString());
            }

            Assert.Equal(check.Equals("det här vinet är läckert ."), true);
        }

        /// <summary>
        /// A simple test to try to parse the new version of the PGF file format.
        /// </summary>
        [Fact]
        public void TestNewPGF()
        {
            string check = string.Empty;

            try
            {
                string filename = "../../PGF examples/Phrasebook_new.pgf";
                PGFReader pr = new PGFReader(filename);
                PGF pgf = pr.ReadPGF();
                Concrete language = pgf.GetConcrete("PhrasebookEng");
                ParseState pstate = new ParseState(language);
                pstate.Next("this");
                pstate.Next("wine");
                pstate.Next("is");
                pstate.Next("delicious");
                pstate.Next(".");

                var currentLin = new Linearize.Linearizer(pgf, language);
                List<Trees.Absyn.Tree> lt = pstate.GetTrees();
                Trees.Absyn.Tree t = lt[0];
                check = currentLin.LinearizeString(t);
            }
            catch (Exception e)
            {
                this.output.WriteLine("Error:" + e.Message + " | " + e.StackTrace.ToString());
            }

            Assert.Equal(check.Equals("this wine is delicious ."), true);
        }

        /// <summary>
        /// A simple test to test that literals works
        /// </summary>
        [Fact]
        public void NewParserLiteral()
        {
            string check = string.Empty;
            try
            {
                string filename = "../../PGF examples/MiniLit.pgf";
                PGFReader pr = new PGFReader(filename);
                PGF pgf = pr.ReadPGF();

                Concrete language = pgf.GetConcrete("MiniLitCnc");
                ParseState pstate = new ParseState(language);
                pstate.Next("flt");
                pstate.Next("(");
                pstate.Next("1.2");
                pstate.Next(")");

                var currentLin = new Linearize.Linearizer(pgf, language);
                List<Trees.Absyn.Tree> lt = pstate.GetTrees();
                Trees.Absyn.Tree t = lt[0];
                check = currentLin.LinearizeString(t);
            }
            catch (Exception e)
            {
                this.output.WriteLine("Error:" + e.Message + " | " + e.StackTrace.ToString());
            }

            Assert.Equal("flt ( 1.2 )", check);
        }

        /// <summary>
        /// A small test to check if the parser and linearizer works.
        /// </summary>
        [Fact]
        public void ParserStandardTest()
        {
            string check = string.Empty;
            try
            {
                string filename = "../../PGF examples/Phrasebook.pgf";
                PGFReader pr = new PGFReader(filename);
                PGF pgf = pr.ReadPGF();
                Concrete language = pgf.GetConcrete("PhrasebookEng");
                ParseState pstate = new ParseState(language);
                pstate.Next("this");
                pstate.Next("wine");
                pstate.Next("is");
                pstate.Next("delicious");
                pstate.Next(".");
                var currentLin = new Linearize.Linearizer(pgf, language);
                List<Trees.Absyn.Tree> lt = pstate.GetTrees();
                Trees.Absyn.Tree t = lt[0];
                check = currentLin.LinearizeString(t);
            }
            catch (Exception e)
            {
                this.output.WriteLine("Error:" + e.Message + " | " + e.StackTrace.ToString());
            }

            Assert.Equal("this wine is delicious .", check);
        }
    }
}
