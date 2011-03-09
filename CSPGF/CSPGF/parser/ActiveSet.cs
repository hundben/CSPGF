using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//Check this class, it seems retarded...

namespace CSPGF.parser
{
    class ActiveSet
    {
        Dictionary<int,Dictionary<int,Tuple<ActiveItem,int>>> store;
        public ActiveSet()
        {
            store = new Dictionary<int,Dictionary<int, Tuple<ActiveItem, int>>>();
        }
        //använd logg :P
        public bool Add(int cat, int cons, ActiveItem item, int cons2)
        {
            //Hämta värde ur hashmap
            if (store.ContainsKey(cat))
            {
                Dictionary<int, Tuple<ActiveItem, int>> map = store[cat];
                if (map.ContainsKey(cons))
                {
                    if (map[cons].Item1 == item && map[cons].Item2 == cons2)
                        return false;
                }
                map.Add(cons, new Tuple<ActiveItem, int>(item,cons2));
                return true;
            }
            else
            {
                Tuple<ActiveItem,int> set = new Tuple<ActiveItem,int>(item,cons2);
                Dictionary<int, Tuple<ActiveItem,int>> newMap = new Dictionary<int,Tuple<ActiveItem,int>>();
                newMap.Add(cons,set);
                store[cat] = newMap;    //TODO check if this is correct, might need to check if the key exists
                return true;
            }
        }
        public List<Tuple<ActiveItem, int, int>> Get(int cat)
        {
            if (store.ContainsKey(cat))
            {
                Dictionary<int, Tuple<ActiveItem, int>> amap = store[cat];
                List<Tuple<ActiveItem, int, int>> tp = new List<Tuple<ActiveItem, int, int>>();
                foreach (int key in amap.Keys)
                {
                    tp.Add(new Tuple<ActiveItem, int, int>(amap[key].Item1, amap[key].Item2, key));
                }
                return tp;
            }
            else
            {
                return new List<Tuple<ActiveItem, int, int>>();
            }
        }
        public Tuple<ActiveItem, int> Get(int cat, int cons)
        {
            if (store.ContainsKey(cat))
            {
                Dictionary<int, Tuple<ActiveItem, int>> amap = store[cat];
                if (amap.ContainsKey(cons))
                {
                    return amap[cons];
                }
                else
                {
                    return null;    //TODO check null :/
                }
            }
            else
            {
                return null;    //TODO check this? I don't like null...
            }
        }
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