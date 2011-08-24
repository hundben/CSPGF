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

    [Serializable]
    public class ParseTrie
    {
        private Stack<ActiveItem> value;
        private Dictionary<string, ParseTrie> child = new Dictionary<string, ParseTrie>();

        public ParseTrie()
        {
            this.value = new Stack<ActiveItem>();
        }

        public ParseTrie(Stack<ActiveItem> value)
        {
            this.value = value;
        }

        public void Add(string[] key, Stack<ActiveItem> value)
        {
            this.Add(key.ToList<string>(), value);
        }

        public void Add(List<string> keys, Stack<ActiveItem> value)
        {
            // TODO: Might be correct, but check.
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

        public Stack<ActiveItem> Lookup(string[] key)
        {
            return this.Lookup(key.ToList<string>());
        }

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
        
        public ParseTrie GetSubTrie(List<string> key)
        {
            // TODO check if null is necessary
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

        public ParseTrie GetSubTrie(string key)
        {
            List<string> tmp = new List<string>();
            tmp.Add(key);
            return this.GetSubTrie(tmp);
        }
        
        public List<string> Predict()
        {
            return this.child.Keys.ToList<string>();
        }
        
        public override string ToString()
        {
            return this.ToStringWithPrefix(string.Empty);
        }
        
        public string ToStringWithPrefix(string prefix)
        {
            // RETARDKOD! TODO: Gör klart!
            string tmp = prefix + "<" + this.value.ToString() + ">";
            return tmp;
        }

        //  def toStringWithPrefix(prefix:String):String = {
        //    prefix + "<" + this.value + ">" +
        //    this.child.keys.map(k =>
        //      prefix + k.toString + ":\n" +
        //      this.child(k).toStringWithPrefix(prefix + "  ")
        //    ).foldLeft("")((a:String,b:String) => a + "\n" + b)
        //  }
    }
}

// /* ************************************************************************* */
// /**
// * The ParseTries are used to keep track of the possible next symbols.
// * It is a trie where the symbol (edge labels) are string (words) and the values (node) are agendas
// * (stacks of ActiveItems)
// * The parse tries is used in the parsing algorithm when a dot is before a token. Then the dot is
// * moved after the tokens and the resulting active item is added to the trie (to the agenda indexed by
// * the words of the token.)
// * Then the scan operation is a simple lookup in the trie...
// * The trie is also used for predictions.
// * In gf, a token in a rule can consist of multiple words (separated by a whitespace), thus the trie is
// * needed and cannot be replaced by a simple map.
// *
// * @param value the value at this node.
// * */
//private class ParseTrie(var value:Stack[ActiveItem]) {
//  import scala.collection.mutable.HashMap

//  val child = new HashMap[String,ParseTrie]

//  def this() = this(new Stack)

//  def add(key:Seq[String], value:Stack[ActiveItem]):Unit =
//    this.add(key.toList, value)

//  def add(keys:List[String], value:Stack[ActiveItem]):Unit =
//    keys match {
//      case Nil => this.value = value
//      case x::l => this.child.get(x) match {
//        case None => {
//          val newN = new ParseTrie
//          newN.add(l,value)
//          this.child.update(x, newN)
//        }
//        case Some(n) => n.add(l,value)
//      }
//    }

//  def lookup(key:Seq[String]):Option[Stack[ActiveItem]] =
//    this.lookup(key.toList)

//  def lookup(key:List[String]):Option[Stack[ActiveItem]] =
//    getSubTrie(key) match {
//      case None => None
//      case Some(t) => Some(t.value)
//    }

//  def lookup(key:String):Option[Stack[ActiveItem]] =
//    this.lookup(key::Nil)

//  def getSubTrie(key:List[String]):Option[ParseTrie] =
//    key match {
//      case Nil => Some(this)
//      case x::l => this.child.get(x) match {
//        case None => None
//        case Some(n) => n.getSubTrie(l)
//      }
//    }

//  def getSubTrie(key:String):Option[ParseTrie] =
//    this.getSubTrie(key::Nil)

//  def predict():Array[String] = this.child.keySet.toArray

//  override def toString() = this.toStringWithPrefix("")

//  def toStringWithPrefix(prefix:String):String = {
//    prefix + "<" + this.value + ">" +
//    this.child.keys.map(k =>
//      prefix + k.toString + ":\n" +
//      this.child(k).toStringWithPrefix(prefix + "  ")
//    ).foldLeft("")((a:String,b:String) => a + "\n" + b)
//  }
// }