//-----------------------------------------------------------------------
// <copyright file="PGFReader.cs" company="None">
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

namespace CSPGF.Grammar
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// Reads an PGF object
    /// </summary>
    internal class PGFReader : IDisposable
    {
        /// <summary>
        /// Main input stream to read from
        /// </summary>
        private readonly MemoryStream inputstream;

        /// <summary>
        /// Binary stream used by this class
        /// </summary>
        private readonly BinaryReader binreader;

        /// <summary>
        /// Desired languages
        /// </summary>
        private readonly List<string> languages;

        /// <summary>
        /// If disposed
        /// </summary>
        private bool disposed;

        /// <summary>
        /// PGF file version
        /// </summary>
        private int[] version;

        /// <summary>
        /// Initializes a new instance of the PGFReader class.
        /// </summary>
        /// <param name="filename">File to read from</param>
        public PGFReader(string filename)
        {
            this.inputstream = new MemoryStream(File.ReadAllBytes(filename));
            this.binreader = new BinaryReader(this.inputstream);
            this.version = new int[2];
        }

        /// <summary>
        /// Finalizes an instance of the PGFReader class.
        /// </summary>
        ~PGFReader()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Starts reading the PGF object
        /// </summary>
        /// <returns>PGF object</returns>
        public PGF ReadPGF()
        {
            Dictionary<string, int> index = null;

            // int[] ii = new int[2];
            for (int i = 0; i < 2; i++)
            {
                this.version[i] = this.BE16toLE16(this.binreader.ReadInt16());
            }

            if (this.version[0] == 1)
            {
                OldPGF pgf = new OldPGF(this.inputstream, this.binreader, this.languages);
                Dictionary<string, RLiteral> flags = pgf.GetFlags(index);
                Abstract abs = pgf.GetAbstract();
                this.CheckLangs();
                PGF pgfobj = new PGF(this.version[0], this.version[1], flags, abs, pgf.GetConcretes(abs.StartCat(), index));
                pgf.Dispose();
                return pgfobj;
            }
            else if (this.version[0] == 2 && this.version[1] == 1)
            {
                NewPGF pgf = new NewPGF(this.inputstream, this.binreader, this.languages);
                Dictionary<string, RLiteral> flags = pgf.GetFlags(index);
                Abstract abs = pgf.GetAbstract();
                this.CheckLangs();
                PGF pgfobj = new PGF(this.version[0], this.version[1], flags, abs, pgf.GetConcretes(abs.StartCat(), index));
                pgf.Dispose();
                return pgfobj;
            }
            else
            {
                throw new PGFException("This library does not support this version of the PGF file format.");
            }
        }

        /// <summary>
        /// Implements disposable interface
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Does the actual disposing
        /// </summary>
        /// <param name="disposing">If disposing should be done or not</param>
        public void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                try
                {
                    this.binreader.Close();
                    this.inputstream.Close();
                    this.binreader.Dispose();
                    this.inputstream.Dispose();
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.ToString());
                }
            }

            this.disposed = true;
        }

        /// <summary>
        /// Converts a short from Big Endian to Little Endian.
        /// </summary>
        /// <param name="val">The value to convert</param>
        /// <returns>Returns the value as Little Endian</returns>
        private int BE16toLE16(short val)
        {
            if (BitConverter.IsLittleEndian)
            {
                byte[] intAsBytes = BitConverter.GetBytes(val);
                Array.Reverse(intAsBytes);
                return BitConverter.ToInt16(intAsBytes, 0);
            }
            else
            {
                return val;
            }
        }

        /// <summary>
        /// Throws exception if a language does not exist in the PGF file.
        /// </summary>
        private void CheckLangs()
        {
            if (this.languages != null && this.languages.Count > 0)
            {
                foreach (string l in this.languages)
                {
                    throw new UnknownLanguageException(l);
                }
            }
        }
    }
}