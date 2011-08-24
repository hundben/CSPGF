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
    using System;
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
            FileStream fs = new FileStream("..\\..\\test\\files\\Foods.pgf", FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            PGFReader pr = new PGFReader(br);
            PGF pgf = pr.ReadPGF();
            fs.Close();
            ParseState st = new ParseState(pgf.GetConcrete("FoodsEng"));
            List<string> temp = st.Predict();
            System.Console.Out.WriteLine("scan this...");
            st.Scan("this");
            st.Scan("wine");
            st.Scan("is");
            st.Scan("Italian");
            List<CSPGF.Trees.Absyn.Tree> trees = st.GetTrees();
            temp = st.Predict();
            foreach (string s in temp)
            {
                System.Console.Out.WriteLine(s);
            }

            // Try deep copy
            try
            {
                ParseState st2 = ObjectCopier.Clone<ParseState>(st);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e.ToString());
            }

            //Linearizer lin = new Linearizer(pgf, pgf.GetConcrete("FoodsEng"));
            //string temp123 = lin.LinearizeString(trees[0]);
            // SpeechSynthesizer ss = new SpeechSynthesizer();
            // ss.SetOutputToDefaultAudioDevice();
            // ss.Speak("wheeeeee!");
            System.Console.Out.WriteLine("done");
            System.Console.In.ReadLine();
        }
    }
}
