/*
Copyright (c) 2011, Christian Ståhlfors (christian.stahlfors@gmail.com), Erik Bergström (erktheorc@gmail.com)
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

namespace CSPGF
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Speech;
    using System.Speech.Recognition;
    using System.Speech.Synthesis;
    using System.Text;
    using CSPGF.Parse;
    using CSPGF.Test;
    using CSPGF.Trees;
    
    public class Program
    {
        public static void Main(string[] args)
        {
            BinaryReader br = new BinaryReader(new FileStream("..\\..\\test\\files\\Foods.pgf", FileMode.Open));
            PGFReader pr = new PGFReader(br);
            PGF pgf = pr.ReadPGF();
            Parser_new.Parser ps = new Parser_new.Parser(pgf);
            //ps.ParseText("FoodsEng","hello world");
            //Parser parser = new Parser(tmp, "FoodsEng");
            ParseState st = new ParseState(pgf.GetConcrete("FoodsEng"));
            List<string> temp = st.Predict();
            foreach (string s in temp) 
            { 
                System.Console.Out.WriteLine(s); 
            }

            System.Console.Out.WriteLine("scan this...");
            st.Scan("this");    //TODO check why it locks here (endless loop :D
            
            //st.Scan("is");
            //st.Scan("expensive");
            //List<CSPGF.trees.Absyn.Tree> trees = st.GetTrees();
            temp = st.Predict();
            foreach (string s in temp)
            {
                System.Console.Out.WriteLine(s);
            }
            //ParseState tmp2 = parser.Parse("hello world");
            //List<CSPGF.trees.Absyn.Tree> tmp3 = tmp2.GetTrees();
            //SpeechSynthesizer ss = new SpeechSynthesizer();
            //ss.SetOutputToDefaultAudioDevice();
            //ss.Speak("wheeeeee!");
            System.Console.Out.WriteLine("done");
            System.Console.In.ReadLine();
        }
    }
}
