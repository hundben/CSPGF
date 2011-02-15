using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public static PGF fromFile(String filename) {
	        if (DBG) System.Console.WriteLine("Reading pgf from file : " + filename);
                InputStream stream = new FileInputStream(filename);
	        try {
	            return new PGFReader(stream).readPGF();
	        } catch (UnknownLanguageException e) {
	            throw new Exception();
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
        public static PGF fromFile(String filename, String[] languages) {
	        if (DBG) { System.Console.WriteLine("Reading pgf from file : " + filename); }
            InputStream stream = new FileInputStream(filename);
            return new PGFReader(stream, languages).readPGF();
        }

        /**
         * Reads a pgf from an input stream
         *
         * @param inStream and InputStream to read the pgf binary from.
         */
        public static PGF fromInputStream(InputStream stream) {
	        try {
	            return new PGFReader(stream).readPGF();
	        } catch (UnknownLanguageException e) {
                throw new Exception();
	        }
        }

        /**
         * Reads a pgf from an input stream
         * A list of the desired languages is given to the function so that the pgf
         * doesn't have to be read entierely. The pgf file have to be indexed
         *
         * @param inStream and InputStream to read the pgf binary from.
         */
        public static PGF fromInputStream(InputStream stream, String[] languages) {
            return new PGFReader(stream, languages).readPGF();
        }


    }
}
