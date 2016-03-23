//-----------------------------------------------------------------------
// <copyright file="Concrete.cs" company="None">
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
    using System.Collections.Generic;

    /// <summary>
    /// Concrete grammar
    /// </summary>
    internal class Concrete
    {
        /// <summary>
        /// Starting category
        /// </summary>
        private readonly string startCat;

        /// <summary>
        /// Concrete grammar flags
        /// </summary>
        private Dictionary<string, Literal> flags;

        /// <summary>
        /// Initializes a new instance of the Concrete class.
        /// </summary>
        /// <param name="name">Name of concrete grammar</param>
        /// <param name="flags">Grammar flags</param>
        /// <param name="linDefs">Default linearization definitions</param>
        /// <param name="prods">List of production sets</param>
        /// <param name="cncCats">Dictionary containing concrete categories</param>
        /// <param name="fId">Function id</param>
        /// <param name="defaultStartCat">Default starting category</param>
        public Concrete(string name, Dictionary<string, Literal> flags, LinDef[] linDefs, ProductionSet[] prods, Dictionary<string, ConcreteCategory> cncCats, int fId, string defaultStartCat)
        {
            this.Name = name;
            this.flags = flags;
            this.Prods = prods;
            this.CncCats = cncCats;
            this.FId = fId;
            this.startCat = defaultStartCat;
            this.LinDefs = linDefs;

            // Populate helper dictionary for production set
            // TODO check if productionsets can have the same ID?
            this.Productions = new Dictionary<int, List<Production>>();

            foreach (ProductionSet ps in this.Prods)
            {
                var tempProd = new List<Production>();
                foreach (Production p in ps.Prods)
                {
                    tempProd.Add(p);
                }

                this.Productions[ps.ID] = tempProd;
            }
        }

        /// <summary>
        /// Gets the name of the concrete grammar
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets a list of production sets.
        /// </summary>
        public ProductionSet[] Prods { get; private set; }

        /// <summary>
        /// Gets the dictionary containing concrete categories
        /// </summary>
        public Dictionary<string, ConcreteCategory> CncCats { get; private set; }

        /// <summary>
        /// Gets the function id
        /// </summary>
        public int FId { get; private set; }

        /// <summary>
        /// Gets the totalFID
        /// </summary>
        public int TotalFID { get; private set; }

        /// <summary>
        /// Gets the default linearization categories
        /// </summary>
        public LinDef[] LinDefs { get; private set; }

        /// <summary>
        /// Gets a dictionary of productions
        /// </summary>
        public Dictionary<int, List<Production>> Productions { get; private set; }

        /// <summary>
        /// Gets a list of concrete categories
        /// </summary>
        /// <returns>List of concrete categories</returns>
        public List<ConcreteCategory> GetCncCats()
        {
            List<ConcreteCategory> tmp = new List<ConcreteCategory>();
            foreach (KeyValuePair<string, ConcreteCategory> c in this.CncCats) 
            {
                tmp.Add(c.Value);
            }

            return tmp;
        }

        /// <summary>
        /// Returns starting category if it exists, otherwise defaultStartCat.
        /// </summary>
        /// <returns>Starting category</returns>
        public ConcreteCategory GetStartCat()
        {
            if (this.CncCats.ContainsKey(this.startCat))
            {
                return this.CncCats[this.startCat];
            }
            
            return new ConcreteCategory(this.startCat, 0, 0, new string[0]);
        }

        /// <summary>
        /// Returns a list of all productions relating to this concrete grammar
        /// </summary>
        /// <returns>List of all productions</returns>
        public List<Production> GetProductions()
        {
            List<Production> tmp = new List<Production>();
            foreach (ProductionSet ps in this.Prods) 
            {
                tmp.AddRange(ps.Prods);
            }

            return tmp;
        }

        /// <summary>
        /// Returns name of the concrete grammar
        /// </summary>
        /// <returns>"Concrete" + grammar name</returns>
        public override string ToString()
        {
            return "Concrete" + this.Name;
        }

        /// <summary>
        /// Returns a Dictionary containing all productions with their set id as a key.
        /// </summary>
        /// <returns>Returns a Dictionary containing all productions</returns>
        public Dictionary<int, HashSet<Production>> GetSetOfProductions()
        {
            Dictionary<int, HashSet<Production>> dict = new Dictionary<int, HashSet<Production>>();
            foreach (ProductionSet p in this.Prods) 
            {
                dict.Add(p.ID, p.GetSetOfProductions());
            }

            return dict;
        }
    }
}
