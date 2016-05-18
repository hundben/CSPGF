//-----------------------------------------------------------------------
// <copyright file="Chart.cs" company="None">
//  Copyright (c) 2011-2016, Christian Ståhlfors (christian.stahlfors@gmail.com), 
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

namespace CSPGF.Parse
{
    using System.Collections.Generic;
    using CSPGF.Grammar;
    
    /// <summary>
    /// The chart, used to store data used by ParseState.
    /// </summary>
    internal class Chart
    {
        /// <summary>
        /// List of active items
        /// </summary>
        private Dictionary<int, Dictionary<int, List<ActiveItem>>> active;

        /// <summary>
        /// List of active items
        /// </summary>
        private List<Dictionary<int, Dictionary<int, List<ActiveItem>>>> actives;

        /// <summary>
        /// List of passive items
        /// </summary>
        private Dictionary<string, int> passive;

        /// <summary>
        /// Initializes a new instance of the Chart class.
        /// </summary>
        /// <param name="concrete">The concrete grammar we want to use.</param>
        public Chart(Concrete concrete)
        {
            this.active = new Dictionary<int, Dictionary<int, List<ActiveItem>>>();
            this.actives = new List<Dictionary<int, Dictionary<int, List<ActiveItem>>>>();
            this.passive = new Dictionary<string, int>();
            this.Forest = new Dictionary<int, List<Production>>();
            this.NextId = concrete.FId; // TODO fix so that it uses totalFID instead
            this.Offset = 0;

            // TODO check if we need to copy or not (copy now to be safe)
            foreach (KeyValuePair<int, List<Production>> entry in concrete.Productions)
            {
                var temp = new List<Production>();
                foreach (Production p in entry.Value)
                {
                    temp.Add(p);
                }

                this.Forest.Add(entry.Key, temp);
            }
        }

        /// <summary>
        /// Gets or sets the list of productions
        /// </summary>
        public Dictionary<int, List<Production>> Forest { get; set; }

        /// <summary>
        /// Gets or sets the next id to be used.
        /// </summary>
        public int NextId { get; set; }

        /// <summary>
        /// Gets or sets the current offset.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Looks up an ActiveItem.
        /// </summary>
        /// <param name="fid">The FId used.</param>
        /// <param name="label">The label used.</param>
        /// <returns>A list of ActiveItems</returns>
        public List<ActiveItem> LookupAC(int fid, int label)
        {
            if (this.active.ContainsKey(fid))
            {
                if (this.active[fid].ContainsKey(label))
                {
                    return this.active[fid][label];
                }
            }

            return null;
        }

        /// <summary>
        /// Looks up an ActiveItem and also use the offset.
        /// </summary>
        /// <param name="offset">The offset used.</param>
        /// <param name="fid">The FId used.</param>
        /// <param name="label">The label used.</param>
        /// <returns>A list of ActiveItems</returns>
        public List<ActiveItem> LookupACo(int offset, int fid, int label)
        {
            if (offset == this.Offset)
            {
                try
                {
                    return this.active[fid][label];
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                try
                {
                    return this.actives[offset][fid][label];
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Returns the labels for a certain FId.
        /// </summary>
        /// <param name="fid">The FId used.</param>
        /// <returns>A dictionary with all the ActiveItems.</returns>
        public Dictionary<int, List<ActiveItem>> LabelsAC(int fid)
        {
            return this.active[fid];
        }

        /// <summary>
        /// Insert an ActiveItem.
        /// </summary>
        /// <param name="fid">The corresponding FId.</param>
        /// <param name="label">The label.</param>
        /// <param name="items">The list of items to be inserted.</param>
        public void InsertAC(int fid, int label, List<ActiveItem> items)
        {
            if (this.active.ContainsKey(fid))
            {
                this.active[fid][label] = items;
            }
            else
            {
                var temp = new Dictionary<int, List<ActiveItem>>();
                temp[label] = items;
                this.active[fid] = temp;
            }         
        }

        /// <summary>
        /// Find a passive item.
        /// </summary>
        /// <param name="fid">The FId used.</param>
        /// <param name="label">The label used.</param>
        /// <param name="offset">The offset used.</param>
        /// <returns>The id of the passive item.</returns>
        public int? LookupPC(int fid, int label, int offset)
        {
            string key = fid + "." + label + "-" + offset;
            if (this.passive.ContainsKey(key))
            {
                return this.passive[key];
            }

            return null;
        }

        /// <summary>
        /// Insert a new passive item.
        /// </summary>
        /// <param name="fid1">The first FId used</param>
        /// <param name="label">The label used.</param>
        /// <param name="offset">The offset used.</param>
        /// <param name="fid2">The second FId used.</param>
        public void InsertPC(int fid1, int label, int offset, int fid2)
        {
            string key = fid1 + "." + label + "-" + offset;
            this.passive[key] = fid2;
        }

        /// <summary>
        /// Shift one step.
        /// </summary>
        public void Shift()
        {
            this.actives.Add(this.active);
            this.active = new Dictionary<int, Dictionary<int, List<ActiveItem>>>();
            this.passive = new Dictionary<string, int>();
            this.Offset++;
        }

        /// <summary>
        /// Expands the forest. Also takes care of ProductionCoerce
        /// </summary>
        /// <param name="fid">The FId used.</param>
        /// <returns>A list of productions.</returns>
        public List<Production> ExpandForest(int fid)
        {
            var rules = new List<Production>();

            foreach (Production p in this.Forest[fid])
            {
                if (p is ProductionApply || p is ProductionConst)
                {
                    rules.Add(p);
                }
                else if (p is ProductionCoerce) 
                {
                    ProductionCoerce pc = (ProductionCoerce)p;
                    var moreRules = this.ExpandForest(pc.InitId);
                    foreach (Production p2 in moreRules)
                    {
                        rules.Add((ProductionApply)p2);
                    }
                }
            }

            return rules;
        }
    }
}
