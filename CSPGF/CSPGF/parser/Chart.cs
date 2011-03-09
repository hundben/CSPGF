using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSPGF.util;
using CSPGF.reader;

namespace CSPGF.parser
{
    class Chart
    {
        private MultiMap<int, AnyProduction> productionSets = new MultiMap<int, AnyProduction>();
        private Dictionary<Tuple<int, int, int, int>, int> categoryBookKeeper = new Dictionary<Tuple<int, int, int, int>, int>();
        int nextCat;

        public Chart(int _nextCat)
        {
            nextCat = _nextCat;
        }
        
        public Boolean AddProduction(AnyProduction p) {
            if(productionSets.ContainsKey(p.getCategory()/*, p.==*/)) {
                return false;
            } else {
            //      productionSets.addBinding(p.getCategory(), p)
            //      this.nextCat = this.nextCat.max(p.getCategory() + 1)
                return true;
            }
        }

        public Boolean AddProduction(int cat, CncFun fun, int[] domain)
        {
            return addProduction(new Production(cat, fun, domain));    
        }
        //  def addProduction(cat:Int, fun: CncFun, domain:Array[Int]):Boolean =
        //    this.addProduction(new Production(cat, fun, domain))

        public Production[] GetProductions(int resultCat)
        {
            HashSet<AnyProduction> test = productionSets.Get(resultCat);
            List<AnyProduction> test2 = new List<AnyProduction>();
            if (test != null)
            {
                foreach (AnyProduction p in test)
                {
                    test2.Add(p);
                }
                return test2.ToArray();
            }
            else
            {
                return null;
            }
        }

        //private Production[] uncoerce(AnyProduction p)
        //{
        //    if (p is Production)
        //    {
        //        p.ToArray<Production>();
        //    }
        //    else if (p is Coercion)
        //    {
        //        foreach (Production prod in p.prods
        //                    //    case (c:Coercion) => for (prod <- this.getProductions(c.getInitId()) ;
        //                    // a <- this.uncoerce(prod) )
        //                    // yield a
        //    }
        //}

        public int getFreshCategory(int oldCat, int l, int j, int k)
        {
            int i = categoryBookKeeper[new Tuple<int, int, int, int>(oldCat, l, j, k)];
            if (i != null)
            {
                return i;
            }
            else
            {
                return generateFreshCategory(oldCat, l, j, k);
            }
        }

        public Int32 getCategory(int oldCat, int cons, int begin, int end)
        {
            Tuple<int, int, int, int> temp = new Tuple<int, int, int, int>(oldCat, cons, begin, end);
            //TODO: Check if it returns null if none is found
            foreach (KeyValuePair<Tuple<int, int, int, int>, int> tmp in categoryBookKeeper)
            {
                if (tmp.Key.Item1 == oldCat && tmp.Key.Item2 == cons && tmp.Key.Item3 == begin && tmp.Key.Item4 == end)
                {
                    return tmp.Value;
                }
            }
            // Change everywhere so that this is correct.
            return -1;
        }

        public int generateFreshCategory(int oldCat, int l, int j, int k)
        {
            int cat = nextCat;
            nextCat++;
            categoryBookKeeper[new Tuple<int, int, int, int>(oldCat, l, j, k)] = cat;
            return cat;
        }

        public String ToString()
        {
            String s = "=== Productions: ===\n";
            foreach (int i in productionSets.KeySet())
            {
                s += productionSets.Get(i).ToString()+'\n'; 
            }
            s += "=== passive items: ===\n";
            foreach (KeyValuePair<Tuple<int, int, int, int>,int> ints in categoryBookKeeper)
            {
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