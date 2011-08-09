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

namespace CSPGF.Parse
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CSPGF.Reader;

    public class Chart
    {
        // private MultiMap<int, Object> productionSets = new MultiMap<int, Object>();
        private Dictionary<int, HashSet<Production>> productionSets = new Dictionary<int, HashSet<Production>>();
        // private Dictionary<Tuple<int, int, int, int>, int> categoryBookKeeper = new Dictionary<Tuple<int, int, int, int>, int>();
        private Dictionary<Category, int> categoryBookKeeper = new Dictionary<Category, int>();
        int nextCat;

        public Chart(int _nextCat)
        {
            this.nextCat = _nextCat;
        }

        public void AddProduction(Production p)
        {
            // TODO New version, check if it works
            HashSet<Production> prodSet;
            if (this.productionSets.TryGetValue(p.fId, out prodSet))
            {
                if (prodSet.Contains(p)) return;
                prodSet.Add(p);
            }
            else
            {
                prodSet = new HashSet<Production>();
                prodSet.Add(p);
                this.productionSets.Add(p.fId, prodSet);
            }
            this.nextCat = Math.Max(this.nextCat, p.fId + 1);
        }

        // Borde vara rätt... (kolla coersion? eriks anm. ;)
        public void AddProduction(int cat, CncFun fun, List<int> domain)
        {
            this.AddProduction(new ApplProduction(cat, fun, domain));
        }

        // TODO: Kolla denna oxå xD (lite skum, borde gå att ta bort massor? (eriks anm.)
        // Fel på den här... oväntat nog ;P Skrev om hoppas skiten blev rätt (eriks anm.)
        public List<ApplProduction> GetProductions(int resultCat)
        {
            HashSet<Production> prod;
            // Check if category exists, if not return empty productionset
            if (this.productionSets.TryGetValue(resultCat, out prod))
            {
                List<ApplProduction> applProd = new List<ApplProduction>();
                foreach (Production p in prod)
                {
                    foreach (ApplProduction ap in this.Uncoerce(p)) 
                    {
                        applProd.Add(ap);
                    }
                }
                return applProd;
            }
            else
            {
                return new List<ApplProduction>();
            }
        }

        // Should now work
        // Changes all Coerceproductions to ApplProductions
        private List<ApplProduction> Uncoerce(object p)
        {
            List<ApplProduction> prodList = new List<ApplProduction>();
            if (p is ApplProduction) {
                prodList.Add((ApplProduction)p);
            }
            else if (p is CoerceProduction) {
                CoerceProduction cp = (CoerceProduction)p;
                foreach (Production prod in this.GetProductions(cp.initId)) {
                    foreach (ApplProduction prod2 in this.Uncoerce(prod)) {
                        prodList.Add(prod2);
                    }
                }
            }
            return prodList;
        }

        public int getFreshCategory(int oldCat, int l, int j, int k)
        {
            // TODO Optimize this, use something else instead of looping through everything
            Category cf = new Category(oldCat, l, j, k);
            foreach (Category c in this.categoryBookKeeper.Keys)
            {
                if (cf.Equals(c))
                {
                    int i = this.categoryBookKeeper[c];
                    if (i != -1) return i;
                }
            }
            return this.GenerateFreshCategory(cf);
        }

        public int GetCategory(int oldCat, int cons, int begin, int end)
        {
            Category cf = new Category(oldCat, cons, begin, end);
            foreach (Category c in this.categoryBookKeeper.Keys)
            {
                if (c.Equals(cf)) return this.categoryBookKeeper[c];
            }

            // TODO check consistency of this
            return -1;
        }

        public int GenerateFreshCategory(Category c)
        {
            int cat = this.nextCat;
            this.nextCat++;
            // Category c = new Category(oldCat, l, j, k);
            this.categoryBookKeeper[c] = cat;    //TODO maybe add check here
            return cat;
        }

        public override string ToString()
        {
            string s = "=== Productions: ===\n";
            foreach (int i in this.productionSets.Keys) {
                s += this.productionSets[i].ToString() + '\n';
            }
            s += "=== passive items: ===\n";
            foreach (KeyValuePair<Category, int> ints in this.categoryBookKeeper) {
                // TODO add ToString on Category I guess? :D
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