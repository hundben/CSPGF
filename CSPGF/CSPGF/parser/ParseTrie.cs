// -----------------------------------------------------------------------
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
// -----------------------------------------------------------------------

namespace CSPGF.Parse
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal class ParseTrie
    {
        /// <summary>
        /// The agenda connected to this node.
        /// </summary>
        private Stack<ActiveItem> value = new Stack<ActiveItem>();

        /// <summary>
        /// List of sub trees
        /// </summary>
        private Dictionary<string, ParseTrie> childs = new Dictionary<string, ParseTrie>();

        /// <summary>
        /// Initializes a new instance of the ParseTrie class.
        /// </summary>
        public ParseTrie()
        {
        }

        /// <summary>
        /// Add a key or a list of keys to the parse trie with the corresponding agenda.
        /// </summary>
        /// <param name="keys">A list of strings, one subtrie for each one.</param>
        /// <param name="value">Value attached to the subtrie of the last key.</param>
        public void Add(List<string> keys, Stack<ActiveItem> value)
        {
            ParseTrie newTrie = this;

            foreach (string k in keys)
            {
                if (!newTrie.childs.ContainsKey(k))
                {
                    ParseTrie tt = new ParseTrie();
                    newTrie.childs[k] = tt;
                    newTrie = tt;
                }
                else
                {
                    newTrie = newTrie.childs[k];
                }
            }

            newTrie.value = value;
        }

        /// <summary>
        /// Searches the current node for the agenda of a subtrie with the corresponding key.
        /// </summary>
        /// <param name="key">The key to search for.</param>
        /// <returns>The agenda.</returns>
        public Stack<ActiveItem> Lookup(string key)
        {
            ParseTrie trie;
            if (this.childs.TryGetValue(key, out trie))
            {
                return trie.value;
            }

            if (key == string.Empty)
            {
                return this.value;
            }

            return null;
        }

        /// <summary>
        /// Same as Lookup but searches down the trie.
        /// TODO this might be wrong, scala version only checks first key
        /// </summary>
        /// <param name="keys">The list of keys.</param>
        /// <returns>The agenda.</returns>
        public Stack<ActiveItem> Lookup(List<string> keys)
        {
            return this.Lookup(keys.First());
        }

        /// <summary>
        /// Returns the subtrie that corresponds to the key.
        /// </summary>
        /// <param name="key">The key we want to search for.</param>
        /// <returns>The subtrie.</returns>
        public ParseTrie GetSubTrie(string key)
        {
            ParseTrie trie;
            if (this.childs.TryGetValue(key, out trie))
            {
                return this.MakeCopy(trie);
            }

            return null;
        }

        /// <summary>
        /// Same as with one key but searches down the trie.
        /// </summary>
        /// <param name="keys">A list of keys.</param>
        /// <returns>The subtrie.</returns>
        public ParseTrie GetSubTrie(List<string> keys)
        {
            ParseTrie trie = this;
            foreach (string k in keys)
            {
                // TODO optimize to use TryGet
                if (trie.childs.ContainsKey(k))
                {
                    trie = trie.childs[k];
                }
                else
                {
                    return null;
                }
            }

            return this.MakeCopy(trie);
        }

        /// <summary>
        /// Returns the predictions of the current node.
        /// </summary>
        /// <returns>A list of predictions (tokens).</returns>
        public List<string> Predict()
        {
            return this.childs.Keys.ToList<string>();
        }

        /// <summary>
        /// Creates a string representation of the tree.
        /// </summary>
        /// <returns>The parsetrie as a string.</returns>
        public override string ToString()
        {
            string temp = "(" + this.value.Count + ")";

            if (this.value.Count > 0)
            {
                temp += "{";

                foreach (ActiveItem ai in this.value.ToList())
                {
                    temp += ai;
                }

                temp += "} ";
            }

            temp += "[";

            foreach (string key in this.childs.Keys)
            {
                ParseTrie t = this.childs[key];
                temp += key + "[" + t + "]";
            }

            temp += "]";
            return temp;
        }

        /// <summary>
        /// Returns a copy of the ParseTrie.
        /// </summary>
        /// <param name="trie">The trie we want to copy.</param>
        /// <returns>A copy of the trie</returns>
        private ParseTrie MakeCopy(ParseTrie trie)
        {
            ParseTrie t = new ParseTrie();
            t.value = new Stack<ActiveItem>(trie.value.Reverse());
 
            foreach (string k in trie.childs.Keys)
            {
                t.childs[k] = this.MakeCopy(trie.childs[k]);
            }

            return t;
        }
    }
}
