using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSPGF.Grammar
{
    class ProductionConst : Production
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fId"></param>
        /// <param name="tokens"></param>
        public ProductionConst(int fId, ConcreteFunction function, List<string> tokens) : base(0, fId)
        {
            this.tokens = tokens;
            this.fun = function;
        }

        public List<string> tokens;
        public ConcreteFunction fun;

        /// <summary>
        /// Removes first token, used when we have a Literal with multiple tokens
        /// </summary>
        public void removeFirstToken()
        {
            if (tokens.Count > 0)
            {
                tokens.RemoveAt(0);
            }
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
