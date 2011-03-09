using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser
{
    class Chart
    {
        public Chart(int nextCat)
        {

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