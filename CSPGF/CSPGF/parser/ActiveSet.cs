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

// Check this class, it seems retarded...

namespace CSPGF.Parse
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    class ActiveSet
    {
        Dictionary<int, Dictionary<int, HashSet<ActiveItemInt>>> store;
        public ActiveSet()
        {
            this.store = new Dictionary<int, Dictionary<int, HashSet<ActiveItemInt>>>();
        }
        // check this one, might be wrong...
        public bool Add(int cat, int cons, ActiveItem item, int cons2)
        {
            Dictionary<int, HashSet<ActiveItemInt>> map;
            if (this.store.TryGetValue(cat, out map))
            {
                HashSet<ActiveItemInt> aItems;
                if (map.TryGetValue(cons, out aItems))
                {
                    foreach (ActiveItemInt aii in aItems)
                    {
                        if (aii.Equals(item, cons2)) return false;
                    }
                    aItems.Add(new ActiveItemInt(item, cons2));
                    return true;
                }
                else
                {
                    // TODO this might be wrong (but I don't think so :)
                    aItems = new HashSet<ActiveItemInt>();
                    aItems.Add(new ActiveItemInt(item, cons2));
                    map.Add(cons, aItems);
                }
            }
            else
            {
                map = new Dictionary<int, HashSet<ActiveItemInt>>();
                HashSet<ActiveItemInt> aItems = new HashSet<ActiveItemInt>();
                aItems.Add(new ActiveItemInt(item, cons2));
                map.Add(cons, aItems);
                this.store.Add(cat, map);
            }
            return true;
        }

        // TODO check if this is correct, new version
        public HashSet<ActiveItemInt> Get(int cat)
        {
            HashSet<ActiveItemInt> aai = new HashSet<ActiveItemInt>();
            Dictionary<int, HashSet<ActiveItemInt>> map;
            if (this.store.TryGetValue(cat, out map))
            {
                foreach (int key in map.Keys)
                {
                    foreach (ActiveItemInt i in map[key])
                    {
                        i.Cons2 = key;
                        aai.Add(i);
                    }
                }
            }
            return aai;
        }
        // Also fixed this one I hope /Erik
        public HashSet<ActiveItemInt> Get(int cat, int cons)
        {
            HashSet<ActiveItemInt> aai = new HashSet<ActiveItemInt>();
            Dictionary<int, HashSet<ActiveItemInt>> map;
            if (this.store.TryGetValue(cat, out map))
            {
                if (map.TryGetValue(cons, out aai))
                {
                    return aai;
                }
            }
            return aai;
        }
    }
}

/*
// * this is used to keed track of sets of active items (the S_k)
// * */
// private class ActiveSet {

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
// }