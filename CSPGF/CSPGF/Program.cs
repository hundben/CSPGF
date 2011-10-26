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
    using System.Speech.Recognition.SrgsGrammar;
    using System.Speech;
    using System.Speech.Recognition;
    using System;
    using System.Globalization;

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

            /*SrgsDocument sd = new SrgsDocument("..\\..\\test\\files\\FoodsEng.grxml");
            Grammar g = new Grammar(sd);
            CultureInfo test = new CultureInfo("en-US");
            SpeechRecognitionEngine sre = new SpeechRecognitionEngine(test);
            sre.SetInputToDefaultAudioDevice();
            sre.LoadGrammar(g);
            System.Console.WriteLine("Start speaking D: :"); 
            RecognitionResult rr = sre.Recognize(new TimeSpan(0, 0, 10));
            if (rr.Text == null || rr.Text == string.Empty)
            {
                System.Console.WriteLine("Failed");
            }
            else
            {
                System.Console.WriteLine(rr.Text);
            }*/

            TempLog.NewLog();
            // Tests below.

            AdvancedTranslator rp = new AdvancedTranslator("..\\..\\test\\files\\Bronzeage.pgf");
            rp.setInputLanguage("BronzeageEng");
            rp.Scan("I");
            rp.Scan("bite");
            //rp.Scan("him");
            rp.Scan("on");
            rp.Scan("a");
            rp.Scan("fingernail");
            rp.setOutputLanguage("BronzeageGer");
            System.Console.WriteLine(rp.Translate());
            /*foreach (string str in rp.Predict())
            {
                System.Console.WriteLine(str);
            }*/
            /*AdvancedTranslator rp = new AdvancedTranslator("..\\..\\test\\files\\Foods.pgf");
            rp.setInputLanguage("FoodsEng");
            rp.Scan("this");
            rp.Scan("wine");
            rp.Scan("is");
            rp.Scan("Italian");
            rp.setOutputLanguage("FoodsGer");
            System.Console.WriteLine(rp.Translate());

            rp.Reset();
            rp.Scan("this");
            rp.Scan("fish");
            rp.Scan("is");
            rp.Scan("Italian");
            System.Console.WriteLine(rp.Translate());*/

            /*AdvancedTranslator rp2 = new AdvancedTranslator("..\\..\\test\\files\\Phrasebook.pgf");
            rp2.setInputLanguage("PhrasebookEng");
            rp2.Scan("this");
            rp2.Scan("wine");
            rp2.RemoveToken();
            rp2.Scan("wine");
            rp2.Scan("is");
            rp2.Scan("Italian");
            rp2.Scan(".");
            rp2.setOutputLanguage("PhrasebookIta");
            System.Console.WriteLine(rp2.Translate());
            */
            System.Console.Out.WriteLine("done");
            System.Console.In.ReadLine();
        }
    }
}
