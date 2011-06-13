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
    class Chart
    {
        //private MultiMap<int, Object> productionSets = new MultiMap<int, Object>();
        private Dictionary<int, HashSet<Production>> productionSets = new Dictionary<int,HashSet<Production>>();
        private Dictionary<Tuple<int, int, int, int>, int> categoryBookKeeper = new Dictionary<Tuple<int, int, int, int>, int>();
        int nextCat;

        public Chart(int _nextCat)
        {
            nextCat = _nextCat;
        }

        public void AddProduction(Production p)
        {
            HashSet<Production> temp;

            if (productionSets.TryGetValue(p.fId, out temp))
            {
                temp.Add(p);
            }
                nextCat = Math.Max(nextCat, p.fId + 1); //TODO this is very wrong...
        }

        // Borde vara rätt... (kolla coersion? eriks anm. ;)
        public void AddProduction(int cat, CncFun fun, List<int> domain)
        {
            AddProduction(new ApplProduction(cat, fun, domain));
        }

        //TODO: Kolla denna oxå xD
        public List<Production> GetProductions(int resultCat)
        {
            HashSet<Production> tmp = productionSets[resultCat];
            List<Production> tmp2 = new List<Production>();
            if (tmp.Count != 0) {
                foreach (Object p in tmp) {
                    if (p is Production) {
                        foreach (Production prod in Uncoerce(p)) {
                            tmp2.Add(prod);
                        }
                    }
                    else {
                        throw new Exception(p.ToString() + " is not a Production, and should be one");
                    }
                }
                return tmp2;
            }
            else {
                return new List<Production>();
            }
        }

        //TODO: Denna kan vara helt åt helvete :D 
        private List<Production> Uncoerce(Object p)
        {
            List<Production> tmp = new List<Production>();
            if (p is Production) {
                tmp.Add((Production)p);
            }
            else if (p is CoerceProduction) {
                CoerceProduction tmp2 = (CoerceProduction)p;
                foreach (Production prod in GetProductions(tmp2.initId)) {
                    foreach (Production prod2 in Uncoerce(prod)) {
                        tmp.Add(prod2);
                    }
                }
            }
            return tmp;
        }

        public int getFreshCategory(int oldCat, int l, int j, int k)
        {
            //Does this line actually work? since it's the reference we need to check...
            //TODO rewrite this... or maybe just wait and rewrite the whole parser...
            int i = categoryBookKeeper[new Tuple<int, int, int, int>(oldCat, l, j, k)];
            if (i != -1) {
                return i;
            }
            else {
                return GenerateFreshCategory(oldCat, l, j, k);
            }
        }

        public int GetCategory(int oldCat, int cons, int begin, int end)
        {
            Tuple<int, int, int, int> temp = new Tuple<int, int, int, int>(oldCat, cons, begin, end);
            //TODO: Check if it returns null if none is found
            foreach (KeyValuePair<Tuple<int, int, int, int>, int> tmp in categoryBookKeeper) {
                if (tmp.Key.Item1 == oldCat && tmp.Key.Item2 == cons && tmp.Key.Item3 == begin && tmp.Key.Item4 == end) {
                    return tmp.Value;
                }
            }
            // Change everywhere so that this is correct.
            return -1;
        }

        public int GenerateFreshCategory(int oldCat, int l, int j, int k)
        {
            int cat = nextCat;
            nextCat++;
            categoryBookKeeper[new Tuple<int, int, int, int>(oldCat, l, j, k)] = cat;
            return cat;
        }

        public override String ToString()
        {
            String s = "=== Productions: ===\n";
            foreach (int i in productionSets.Keys) {
                s += productionSets[i].ToString() + '\n';
            }
            s += "=== passive items: ===\n";
            foreach (KeyValuePair<Tuple<int, int, int, int>, int> ints in categoryBookKeeper) {
                s += ints.Key.ToString() + " -> " + ints.Value + '\n';
            }
            return s;
        }
    }
}

//private class Chart(var nextCat:Int) {

//  /** **********************************************************************
//   * Handling Productions
//   * */
//  private val productionSets : MultiMap[Int,AnyProduction] =
//    new HashMap[Int, Set[AnyProduction]] with MultiMap[Int,AnyProduction]

//  def addProduction(p:AnyProduction):Boolean = {
//    if (productionSets.entryExists(p.getCategory(), p.==))
//      return false
//    else {
//      //log.finest("Adding production " + p + " in chart.")
//      productionSets.addBinding(p.getCategory(), p)
//      this.nextCat = this.nextCat.max(p.getCategory() + 1)
//      return true
//    }
//  }

//  def addProduction(cat:Int, fun: CncFun, domain:Array[Int]):Boolean =
//    this.addProduction(new Production(cat, fun, domain))


//  def getProductions(resultCat : Int):Array[Production] =
//    productionSets.get(resultCat) match {
//      case Some(ps) =>
//        for ((anyP:AnyProduction) <- ps.toArray;
//             prod <- this.uncoerce(anyP) )
//        yield prod
//      case None => new Array[Production](0)
//    }

//  private def uncoerce(p : AnyProduction):Array[Production] = p match {
//    case (p:Production) => Array(p)
//    case (c:Coercion) => for (prod <- this.getProductions(c.getInitId()) ;
//                              a <- this.uncoerce(prod) )
//                          yield a
//  }


//  /** **********************************************************************
//   *  Handling fresh categories
//   * */
//  private val categoryBookKeeper: HashMap[(Int, Int, Int, Int), Int]
//  = new HashMap[(Int, Int, Int, Int), Int]()

//  def getFreshCategory(oldCat:Int, l:Int, j:Int, k:Int):Int =
//    categoryBookKeeper.get((oldCat, l, j, k)) match {
//      case None => this.generateFreshCategory(oldCat, l, j, k)
//      case Some(c) => c
//    }
//  def getCategory(oldCat:Int, cons:Int, begin:Int, end:Int):Option[Int] =
//    categoryBookKeeper.get((oldCat, cons, begin, end))

//  def generateFreshCategory(oldCat:Int, l:Int, j:Int, k:Int):Int = {
//    val cat = this.nextCat
//    this.nextCat += 1
//    categoryBookKeeper.update((oldCat, l, j, k), cat)
//    return cat
//  }

//  override def toString() = {
//    var s = "=== Productions: ===\n"
//    for(cat <- this.productionSets.keys ;
//        prod <- this.productionSets(cat))
//      s += prod.toString + "\n"
//    s += "=== passive items: ===\n"
//    for(fk <- this.categoryBookKeeper.keys)
//      s += fk + " -> " + this.categoryBookKeeper(fk) + "\n"
//    s
//  }
//}