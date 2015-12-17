//-----------------------------------------------------------------------
// <copyright file="UnitTest1.cs" company="None">
//  Copyright (c) 2011, Christian Ståhlfors (christian.stahlfors@gmail.com), 
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

namespace CSPGFTest
{
    using CSPGF;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    
    /// <summary>
    /// Unittest class.
    /// </summary>
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

        /// <summary>
        /// Simple test to test literals.
        /// </summary>
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

        /// <summary>
        /// A simple translation test from English to Swedish.
        /// </summary>
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