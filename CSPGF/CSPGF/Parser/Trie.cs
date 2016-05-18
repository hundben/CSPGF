//-----------------------------------------------------------------------
// <copyright file="Trie.cs" company="None">
//  Copyright (c) 2011-2016, Christian Ståhlfors (christian.stahlfors@gmail.com), 
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
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// The parse tree.
    /// </summary>
    internal class Trie
    {
        /// <summary>
        /// Initializes a new instance of the Trie class.
        /// </summary>
        public Trie()
        {
            this.Value = new List<ActiveItem>();
            this.Items = new Dictionary<string, Trie>();
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public List<ActiveItem> Value { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        public Dictionary<string, Trie> Items { get; set; }

        /// <summary>
        /// Inserts a new tree for each token in keys.
        /// </summary>
        /// <param name="keys">The list of keys.</param>
        /// <param name="obj">The tree.</param>
        /// <returns>The new tree.</returns>
        public Trie InsertChain(List<string> keys, Trie obj)
        {
            var node = this;
            foreach (string key in keys)
            {
                Trie nnode;
                if (this.Items.ContainsKey(key))
                {
                    nnode = node.Items[key];
                }
                else
                {
                    nnode = new Trie();
                    node.Items[key] = nnode;
                }

                node = nnode;
            }

            node.Value = obj.Value; 
            return node;
        }

        /// <summary>
        /// Inserts a chain of trees.
        /// </summary>
        /// <param name="keys">The list of tokens.</param>
        /// <param name="obj">The tree used.</param>
        /// <returns>The new tree.</returns>
        public Trie InsertChain1(List<string> keys, Trie obj)
        {
            var node = this;
            foreach (string key in keys)
            {
                Trie nnode;
                if (this.Items.ContainsKey(key))
                {
                    nnode = node.Items[key];
                }
                else
                {
                    nnode = new Trie();
                    node.Items[key] = nnode;
                }

                node = nnode;
            }
            
            if (node.Value.Count == 0)
            {
                node.Value = new List<ActiveItem>();
                node.Value.AddRange(obj.Value);
            }
            else
            {
                node.Value.AddRange(obj.Value);
            }

            return node;
        }

        /// <summary>
        /// Looks for a tree for the corresponding token.
        /// </summary>
        /// <param name="key">The token.</param>
        /// <returns>The corresponding tree.</returns>
        public Trie Lookup(string key)
        {
            if (this.Items.ContainsKey(key))
            {
                return this.Items[key];
            }
            else
            {
                return new Trie();
            }
        }

        /// <summary>
        /// Checks if the tree is empty.
        /// </summary>
        /// <returns>Returns true if empty.</returns>
        public bool IsEmpty()
        {
            // TODO value should not be null
            if (this.Value.Count != 0)
            {
                return false;
            }

            if (this.Items.Count > 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Used to predict the next correct tokens
        /// </summary>
        /// <returns>A list of predictions</returns>
        public List<string> Predict()
        {
            return this.Items.Keys.ToList<string>();
        }
    }
}
