//-----------------------------------------------------------------------
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
    using CSPGF.Reader;

    /// <summary>
    /// Builds trees.
    /// </summary>
    internal class TreeBuilder
    {
        /// <summary>
        /// Initializes a new instance of the TreeBuilder class.
        /// </summary>
        public TreeBuilder() 
        { 
        }

        /// <summary>
        /// Builds trees.
        /// </summary>
        /// <param name="chart">The current chart.</param>
        /// <param name="startCat">The start category.</param>
        /// <param name="length">The length</param>
        /// <returns>A list of trees.</returns>
        public List<Tree> BuildTrees(Chart chart, CncCat startCat, int length)
        {
            List<Tree> temp = new List<Tree>();
            for (int catID = startCat.FirstFID; catID < startCat.LastFID + 1; catID++) 
            {
                int cat = chart.GetCategory(catID, 0, 0, length);
                if (cat != -1) 
                {
                    List<Tree> tmp = this.MkTreesForCat(cat, chart);
                    DisplayList(tmp);
                    temp.AddRange(tmp);
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
            foreach (ApplProduction p in chart.GetProductions(cat)) 
            {
                foreach (Tree t in this.MkTreesForProduction(p, chart)) 
                {
                    temp.Add(t);
                }
            }

            return temp;
        }

        /// <summary>
        /// Makes a tree with help of a production and a chart.
        /// </summary>
        /// <param name="p">An application production.</param>
        /// <param name="chart">The current chart.</param>
        /// <returns>A list of trees.</returns>
        public List<Tree> MkTreesForProduction(CSPGF.Reader.ApplProduction p, Chart chart)
        {
            List<Tree> temp = new List<Tree>();
            if (p.Domain().Length == 0)
            {
                temp.Add(new Application(p.Function.Name, new List<Tree>()));
                return temp;
            }
            else
            {
                List<List<Tree>> lsmx = new List<List<Tree>>();
                foreach (int pp in p.Domain())
                {
                    if (pp != p.FId)
                    {
                        lsmx.Add(this.MkTreesForCat(pp, chart));    // TODO fix since it can create endless trees.
                    }
                }           

                foreach (List<Tree> tree in this.ListMixer(lsmx)) 
                {
                    temp.Add(new Application(p.Function.Name, tree));
                }

                return temp;
            }
        }

        /// <summary>
        /// Mixes a list of lists.
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
                    List<Tree> nT = new List<Tree>();
                    nT.Add(lT);
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

        /// <summary>
        /// This is only for debug. Prints a list of lists of trees to the log.
        /// </summary>
        /// <param name="trees">The tree to display.</param>
        public string DisplayTree(Tree tree)
        {
            string tmp = string.Empty;
            if (tree is Application)
            {
                Application ap = (Application)tree;
                tmp = ap.Fun + "(";
                foreach (Tree t in ap.Args)
                {
                    tmp += DisplayTree(t)+","; 
                }
                tmp += ")";
            }
            else if (tree is Variable)
            {
                Variable v = (Variable)tree;
                tmp += v.Cid;
            }
            else if (tree is MetaVariable)
            {
                MetaVariable mv = (MetaVariable)tree;
                tmp += mv.ID;
            }
            else if (tree is Literal)
            {
                Literal lt = (Literal)tree;
                tmp += lt.Value;
            }
            else if (tree is Lambda)
            {
                Lambda lb = (Lambda)tree;
                tmp += "Lambda(" + DisplayTree(lb.Body) + ")";
            }

            return tmp + " ";
        }

        private void DisplayList(List<Tree> t)
        {
            foreach (Tree tree in t)
            {
                TempLog.LogMessageToFile("Tree:  " + this.DisplayTree(tree));
            }
        }
    }
}

// object TreeBuilder {
//  //val log = Logger.getLogger("org.grammaticalframework.parser.TreeBuilder")
//  def buildTrees( chart:Chart, startCat:CncCat, length:Int ):List[Tree] = {
//    //log.fine("Building trees with start category " + (0, startCat, 0, length))
//    (startCat.firstID until startCat.lastID + 1).flatMap( catID =>
//      chart.getCategory(catID, 0, 0, length) match {
//        case None => Nil
//        case Some(cat) => mkTreesForCat(cat, chart)
//      }).toList
//  }
//  def mkTreesForCat(cat : Int, chart:Chart):List[Tree] = {
//    //log.fine("Making trees for category "+ cat)
//    for {p <- chart.getProductions(cat).toList;
//         t <- mkTreesForProduction(p, chart)}
//    yield t
//  }
//  def mkTreesForProduction( p:Production, chart:Chart):List[Tree] = {
//      if (p.domain.length == 0)
//         List(new Application(p.function.name, Nil))
//      else
//         for (args <- listMixer( p.domain.toList.map(mkTreesForCat(_,chart)) ) )
//         yield new Application(p.function.name, args)
//  }

