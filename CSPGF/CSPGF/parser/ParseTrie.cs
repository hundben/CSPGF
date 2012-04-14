// -----------------------------------------------------------------------
// <copyright file="ParseTrie2.cs" company="">
// TODO: Update copyright text.
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

        private Stack<ActiveItem> value = new Stack<ActiveItem>();

        private Dictionary<string, ParseTrie> childs = new Dictionary<string,ParseTrie>();

        public ParseTrie()
        {

        }

        public void Add(List<string> keys, Stack<ActiveItem> value)
        {
            ParseTrie newTrie = this;

            foreach (string k in keys)
            {
                if (!childs.ContainsKey(k))
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

            // Check if correct
            newTrie.value = value;
        }

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

        public Stack<ActiveItem> Lookup(List<string> keys)
        {
            if (keys.Count == 0)
            {
                return this.value;
            }
            Stack<ActiveItem> items = new Stack<ActiveItem>();
            ParseTrie currentTrie = this;
            foreach (string k in keys)
            {
                if (this.childs.ContainsKey(k))
                {
                    currentTrie = this.childs[k];
                }
                else
                {
                    return null;
                }
            }

            return currentTrie.value ;
        }


        public ParseTrie GetSubTrie(string key)
        {
            ParseTrie trie;
            if (this.childs.TryGetValue(key, out trie))
            {
                return trie;
            }
            return null;
        }

        public ParseTrie GetSubTrie(List<string> keys)
        {
            ParseTrie trie = this;
            foreach (string k in keys)
            {
                // TODO optimize to use TryGet
                if (trie.childs.ContainsKey(k))
                {
                    trie = childs[k];
                }
                else
                {
                    return null;
                }
            }
            return trie;
        }

        public List<string> Predict()
        {
            return this.childs.Keys.ToList<string>();
        }

        public void ResetChild(string token)
        {
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
    }
}
