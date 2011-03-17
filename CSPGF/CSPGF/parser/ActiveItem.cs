using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CncFun = CSPGF.reader.CncFun;
using Symbol = CSPGF.reader.Symbol;

namespace CSPGF.parser
{
    class ActiveItem
    {
        public int begin;
        public int category;
        public CncFun function;
        public int[] domain;
        public int constituent;
        public int position;
        public ActiveItem(int _begin, int _category, CncFun _function, int[] _domain, int _constituent, int _position)
        {
            begin = _begin;
            category = _category;
            function = _function;
            domain = _domain;
            constituent = _constituent;
            position = _position;
        }
        //Notice that Option and Same is used together in scala, so ignore :D
        public Symbol NextSymbol()
        {
            if (position < function.GetSequence(constituent).GetLength())
            {
                Symbol sym = function.GetSequence(constituent).GetSymbol(position);
                return sym;
            }
            return null;    //this might be dangerous
        }
        //equals method
        public override bool Equals(ActiveItem ai)
        {
            if (begin == ai.begin &&
                category == ai.category &&
                function == ai.function &&
                constituent == ai.constituent &&
                position == ai.position)
            {
                //Since there is no deep method in c# that we know of, use a crappy forloop
                if (domain.Length == ai.domain.Length)
                {
                    for (int i = 0; i < domain.Length; i++)
                    {
                        if (domain[i] != ai.domain[i]) return false;
                    }
                    return true;
                }
            }

            return false;
        }
        //To string
        public override string ToString()
        {
            String str = "[" + begin.ToString() + ";" + category.ToString() + 
                "->" + this.function.name + "[" + DomainToString() + "];" + 
                constituent.ToString() + ";" + position.ToString() + "]";
            return str;
        }
        //Helper method
        public String DomainToString()
        {
            String tot = "";
            foreach(int d in domain)
            {
                tot += d.ToString();
            }
            return tot;
        }
    }
}
