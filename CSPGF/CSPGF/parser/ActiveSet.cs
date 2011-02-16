using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser
{
    class ActiveSet
    {
    }
}

//**
// * this is used to keed track of sets of active items (the S_k)
// * */
//private class ActiveSet {

//  //val log = Logger.getLogger("org.grammaticalframework.parser")

//  val store = new HashMap[Int, MultiMap[Int, (ActiveItem,Int)]]

//  def add(cat:Int, cons:Int, item:ActiveItem, cons2:Int):Boolean =
//    this.store.get(cat) match {
//      case None => {
//        val newMap = new HashMap[Int, Set[(ActiveItem,Int)]]
//                              with MultiMap[Int, (ActiveItem,Int)]
//        newMap.addBinding(cons,(item,cons2))
//        this.store.update(cat, newMap)
//        return true
//      }
//      case Some(map) =>
//        if (map.entryExists(cons, (item,cons2).equals))
//          return false
//        else {
//          map.addBinding(cons, (item,cons2))
//          return true
//        }
//    }

//  def get(cat:Int):Iterator[(ActiveItem, Int, Int)] =
//    this.store.get(cat) match {
//      case None => return Iterator.empty
//      case Some(amap) => {
//        for( k <- amap.keys.iterator ;
//             (item, d) <- amap(k).iterator)
//            yield (item, d, k)
//      }
//    }

//  def get(cat:Int, cons:Int):Seq[(ActiveItem,Int)] =
//    this.store.get(cat) match {
//      case None => return Nil
//      case Some(map) => map.get(cons) match {
//        case None => return Nil
//        case Some(s) => return s.toSeq
//      }
//    }
//}