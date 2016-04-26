/*namespace CSPGF.Linearize
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Grammar;
    using Trees.Absyn;
    class Linearizer2
    {
        public Linearizer2()
        {

        }
        
        public List<string> linearize(Tree absyn)
        {
            var res = this.linearizeSyms(absyn, "0");
            var ret = this.unlex(this.sym2toks(res[0].table[0]));
            return new List<string>();
        }

        public string linearizeSyms(Tree tree, string token)
        {
            return string.Empty;
        }

        public string sym2toks(List<LeafKS2> list)
        {
            var ts = new List<string>();
            foreach (LeafKS2 leaf in list)
            {
                if (leaf is LeafKS2)
                {
                    foreach (string str in ((LeafKS2)leaf).Tokens)
                    {
                        ts.Add(this.tagit(str,leaf.tag);
                    }
                }
            }
            return string.Empty;
        }

        public string unlex(string str)
        {
            return string.Empty;
        }

        public string tagit(string leaf, string tag)
        {
            if(isString(leaf))
            {

            }
            /*GFConcrete.prototype.tagIt = function (obj, tag) {
              if (isString(obj)) {
	            var o = new String(obj);
	            o.setTag(tag);
	            return o;
              } else {
	            var me = arguments.callee;
                if (arguments.length == 2) {
                  me.prototype = obj;
                  var o = new me();
                  o.tag = tag;
                  return o;
                }
              }
            };*//*
            return string.Empty;
        }

    }
}
*/