//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="None">
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

namespace CSPGF
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The mainclass that runs the program
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Runs some tests.
        /// </summary>
        /// <param name="args">Commandline arguments</param>
        public static void Main(string[] args)
        {
            TempLog.NewLog();
            // Tests below.
            PGFReader pr = new PGFReader("..\\..\\test\\files\\Foods.pgf");
            PGFile pgf = pr.ReadPGF();
            PGFReader pr2 = new PGFReader("..\\..\\test\\files\\Phrasebook.pgf");
            PGFile pgf2 = pr2.ReadPGF();
            AdvancedTranslator rp = new AdvancedTranslator(pgf, "FoodsEng");

            rp.Scan("this");
            rp.Scan("wine");
            rp.Scan("is");
            rp.Scan("Italian");

            Linearizer lin = new Linearizer(pgf, pgf.GetConcrete("FoodsGer"));
            foreach (Trees.Absyn.Tree tree in rp.GetTrees())
            {
                System.Console.WriteLine(lin.LinearizeString(tree));
            }

            rp.Reset();
            rp.Scan("this");
            rp.Scan("fish");
            rp.Scan("is");
            rp.Scan("Italian");

            foreach (Trees.Absyn.Tree tree in rp.GetTrees())
            {
                System.Console.WriteLine(lin.LinearizeString(tree));
            }


            /*DebugParser rp = new DebugParser(pgf, "PhrasebookEng");
            rp.Scan("Finnish");
            rp.Scan("fish");
            rp.Scan("isn't");
            rp.Scan("too");
            rp.Scan("warm");
            rp.Scan(".");*/

            AdvancedTranslator rp2 = new AdvancedTranslator(pgf2, "PhrasebookEng");
            rp2.Scan("this");
            rp2.Scan("wine");
            rp2.RemoveToken();
            rp2.Scan("wine");
            rp2.Scan("is");
            rp2.Scan("Italian");
            rp2.Scan(".");
            Linearizer lin2 = new Linearizer(pgf2, pgf2.GetConcrete("PhrasebookGer"));

            foreach (Trees.Absyn.Tree tree in rp2.GetTrees())
            {
                System.Console.WriteLine(lin2.LinearizeString(tree));
            }
            System.Console.Out.WriteLine("done");
            System.Console.In.ReadLine();
        }
    }
}
