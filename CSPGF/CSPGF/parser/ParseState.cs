//-----------------------------------------------------------------------
// <copyright file="ParseState.cs" company="None">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CSPGF.Reader;

    /// <summary>
    /// The parsestate class.
    /// </summary>
    [Serializable]
    public class ParseState
    {
        /// <summary>
        /// Start category
        /// </summary>
        private CncCat startCat;

        /// <summary>
        /// The parse tree.
        /// </summary>
        private ParseTrie trie;

        /// <summary>
        /// The chart.
        /// </summary>
        private Chart chart;

        /// <summary>
        /// A stack of active items.
        /// </summary>
        private Stack<ActiveItem> agenda;

        /// <summary>
        /// Current position.
        /// </summary>
        private int position;

        /// <summary>
        /// Dictionary of ActiveSets.
        /// </summary>
        private Dictionary<int, ActiveSet> active;

        /// <summary>
        /// Initializes a new instance of the ParseState class.
        /// </summary>
        /// <param name="grammar">The concrete grammar we would like to use.</param>
        public ParseState(Concrete grammar)
        {
            this.startCat = grammar.GetStartCat();
            this.trie = new ParseTrie(null);

            // TODO check if we should use all languages or just the "active" one
            int lastCat = 0;
            foreach (CncCat cncTemp in grammar.GetCncCats())
            {
                // TODO check if first or last id
                lastCat = Math.Max(cncTemp.LastFID, lastCat);
            }

            this.chart = new Chart(lastCat++); // was 100
            this.agenda = new Stack<ActiveItem>();
            this.position = 0;
            this.active = new Dictionary<int, ActiveSet>();

            // initiate
            foreach (Production k in grammar.GetProductions()) 
            {
                this.chart.AddProduction(k);
            }

            for (int id = this.startCat.FirstFID; id <= this.startCat.LastFID + 1; id++) 
            {
                foreach (ApplProduction prod in this.chart.GetProductions(id)) 
                {
                    ActiveItem it = new ActiveItem(0, id, prod.Function, prod.Domain(), 0, 0);
                    this.agenda.Push(it);
                }
            }

            this.Compute();
        }

        /// <summary>
        /// Get the trees.
        /// </summary>
        /// <returns>A list of the trees.</returns>
        public List<CSPGF.Trees.Absyn.Tree> GetTrees()
        {
            TreeBuilder tb = new TreeBuilder();
            TreeConverter tc = new TreeConverter();
            List<CSPGF.Trees.Absyn.Tree> tmp = new List<CSPGF.Trees.Absyn.Tree>();
            foreach (Tree t in tb.BuildTrees(this.chart, this.startCat, this.position)) 
            {
                tmp.Add(tc.Intermediate2Abstract(t));
            }

            return tmp;
        }

        /// <summary>
        /// Scans a new token.
        /// </summary>
        /// <param name="token">The token to scan.</param>
        /// <returns>Returns true if scan i complete.</returns>
        public bool Scan(string token)
        {
            ParseTrie newTrie = this.trie.GetSubTrie(token);
            if (newTrie != null) 
            {
                string[] empt = new string[0];
                Stack<ActiveItem> newAgenda = newTrie.Lookup(empt);
                if (newAgenda != null) 
                {
                    this.trie = newTrie;
                    this.position++;
                    this.agenda = newAgenda;
                    this.Compute();
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Predicts the next token.
        /// </summary>
        /// <returns>A list of valid tokens.</returns>
        public List<string> Predict()
        {
            return this.trie.Predict();
        }

        /// <summary>
        /// Computes the new trees.
        /// </summary>
        private void Compute()
        {
            this.active[this.position] = new ActiveSet();

            // redo this with iterator or something like that?
            while (this.agenda.Count != 0) 
            {
                ActiveItem e = this.agenda.Pop();
                this.ProcessActiveItem(e);
            }
        }

        /// <summary>
        /// Processes an active item.
        /// </summary>
        /// <param name="item">The item to be processed.</param>
        private void ProcessActiveItem(ActiveItem item)
        {
            int j = item.Begin;
            int a = item.Category;
            CncFun f = item.Function;
            List<int> b = item.Domain;
            int l = item.Constituent;
            int p = item.Position;

            Symbol sym = item.NextSymbol(); // is this correct?

            if (sym is ToksSymbol) 
            {
                ToksSymbol tok = (ToksSymbol)sym;
                List<string> tokens = tok.Tokens;
                ActiveItem i = new ActiveItem(j, a, f, b, l, p + 1);

                // scan
                Stack<ActiveItem> newAgenda;
                Stack<ActiveItem> tempAgenda = this.trie.Lookup(tokens);
                if (tempAgenda == null || tempAgenda.Count == 0) 
                {
                    Stack<ActiveItem> sai = new Stack<ActiveItem>();
                    this.trie.Add(tokens, sai);
                    newAgenda = sai;
                }
                else 
                {
                    newAgenda = tempAgenda;
                }

                newAgenda.Push(i);
            }
            else if (sym is ArgConstSymbol) 
            {
                ArgConstSymbol arg = (ArgConstSymbol)sym;
                int d = arg.Arg;
                int r = arg.Cons;
                int bd = item.Domain[d];
                if (this.active.ContainsKey(this.position)) 
                {
                    // a bit strange, check if we should create an active set first...
                    if (this.active[this.position].Add(bd, r, item, d)) 
                    {
                        foreach (ApplProduction prod in this.chart.GetProductions(bd)) 
                        {
                            ActiveItem it = new ActiveItem(this.position, bd, prod.Function, prod.Domain(), r, 0);
                            this.agenda.Push(it);
                        }
                    }

                    int cat = this.chart.GetCategory(bd, r, this.position, this.position);
                    
                    // null here is wierd? :D
                    if (cat != -1) 
                    {
                        List<int> newDomain = new List<int>(b);
                        newDomain[d] = cat;
                        ActiveItem it = new ActiveItem(j, a, f, newDomain, l, p + 1);
                        this.agenda.Push(it);
                    }
                }
            }
            else 
            {
                int cat = this.chart.GetCategory(a, l, j, this.position);
                if (cat == -1) 
                {
                    int n = this.chart.GenerateFreshCategory(new Category(a, l, j, this.position));
                    foreach (ActiveItemInt aii in this.active[j].Get(a, l)) 
                    {
                        ActiveItem ip = aii.Item;
                        int d = aii.Cons;
                        List<int> domain = new List<int>(ip.Domain);
                        domain[d] = n;
                        ActiveItem i = new ActiveItem(ip.Begin, ip.Category, ip.Function, domain, ip.Constituent, ip.Position + 1);
                        this.agenda.Push(i);
                    }

                    this.chart.AddProduction(n, f, b);
                }
                else 
                {
                    HashSet<ActiveItemInt> items = this.active[this.position].Get(cat);
                    foreach (ActiveItemInt aii in items) 
                    {
                        // ActiveItem xprime = aii.item;
                        // int dprime = aii.cons;
                        int r = aii.Cons2;
                        ActiveItem i = new ActiveItem(this.position, cat, f, b, r, 0);
                        this.agenda.Push(i);
                    }

                    this.chart.AddProduction(cat, f, b);
                }
            }
        }
    }
}

// private class ParseState(val grammar:Concrete) {
//    private val startCat = this.grammar.startCat
//    private var trie = new ParseTrie
//    private val chart = new Chart(100) // TODO: 100 is a bad value...
//    private var agenda = new Stack[ActiveItem]
//    private var position = 0
//    // 'active' is the set of all the S_k's, holding the active items which are not
//    // on the agenda.
//    private var active = new HashMap[Integer, ActiveSet]
//    /* ********************************************************************* *
//     *                           INITIALIZATION                              */
//    // Adding all grammar productions in the chart
//    this.grammar.productions.foreach( p => this.chart.addProduction(p) )
//    // We create an initial agenda of all prodution with the
//    // start category as left-hand-side.
//    for ( id <- startCat.firstID until (startCat.lastID + 1) ;
//          prod <- chart.getProductions(id) )
//    {
//        val it = new ActiveItem(0, id, prod.function, prod.domain, 0, 0)
//        agenda.push(it)
//    }
//    // Finally we process this agenda
//    compute()
//    /*                        END OF INITIALIZATION                          *
//     * ********************************************************************* */
//     /* ********************************************************************* *
//      *                        PROCESSING AGENDA                              */
//     private def compute() = {
//         // We have to create a new set S_k where k is the current position
//         this.active = this.active + ((this.position, new ActiveSet))
//         // Then we can process the agenda
//         while (!agenda.isEmpty) {
//             //System.err.println("Agenda.size=" + agenda.size);
//             val e:ActiveItem = agenda.pop();
//             processActiveItem(e)
//         }
//     }
//     private def processActiveItem(item:ActiveItem) = {
//         val j = item.begin
//         val A = item.category
//         val f = item.function
//         val B = item.domain
//         val l = item.constituent
//         val p = item.position
//         //System.out.println("Processing active item " + item + " from the agenda")
//         item.nextSymbol match {
//             // ------------------------- before s∈T -------------------------
//             case Some(tok:ToksSymbol) => {
//                 //log.fine("Case before s∈T")
//                 val tokens = tok.tokens
//                 val i = new ActiveItem(j, A, f, B, l, p + 1)
//                 // SCAN
//                 // this.trie.add(tokens, i)
//                 val newAgenda = this.trie.lookup(tokens) match {
//                     case None => {
//                        val a = new Stack[ActiveItem]
//                        this.trie.add(tokens,a)
//                           a
//                    }
//                    case Some(a) => a
//                }
//                //log.fine("Adding item " + i + " for terminals "
//                //           + tokens.map(_.toString) + "(from item : " + item + ")")
//                newAgenda.push(i)
//            }
//         // ------------------------- before <d,r> -----------------------
//         case Some(arg:ArgConstSymbol) => {
//           //log.finest("Case before <d,r>")
//           val d = arg.arg
//           val r = arg.cons
//           val Bd = item.domain(d)
//           if (this.active(this.position).add(Bd,r,item,d)) {
//             for (prod <- chart.getProductions(Bd)) {
//               val it = new ActiveItem(this.position, Bd, prod.function,
//                                       prod.domain, r, 0)
//               agenda.push(it)
//             }
//           }****
//           chart.getCategory(Bd,r, this.position, this.position) match {
//             case None => {}
//             case Some(catN) => {
//               val newDomain = B.clone()
//               newDomain(d) = catN
//               val it = new ActiveItem(j, A, f, newDomain, l, p + 1)
//               //log.finest("Adding " + it + " to the agenda")
//               agenda.push(it)
//             }
//           }
//         }
//         // ------------------------- at the end --------------------------
//         case None => {
//           //log.finest("Case at the end")
//           chart.getCategory(A, l, j, this.position) match {
//             case None => {
//               val N = chart.generateFreshCategory(A, l, j, this.position)
//               for( (ip,d) <- this.active(j).get(A,l)) {
//                 //log.finest("combine with " + ip + " (" + d + ")")
//                 val domain = ip.domain.clone()
//                 domain(d) = N
//                 val i = new ActiveItem(ip.begin, ip.category, ip.function,
//                                        domain, ip.constituent, ip.position + 1)
//                 //log.finest("Adding " + i + " to the agenda")
//                 agenda.push(i)
//               }
//               chart.addProduction(N, f, B)
//             }
//             case Some(n) => {
//               for((xprime, dprime, r) <- this.active(this.position).get(n)) {
//                 val i = new ActiveItem(this.position, n, f, B, r, 0)
//                 //log.finest("Adding "+ i + " to the agenda")
//                 agenda.push(i)
//               }
//               chart.addProduction(n, f, B)
//             }
//           }
//         }
//       }
//     }
//     /*                                                                       *
//      * ********************************************************************* */
//  /**
//   * returns the set of possible next words
//   * */
//  def predict():Array[String] = this.trie.predict
//  def getTrees():Array[AbsSynTree] = {
//    val chart = this.chart
//    val startCat = this.startCat
//    val length = this.position
//    val parseTrees = TreeBuilder.buildTrees(chart, startCat, length)
//    return parseTrees.map(TreeConverter.intermediate2abstract).toArray
//  }
//  def scan(token:String):Boolean = this.trie.getSubTrie(token) match {
//    case None => return false
//    case Some(newTrie) => {
//      newTrie.lookup(Nil) match {
//        case None => return false
//        case Some(agenda) => {
//          this.trie = newTrie
//          this.position += 1
//          this.agenda = agenda
//          this.compute()
//        }
//      }
//    }
//    //log.finer(this.trie.toString)
//    return true
//  }
//  /* Overrides */ 
//  override def toString() =
//    "= ParseState =\n" +
//    "== Chart ==\n" +
//    this.chart.toString() +
//    "== Trie ==\n" +
//    this.trie.toString()
// }