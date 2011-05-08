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
using CSPGF.reader;

namespace CSPGF {
    class PGFBuilder {
        private static Boolean DBG = false;

        /* ************************************************* */
        /* Public reading functions                          */
        /* ************************************************* */
        /**
         * Reads a PGF binary from a file idenfied by filename.
         *
         * @param filename the path of the pgf file.
         */
        
        // No reason to pass an inputstream to the constructor? Could just as easily just use the filename?
        public static PGF FromFile(String filename) {
            if (DBG) { System.Console.WriteLine("Reading pgf from file : " + filename); }
                // TODO: Check!
            BinaryReader stream = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
	        try {
	            return new PGFReader(stream).ReadPGF();
	        } catch (UnknownLanguageException e) {
	            throw new Exception(e.ToString());
	        }
        }

        /**
         * Reads a PGF binary from a file idenfied by filename.
         * A list of the desired languages is given to the function so that the pgf
         * doesn't have to be read entierely. The pgf file have to be indexed
         *
         *
         * @param filename the path of the pgf file.
         * @param languages the list of desired languages
         */
        public static PGF FromFile(String filename, List<String> languages) {
	        if (DBG) { System.Console.WriteLine("Reading pgf from file : " + filename); }
            BinaryReader stream = new BinaryReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            return new PGFReader(stream, languages).ReadPGF();
        }

        /**
         * Reads a pgf from an input stream
         *
         * @param inStream and InputStream to read the pgf binary from.
         */
        //TODO: Check inputstream -> streamreader
        public static PGF FromInputStream(BinaryReader stream) {
	        try {
	            return new PGFReader(stream).ReadPGF();
	        } catch (UnknownLanguageException e) {
                throw new Exception(e.ToString());
	        }
        }

        /**
         * Reads a pgf from an input stream
         * A list of the desired languages is given to the function so that the pgf
         * doesn't have to be read entierely. The pgf file have to be indexed
         *
         * @param inStream and InputStream to read the pgf binary from.
         */
        //TODO: Check inputstream -> streamreader
        public static PGF FromInputStream(BinaryReader stream, List<String> languages) {
            return new PGFReader(stream, languages).ReadPGF();
        }


    }
}
