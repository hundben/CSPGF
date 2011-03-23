/*
Copyright (c) 2011, Christian Ståhlfors (christian.stahlfors@gmail.com), Erik Bergström (erktheorc@gmail.com)
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSPGF.reader;

namespace CSPGF.parser
{
    class ParseState
    {
        private CncCat startCat;
        private ParseTrie trie;
        private Chart chart;
        private Stack<ActiveItem> agenda;
        private int position;
        private Dictionary<int,ActiveSet> active;
        public ParseState(Concrete grammar)
        {
            startCat = grammar.GetStartCat();
            trie = new ParseTrie(null);
            chart = new Chart(100); //TODO 100 is a bad value... (even in c#)
            agenda = new Stack<ActiveItem>();
            position = 0;
            active = new Dictionary<int, ActiveSet>();

            //initiate
            foreach (Production k in grammar.GetProductions())
            {
                //TODO remove comment below
                chart.AddProduction(k);
            }
            for (int id = startCat.firstFID; id <= startCat.lastFID + 1; id++)
            {
                //TODO remove comment below
                // Får gå igenom och kolla om objektet är en ApplProduction, antar att foreach inte kör en is på alla objekt.
                foreach (Object obj in chart.GetProductions(id)) {
                    if (obj is ApplProduction) {
                        ApplProduction tmp = (ApplProduction)obj;
                        ActiveItem it = new ActiveItem(0, id, tmp.function, tmp.domain, 0, 0);
                        agenda.Push(it);
                    }
                }
                //Production[] prod = chart.GetProductions(id);
                //ActiveItem it = new ActiveItem(0, id, prod.function, prod.domain, 0, 0);
                //agenda.Push(it);
            }
            Compute();
        }
        private void Compute()
        {
            active[position] = new ActiveSet();
            //redo this with iterator or something like that?
            while (agenda.Count != 0)
            {
                ActiveItem e = agenda.Pop();
                ProcessActiveItem(e);

            }
        }
        private void ProcessActiveItem(ActiveItem item)
        {
            int j = item.begin;
            int A = item.category;
            CncFun f = item.function;
            int[] B = item.domain;
            int l = item.constituent;
            int p = item.position;

            Symbol sym = item.NextSymbol(); //is this correct?

            if (sym is ToksSymbol)
            {
                ToksSymbol tok = (ToksSymbol)sym;
                String[] tokens = tok.tokens;
                ActiveItem i = new ActiveItem(j, A, f, B, l, p + 1);
                //scan
                Stack<ActiveItem> newAgenda;
                Stack<ActiveItem> luAgenda = trie.Lookup(tokens);
                if (luAgenda.Count == 0)
                {
                    Stack<ActiveItem> a = new Stack<ActiveItem>();
                    trie.Add(tokens, a);
                    newAgenda = a;
                }
                else
                {
                    newAgenda = luAgenda;
                }
                newAgenda.Push(i);
            }
            else if (sym is ArgConstSymbol)
            {
                ArgConstSymbol arg = (ArgConstSymbol)sym;
                int d = arg.arg;
                int r = arg.cons;
                int bd = item.domain[d];
                if (active.ContainsKey(position))
                {
                    active[position].Add(bd, r, item, d);
                    foreach (Production prod in chart.GetProductions(bd))
                    {
                        if (prod is ApplProduction) 
                        {
                            ApplProduction prodAp = (ApplProduction)prod;
                            ActiveItem it = new ActiveItem(position, bd,prodAp.function, prod.Domain(), r, 0);
                            agenda.Push(it);
                        }
                    }
                    int cat = chart.GetCategory(bd, r, position, position);
                    //null here is wierd? :D
                    if (cat != -1)
                    {
                        int[] newDomain = (int[])B.Clone();  // WHAT TEH HELL??? clone returns an object :'(
                        newDomain[d] = cat;
                        ActiveItem it = new ActiveItem(j, A, f, newDomain, l, p + 1);
                        agenda.Push(it);
                    }
                }
            }
            else
            {
                int cat = chart.GetCategory(A, l, j, this.position);
                if (cat == -1)  //TODO check this -1 == null in this case???
                {
                    int N = chart.GenerateFreshCategory(A, l, j, position);
                    foreach (Tuple<ActiveItem, int> tmp in active[j].Get(A, l))
                    {
                        ActiveItem ip = tmp.Item1;
                        int d = tmp.Item2;
                        int[] domain = (int[])ip.domain.Clone();
                        domain[d] = N;
                        ActiveItem i = new ActiveItem(ip.begin, ip.category, ip.function, domain, ip.constituent, ip.position + 1);
                        agenda.Push(i);
                    }
                }
                else
                {
                    foreach (Tuple<ActiveItem,int,int> aset in active[position].Get(cat))
                    {
                        ActiveItem xprime = aset.Item1;
                        int dprime = aset.Item2;
                        int r = aset.Item3;
                        ActiveItem i = new ActiveItem(position, cat, f, B, r, 0);
                        agenda.Push(i);

                    }
                    chart.AddProduction(cat, f, B);
                }
            }
        }
        //TODO predict and so on


        public CSPGF.trees.Absyn.Tree[] GetTrees()
        {
            TreeBuilder tb = new TreeBuilder();
            TreeConverter tc = new TreeConverter();
            List<CSPGF.trees.Absyn.Tree> tmp = new List<CSPGF.trees.Absyn.Tree>();
            foreach (Tree t in tb.BuildTrees(chart, startCat, position)) {
                tmp.Add(tc.Intermediate2Abstract(t));
            }
            return tmp.ToArray<CSPGF.trees.Absyn.Tree>();
            
        }

        public Boolean Scan(String token)
        {
            ParseTrie tmp = trie.GetSubTrie(token);
            if (tmp != null) {
                Stack<ActiveItem> tmp2 = tmp.Lookup("");
                if( tmp2 != null)
                {
                    trie = tmp;
                    position++;
                    agenda = tmp2;
                    Compute();
                    return true;
                }
            }
            return false;
        }

        public String[] Predict()
        {
            return trie.Predict();
        }
        //  def predict():Array[String] = this.trie.predict
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


    }
}

//private class ParseState(val grammar:Concrete) {
    
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
//           }
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
//}