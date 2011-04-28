/*
 * MultiMap.java
 * Copyright (C) 2004-2005, Bjorn Bringert (bringert@cs.chalmers.se)
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.util
{
    public class MultiMap<K, V>
    {

        /**
         *  Invariant: the sets have at least one element.
         */
        private Dictionary<K, HashSet<V>> map;

        public MultiMap()
        {
            this.map = new Dictionary<K, HashSet<V>>();
        }

        public HashSet<V> Get(K key) {
                HashSet<V> s = map[key];
                if (s == null) {
                    return new HashSet<V>();
                } else {
                    return s;
                }
                //return s == null ? HashSet.<V>emptySet() : s;
        }

        public Boolean Add(K key, V value)
        {
            HashSet<V> s = null;
            try {
                s = map[key];
            }
            catch (Exception e) {
                e.ToString();
            }
            if (s == null)
            {
                s = new HashSet<V>();
                map.Add(key, s);
            }
            return s.Add(value);
        }

        public List<K> KeySet()
        {
            // WTH?
            return map.Keys.ToList<K>();
        }

        /** 
         *  Checks whether there is at least one value for the 
         *  given key.
         */
        public Boolean ContainsKey(K key)
        {
            return map.ContainsKey(key);
        }

        public override String ToString()
        {
            return map.ToString();
        }

        /**
         *  Returns the all the values in the map.
         *  FIXME: The result is not backed by the map.
         */
        public List<V> Values() {
                List<V> l = new List<V>();
                foreach (KeyValuePair<K, HashSet<V>> s in map)
                    foreach (V t in s.Value)
                                l.Add(t);
                return l;
        }

    }
}
