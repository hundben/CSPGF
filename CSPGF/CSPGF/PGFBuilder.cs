//-----------------------------------------------------------------------
// <copyright file="PGFBuilder.cs" company="None">
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

    /// <summary>
    /// Builds a PGF object with the help of PGF-reader.
    /// </summary>
    public class PGFBuilder 
    {
        /// <summary>
        /// Print debuginformation if true.
        /// </summary>
        private static bool debug = false;

        /// <summary>
        /// Reads a PGF binary from a file idenfied by filename.
        /// </summary>
        /// <param name="filename">Name of file</param>
        /// <returns>PGF object</returns>
        public static PGF FromFile(string filename) 
        {
            if (debug) 
            { 
                System.Console.WriteLine("Reading pgf from file : " + filename); 
            }

            BinaryReader stream = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            try 
            {
                return new PGFReader(stream).ReadPGF();
            } 
            catch (UnknownLanguageException e) 
            {
                throw new Exception(e.ToString());
            }
        }

        /// <summary>
        /// Reads a PGF binary from a file idenfied by filename.
        /// A list of the desired languages is given to the function so that the pgf
        /// doesn't have to be read entierely. The pgf file has to be indexed.
        /// </summary>
        /// <param name="filename">Name of file</param>
        /// <param name="languages">List of desired languages</param>
        /// <returns>PGF object</returns>
        public static PGF FromFile(string filename, List<string> languages) 
        {
            if (debug) 
            { 
                System.Console.WriteLine("Reading pgf from file : " + filename); 
            }

            BinaryReader stream = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            return new PGFReader(stream, languages).ReadPGF();
        }

        /// <summary>
        /// Reads a PGF binary from an inputstream.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <returns>PGF object</returns>
        public static PGF FromInputStream(BinaryReader stream) 
        {
            try 
            {
                return new PGFReader(stream).ReadPGF();
            } 
            catch (UnknownLanguageException e) 
            {
                throw new Exception(e.ToString());
            }
        }

        /// <summary>
        /// Reads a PGF binary from an inputstream.
        /// A list of the desired languages is given to the function so that the pgf
        /// doesn't have to be read entierely. The pgf file has to be indexed.
        /// </summary>
        /// <param name="stream">Stream to read from</param>
        /// <param name="languages">List of desired languages</param>
        /// <returns>PGF object</returns>
        public static PGF FromInputStream(BinaryReader stream, List<string> languages) 
        {
            return new PGFReader(stream, languages).ReadPGF();
        }
    }
}
