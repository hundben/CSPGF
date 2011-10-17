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
            FileStream fs2 = new FileStream("..\\..\\test\\files\\Phrasebook.pgf", FileMode.Open);
            FileStream fs = new FileStream("..\\..\\test\\files\\Foods.pgf", FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            PGFReader pr = new PGFReader(br);
            PGF pgf = pr.ReadPGF();
            fs.Close();
            BinaryReader br2 = new BinaryReader(fs2);
            PGFReader pr2 = new PGFReader(br2);
            PGF pgf2 = pr2.ReadPGF();
            fs2.Close();
            DebugParser rp = new DebugParser(pgf, "FoodsEng");

            rp.Print();
            rp.Scan("this");
            rp.Print();
            rp.Scan("wine");
            rp.Print();
            rp.Scan("is");
            rp.Print();
            rp.RemoveToken();
            rp.Print();
            rp.RemoveToken();
            rp.Print();
            rp.Scan("fish");
            rp.Print();
            rp.Scan("is");
            rp.Print();
            rp.Scan("Italian");
            rp.Print();

            Linearizer lin = new Linearizer(pgf, pgf.GetConcrete("FoodsGer"));
            /*DebugParser rp = new DebugParser(pgf, "PhrasebookEng");
            rp.Scan("Finnish");
            rp.Scan("fish");
            rp.Scan("isn't");
            rp.Scan("too");
            rp.Scan("warm");
            rp.Scan(".");*/

            DebugParser rp2 = new DebugParser(pgf2, "PhrasebookEng");
            rp2.Scan("this");
            rp2.Scan("wine");
            rp2.RemoveToken();
            rp2.Scan("wine");
            rp2.Scan("is");
            rp2.Scan("Italian");
            rp2.Scan(".");
            // Linearizer lin = new Linearizer(pgf, pgf.GetConcrete("PhrasebookGer"));
            Linearizer lin2 = new Linearizer(pgf2, pgf2.GetConcrete("PhrasebookGer"));

            foreach (Trees.Absyn.Tree tree in rp.GetTrees())
            {
                System.Console.WriteLine(lin.LinearizeString(tree));
                //System.Console.WriteLine(lin2.LinearizeString(tree));
            }

            
            foreach (Trees.Absyn.Tree tree in rp2.GetTrees())
            {
                //System.Console.WriteLine(lin.LinearizeString(tree));
                System.Console.WriteLine(lin2.LinearizeString(tree));
            }
            System.Console.Out.WriteLine("done");
            System.Console.In.ReadLine();
        }
    }
}
