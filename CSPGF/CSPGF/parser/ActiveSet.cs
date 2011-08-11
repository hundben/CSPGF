//-----------------------------------------------------------------------
// <copyright file="ActiveSet.cs" company="None">
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

// Check this class, it seems retarded...
namespace CSPGF.Parse
{
    using System.Collections.Generic;

    /// <summary>
    /// Contains all the current active items.
    /// </summary>
    public class ActiveSet
    {
        /// <summary>
        /// Dictionary where everything is stored
        /// </summary>
        private Dictionary<int, Dictionary<int, HashSet<ActiveItemInt>>> store;

        /// <summary>
        /// Initialized a new instance of the ActiveSet class.
        /// </summary>
        public ActiveSet()
        {
            this.store = new Dictionary<int, Dictionary<int, HashSet<ActiveItemInt>>>();
        }

        // check this one, might be wrong...
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cat"></param>
        /// <param name="cons"></param>
        /// <param name="item"></param>
        /// <param name="cons2"></param>
        /// <returns></returns>
        public bool Add(int cat, int cons, ActiveItem item, int cons2)
        {
            Dictionary<int, HashSet<ActiveItemInt>> map;
            if (this.store.TryGetValue(cat, out map))
            {
                HashSet<ActiveItemInt> activeItems;
                if (map.TryGetValue(cons, out activeItems))
                {
                    foreach (ActiveItemInt aii in activeItems)
                    {
                        if (aii.Equals(item, cons2))
                        {
                            return false;
                        }
                    }

                    activeItems.Add(new ActiveItemInt(item, cons2));
                    return true;
                }
                else
                {
                    // TODO this might be wrong (but I don't think so :)
                    activeItems = new HashSet<ActiveItemInt>();
                    activeItems.Add(new ActiveItemInt(item, cons2));
                    map.Add(cons, activeItems);
                }
            }
            else
            {
                map = new Dictionary<int, HashSet<ActiveItemInt>>();
                HashSet<ActiveItemInt> activeItems = new HashSet<ActiveItemInt>();
                activeItems.Add(new ActiveItemInt(item, cons2));
                map.Add(cons, activeItems);
                this.store.Add(cat, map);
            }

            return true;
        }

        // TODO check if this is correct, new version
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cat"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cat"></param>
        /// <param name="cons"></param>
        /// <returns></returns>
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