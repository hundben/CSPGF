using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF
{
    class UnknownLanguageException : Exception
    {
        //public String language { get; private set; }
        public UnknownLanguageException(String language) : base(language)
        {
            //this.language = language;
        }
        public UnknownLanguageException()
        {
        }

        public override String ToString()
        {
            return "Unknown language: " + base.ToString();
        }
    }
}
