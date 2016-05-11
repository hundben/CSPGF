using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSPGF.Parse
{
    using CSPGF.Grammar;
    class Trie
    {
        public List<ActiveItem2> value;
        public Dictionary<string, Trie> items;

        /// <summary>
        /// 
        /// </summary>
        public Trie()
        {
            value = new List<ActiveItem2>();
            items = new Dictionary<string, Trie>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Trie insertChain(List<string> keys, Trie obj)
        {
            var node = this;
            foreach(string key in keys)
            {
                Trie nnode;
                if (this.items.ContainsKey(key))
                {
                    nnode = node.items[key];
                }
                else
                {
                    nnode = new Trie();
                    node.items[key] = nnode;
                }

                node = nnode;
            }

            node.value = obj.value; 
            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Trie insertChain1(List<string> keys, Trie obj)
        {
            var node = this;
            foreach (string key in keys)
            {
                Trie nnode;
                if (this.items.ContainsKey(key))
                {
                    nnode = node.items[key];
                }
                else
                {
                    nnode = new Trie();
                    node.items[key] = nnode;
                }

                node = nnode;
            }
            
            if (node.value.Count == 0)
            {
                node.value = new List<ActiveItem2>();
                node.value.AddRange(obj.value);
            }
            else
            {
                node.value.AddRange(obj.value);
            }

            return node;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <param name="items"></param>
        public Trie lookup(string key)
        {
            if (this.items.ContainsKey(key))
            {
                return this.items[key];
            }
            else
            {
                return new Trie();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool isEmpty()
        {
            // TODO value should not be null
            if (this.value.Count != 0)
            {
                return false;
            }
            if (items.Count > 0)
            {
                return false;
            }

            return true;
        }
    }
}
