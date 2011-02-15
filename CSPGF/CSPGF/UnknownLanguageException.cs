using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF
{
    class UnknownLanguageException : Exception
    {
        private String language;
        public UnknownLanguageException(String language)
        {
            this.language = language;
        }

        public String getLanguage()
        {
            return this.language;
        }

        public String toString()
        {
            return "Unknown language: " + language;
        }
    }
}
