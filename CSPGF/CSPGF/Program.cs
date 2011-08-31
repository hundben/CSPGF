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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using CSPGF.Parse;

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
            DateTime start = DateTime.Now;
            // Tests below.
            FileStream fs = new FileStream("..\\..\\test\\files\\Foods.pgf", FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            PGFReader pr = new PGFReader(br);
            PGF pgf = pr.ReadPGF();
            fs.Close();

            RecoveryParser rp = new RecoveryParser(pgf, "FoodsEng");
            rp.Scan("this");
            rp.Scan("wine");
            rp.Scan("is");
            rp.Scan("Italian");
            List<Trees.Absyn.Tree> trees = rp.GetTrees();
            System.Console.WriteLine(trees.Count);
            //Linearizer lin = new Linearizer(pgf, pgf.GetConcrete("FoodsGer"));
            //Linearizer2 lin2 = new Linearizer2(pgf, pgf.GetConcrete("FoodsGer"));
            //lin2.SetProductions(lin.LProds());
            //Linearizer lin = new Linearizer(pgf, pgf.GetConcrete("FoodsGer"));
            //rp.Debug3(pgf.GetConcrete("PhrasebookSwe"));
            //System.Console.WriteLine(lin.LinearizeString(trees[0]));
            rp.Debug2();
            //lin2.Linearize(trees[0]);
            //Linearizer lin = new Linearizer(pgf, pgf.GetConcrete("PhrasebookDan"));
            //System.Console.WriteLine(lin.LinearizeString(trees[0]));
            System.Console.Out.WriteLine("done");
            DateTime stop = DateTime.Now;
            TimeSpan span = start - stop;
            System.Console.WriteLine(span);
            System.Console.In.ReadLine();
        }
    }
}
