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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Contains all the current active items.
    /// </summary>
    [Serializable]
    public class ActiveSet
    {
        /// <summary>
        /// Dictionary where everything is stored
        /// </summary>
        private Dictionary<int, Dictionary<int, HashSet<ActiveItem>>> store;

        /// <summary>
        /// Initializes a new instance of the ActiveSet class.
        /// </summary>
        public ActiveSet()
        {
            this.store = new Dictionary<int, Dictionary<int, HashSet<ActiveItem>>>();
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
            Dictionary<int, HashSet<ActiveItem>> map;
            if (this.store.TryGetValue(cat, out map))
            {
                HashSet<ActiveItem> activeItems;
                if (map.TryGetValue(cons, out activeItems))
                {
                    foreach (ActiveItem ai in activeItems)
                    {
                        if (ai.Equals(item))
                        {
                            return false;
                        }
                    }

                    activeItems.Add(item);
                    return true;
                }
                else
                {
                    activeItems = new HashSet<ActiveItem>();
                    activeItems.Add(item);
                    map.Add(cons, activeItems);
                }
            }
            else
            {
                map = new Dictionary<int, HashSet<ActiveItem>>();
                HashSet<ActiveItem> activeItems = new HashSet<ActiveItem>();
                activeItems.Add(item);
                map.Add(cons, activeItems);
                this.store.Add(cat, map);
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cat"></param>
        /// <returns></returns>
        public HashSet<ActiveItem> Get(int cat)
        {
            HashSet<ActiveItem> ai = new HashSet<ActiveItem>();
            Dictionary<int, HashSet<ActiveItem>> map;
            if (this.store.TryGetValue(cat, out map))
            {
                foreach (int key in map.Keys)
                {
                    foreach (ActiveItem i in map[key])
                    {
                        ai.Add(i);
                    }
                }
            }

            return ai;
        }
    }
}
