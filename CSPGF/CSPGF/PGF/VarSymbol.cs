// -----------------------------------------------------------------------
// <copyright file="VarSymbol.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CSPGF.PGF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class VarSymbol : Symbol
    {
        int arg { get; private set; }     // Int argument index
        int var { get; private set; }     // Int variable number

        public VarSymbol(int arg, int var)
        {
            this.arg = arg;
            this.var = var;
        }

        public string ToString() 
        {
            return "";
        }
    }
}
