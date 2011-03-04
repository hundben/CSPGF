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
	        if (DBG) System.Console.WriteLine("Reading pgf from file : " + filename);
                // TODO: Check!
                StreamReader stream = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
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
        public static PGF FromFile(String filename, String[] languages) {
	        if (DBG) { System.Console.WriteLine("Reading pgf from file : " + filename); }
            StreamReader stream = new StreamReader(new FileStream(filename, FileMode.Open, FileAccess.Read));
            return new PGFReader(stream, languages).ReadPGF();
        }

        /**
         * Reads a pgf from an input stream
         *
         * @param inStream and InputStream to read the pgf binary from.
         */
        //TODO: Check inputstream -> streamreader
        public static PGF FromInputStream(StreamReader stream) {
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
        public static PGF FromInputStream(StreamReader stream, String[] languages) {
            return new PGFReader(stream, languages).ReadPGF();
        }


    }
}
