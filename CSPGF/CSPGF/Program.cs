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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CSPGF.parser;
using CSPGF.test;
using CSPGF.trees;
using System.Speech;
using System.Speech.Recognition;
using System.Speech.Synthesis;

namespace CSPGF
{
    class Program
    {
        static void Main(string[] args)
        {
            BinaryReader br = new BinaryReader(new FileStream("..\\..\\test\\files\\Foods.pgf", FileMode.Open));
            PGFReader pr = new PGFReader(br);
            PGF tmp = pr.ReadPGF();
            parser_new.Parser ps = new parser_new.Parser(tmp);
            ps.ParseText("FoodsIta","hello world");
            //Parser parser = new Parser(tmp, "FoodsEng");
            //ParseState tmp2 = parser.Parse("hello world");
            //List<CSPGF.trees.Absyn.Tree> tmp3 = tmp2.GetTrees();

            //SpeechSynthesizer ss = new SpeechSynthesizer();
            //ss.SetOutputToDefaultAudioDevice();
            //ss.Speak("wheeeeee!");
            System.Console.In.ReadLine();
        }
    }
}
