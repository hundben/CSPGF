using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSPGF.Parse
{
    using CSPGF.Grammar;


    class ActiveItem2
    {
        public int offset;

        public int dot;

        public CncFun fun;

        public List<Symbol> seq;

        public List<int> args;

        public int fid;

        public int lbl;

        public ActiveItem2(int offset, int dot, CncFun fun, List<Symbol> seq, List<int> args, int fid, int lbl) 
        {
            this.offset = offset;
            this.dot = dot;
            this.fun = fun;
            this.seq = seq;
            this.args = args;
            this.fid = fid;
            this.lbl = lbl;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is ActiveItem2)
            {
                var obj2 = (ActiveItem2)obj;
                return (this.offset == obj2.offset &&
                    this.dot == obj2.dot &&
                    this.fun == obj2.fun &&
                    this.seq == obj2.seq &&
                    this.args == obj2.args &&
                    this.fid == obj2.fid &&
                    this.lbl == obj2.lbl);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="fid"></param>
        /// <returns></returns>
        public ActiveItem2 shiftOverArg(int i, int fid)
        {
            var nargs = new List<int>();
            foreach (int k in this.args)
            {
                nargs.Add(k);
            }

            nargs[i] = fid;
            return new ActiveItem2(this.offset, this.dot + 1, this.fun, this.seq, nargs, this.fid, this.lbl);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActiveItem2 shiftOverTokn()
        {
            return new ActiveItem2(this.offset, this.dot + 1, this.fun, this.seq, this.args, this.fid, this.lbl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }
}
