//-----------------------------------------------------------------------
// <copyright file="ParseTrie.cs" company="None">
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

namespace CSPGF.Parse
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The ParseTrie class.
    /// </summary>
    [Serializable]
    internal class ParseTrie
    {
        /// <summary>
        /// A stack of active items.
        /// </summary>
        private Stack<ActiveItem> value;

        /// <summary>
        /// All the childs
        /// </summary>
        private Dictionary<string, ParseTrie> child = new Dictionary<string, ParseTrie>();

        /// <summary>
        /// Initializes a new instance of the ParseTrie class.
        /// </summary>
        public ParseTrie()
        {
            this.value = new Stack<ActiveItem>();
        }

        /// <summary>
        /// Initializes a new instance of the ParseTrie class.
        /// </summary>
        /// <param name="value">A stack of active items.</param>
        public ParseTrie(Stack<ActiveItem> value)
        {
            this.value = value;
        }

        /// <summary>
        /// Insert description.
        /// </summary>
        /// <param name="key">A list of keys.</param>
        /// <param name="value">A stack of active items.</param>
        public void Add(string[] key, Stack<ActiveItem> value)
        {
            this.Add(key.ToList<string>(), value);
        }

        /// <summary>
        /// Insert description.
        /// </summary>
        /// <param name="keys">A list of keys.</param>
        /// <param name="value">A stack of active items.</param>
        public void Add(List<string> keys, Stack<ActiveItem> value)
        {
            if (keys == null || keys.Count == 0) 
            {
                this.value = value;
            }
            else
            {
                List<string> l = new List<string>(keys);
                string x = l.First<string>();
                l.Remove(x);
                ParseTrie newTrie;
                if (!this.child.TryGetValue(x, out newTrie))
                {
                    ParseTrie newN = new ParseTrie();
                    newN.Add(l, value);
                    this.child[x] = newN;
                }
                else
                {
                    newTrie.Add(l, value);
                }
            }
        }

        /// <summary>
        /// Looks for a stack of active items.
        /// </summary>
        /// <param name="key">The keys.</param>
        /// <returns>The stack of active items.</returns>
        public Stack<ActiveItem> Lookup(string[] key)
        {
            return this.Lookup(key.ToList<string>());
        }

        /// <summary>
        /// Look for a stack of active items.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The corresponding stack of active items.</returns>
        public Stack<ActiveItem> Lookup(List<string> key)
        {
            if (this.GetSubTrie(key) != null) 
            {
                return this.GetSubTrie(key).value;
            }
            else 
            {
                return null;
            }
        }
        
        /// <summary>
        /// Returns the subtrie.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The subtrie if any.</returns>
        public ParseTrie GetSubTrie(List<string> key)
        {
            if (key == null || key.Count == 0)
            {
                return this;
            }

            List<string> l = new List<string>(key);
            string x = l.First<string>();
            l.Remove(x);

            ParseTrie newTrie;
            if (this.child.TryGetValue(x, out newTrie))
            {
                return newTrie.GetSubTrie(l);
            }

            return newTrie;
        }

        /// <summary>
        /// Returns the subtrie of one key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The subtrie.</returns>
        public ParseTrie GetSubTrie(string key)
        {
            List<string> tmp = new List<string>();
            tmp.Add(key);
            return this.GetSubTrie(tmp);
        }
        
        /// <summary>
        /// Predicts the next token.
        /// </summary>
        /// <returns>Returns a list of tokens.</returns>
        public List<string> Predict()
        {
            return this.child.Keys.ToList<string>();
        }

        /// <summary>
        /// Creates a string reoresentation of the tree.
        /// </summary>
        /// <returns>The parsetrie as a string.</returns>
        public override string ToString()
        {
            string temp = "[";
            foreach (string key in this.child.Keys)
            {
                ParseTrie t = this.child[key];
                temp += key + "[" + t.ToString() + "]";
            }

            temp += "]";
            return temp;
        }
    }
}
