﻿//-----------------------------------------------------------------------
// <copyright file="TreeBuilder.cs" company="None">
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

namespace CSPGF.Parse
{
    using System.Collections.Generic;
    using System.Linq;
    using Grammar;

    /// <summary>
    /// Builds trees.
    /// </summary>
    internal class TreeBuilder
    {
        /// <summary>
        /// Builds the trees.
        /// </summary>
        /// <param name="chart">The current chart.</param>
        /// <param name="startCat">The start category.</param>
        /// <returns>A list of trees.</returns>
        public List<Tree> BuildTrees(Chart chart, ConcreteCategory startCat)
        {
            List<Tree> temp = new List<Tree>();
            for (int catID = startCat.FirstFID; catID < startCat.LastFID + 1; catID++)
            {
                int? cat = chart.LookupPC(catID, 0, 0); // TODO what should last value be?
                // int? cat = chart.GetCategory(catID, 0, 0, chart.nextId); TODO length=nextId? nope
                if (cat.HasValue)
                {
                    temp.AddRange(this.MkTreesForCat(cat.Value, chart));
                }
            }

            return temp;
        }

        /// <summary>
        /// Makes a list of trees for a certain category with help of the current chart.
        /// </summary>
        /// <param name="cat">The category.</param>
        /// <param name="chart">The current chart.</param>
        /// <returns>A list of trees.</returns>
        public List<Tree> MkTreesForCat(int cat, Chart chart)
        {
            List<Tree> temp = new List<Tree>();
            foreach (Production p in chart.ExpandForest(cat))
            {
                temp.AddRange(this.MkTreesForProduction(p, chart));
            }

            return temp;
        }

        /// <summary>
        /// Makes a tree with help of a production and a chart.
        /// </summary>
        /// <param name="p">An application production.</param>
        /// <param name="chart">The current chart.</param>
        /// <returns>A list of trees.</returns>
        public List<Tree> MkTreesForProduction(Production p, Chart chart)
        {
            List<Tree> temp = new List<Tree>();

            if (p is ProductionApply)
            {
                var pa = (ProductionApply)p;
                if (pa.Domain().Length == 0)
                {
                    temp.Add(new Application(pa.Function.Name, new List<Tree>()));
                    return temp;
                }
                else
                {
                    List<List<Tree>> lsmx = new List<List<Tree>>();
                    foreach (int pp in pa.Domain())
                    {
                        if (pp != p.FId)
                        {
                            lsmx.Add(this.MkTreesForCat(pp, chart));    // TODO fix since it can create endless trees.
                        }
                    }

                    foreach (List<Tree> tree in this.ListMixer(lsmx))
                    {
                        temp.Add(new Application(pa.Function.Name, tree));
                    }
                }
            }
            else if (p is ProductionConst)
            {
                var pc = (ProductionConst)p;
                temp.Add(new Literal(pc.Tokens[0], pc.Type));
            }

            return temp;
        }

        /// <summary>
        /// Mixes a list of lists. Takes the first list and mixes it with the rest of the lists in the list.
        /// </summary>
        /// <param name="l">A list of lists of trees.</param>
        /// <returns>A new list of lists of trees.</returns>
        public List<List<Tree>> ListMixer(List<List<Tree>> l)
        {
            List<List<Tree>> newList = new List<List<Tree>>();
            if (l.Count == 0)
            {
                return newList;
            }
            else if (l.Count == 1)
            {
                foreach (Tree lT in l.First<List<Tree>>())
                {
                    List<Tree> nT = new List<Tree> { lT };
                    newList.Add(nT);
                }

                return newList;
            }
            else
            {
                List<Tree> head = l.First<List<Tree>>();
                l.Remove(head);
                foreach (Tree first in head)
                {
                    foreach (List<Tree> then in this.ListMixer(l))
                    {
                        then.Insert(0, first);
                        newList.Add(then);
                    }
                }

                return newList;
            }
        }
    }
}
