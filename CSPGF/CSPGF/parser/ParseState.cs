using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser
{
    class ParseState
    {
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