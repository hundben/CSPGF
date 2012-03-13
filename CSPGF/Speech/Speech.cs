//-----------------------------------------------------------------------
// <copyright file="Speech.cs" company="None">
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

namespace Speech
{
    using System;
    using System.Speech.Recognition;
    using System.Speech.Synthesis;

    /// <summary>
    /// Helperclass to use speech recognition and synthesis.
    /// </summary>
    public class Speech
    {
        /// <summary>
        /// Private SpeechRecognitionEngine
        /// </summary>
        private SpeechRecognitionEngine sre;

        /// <summary>
        /// Private SpeechSynthesizer
        /// </summary>
        private SpeechSynthesizer ss;

        /// <summary>
        /// Initializes a new instance of the Speech class.
        /// </summary>
        public Speech()
        {
            this.sre = new SpeechRecognitionEngine();
            this.sre.SetInputToDefaultAudioDevice();
            this.sre.LoadGrammar(new DictationGrammar());
            this.ss = new SpeechSynthesizer();
            this.ss.SetOutputToDefaultAudioDevice();
        }

        /// <summary>
        /// Initializes a new instance of the Speech class.
        /// </summary>
        /// <param name="grammar">Grammar to use with the speech recogniser</param>
        public Speech(string grammar)
        {
            this.sre = new SpeechRecognitionEngine();
            this.sre.SetInputToDefaultAudioDevice();
            this.sre.LoadGrammar(new Grammar(grammar));
            this.ss = new SpeechSynthesizer();
            this.ss.SetOutputToDefaultAudioDevice();
        }

        /// <summary>
        /// Listens for a set time and returns the recognised text.
        /// </summary>
        /// <param name="time">Time to listen</param>
        /// <returns>Recognised words</returns>
        public string Listen(TimeSpan time)
        {
            RecognitionResult result = this.sre.Recognize(time);
            string tmp = string.Empty;
            foreach (RecognizedWordUnit word in result.Words)
            {
                tmp += word.Text + " ";
            }

            return tmp;
        }

        /// <summary>
        /// Starts listening.
        /// </summary>
        public void ListenASync()
        {
            this.sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        /// <summary>
        /// Stops listening.
        /// </summary>
        public void ListenStop()
        {
            this.sre.RecognizeAsyncStop();
        }

        /// <summary>
        /// Returns the result from ListenASync.
        /// </summary>
        /// <returns>Recognised sentence</returns>
        public string ASyncResult()
        {
            return string.Empty;
        }

        /// <summary>
        /// Synthesises the sentence.
        /// </summary>
        /// <param name="sentence">Sentence to synthesise</param>
        public void Say(string sentence)
        {
            this.ss.Rate = 1;
            this.ss.Volume = 100;
            this.ss.Speak(sentence);
        }

        /// <summary>
        /// Synthesises the sentence with the given variables.
        /// </summary>
        /// <param name="sentence">Sentence to synthesise</param>
        /// <param name="rate">Rate of speech</param>
        /// <param name="volume">Volume of speech</param>
        public void Say(string sentence, int rate, int volume)
        {
            this.ss.Rate = rate;
            this.ss.Volume = volume;
            this.ss.Speak(sentence);
        }
    }
}
