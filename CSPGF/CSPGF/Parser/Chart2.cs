using CSPGF.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSPGF.Parse
{
    class Chart2
    {
        private Dictionary<int, Dictionary<int, List<ActiveItem2>>> active;
        private List<Dictionary<int, Dictionary<int, List<ActiveItem2>>>> actives;
        private Dictionary<string, int> passive;
        public Dictionary<int, List<Production>> forest;
        public int nextId;
        public int offset;

        public Chart2(Concrete concrete)
        {
            this.active = new Dictionary<int, Dictionary<int, List<ActiveItem2>>>();
            this.actives = new List<Dictionary<int, Dictionary<int, List<ActiveItem2>>>>();
            this.passive = new Dictionary<string, int>();
            this.forest = new Dictionary<int, List<Production>>();
            this.nextId = concrete.FId; //  TODO fix so that it uses totalFID instead
            this.offset = 0;

            // TODO check if we need to copy or not (copy now to be safe)
            foreach(KeyValuePair<int, List<Production>> entry in concrete.Productions)
            {
                var temp = new List<Production>();
                foreach(Production p in entry.Value)
                {
                    temp.Add(p);
                }

                this.forest.Add(entry.Key, temp);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public List<ActiveItem2> lookupAC(int fid, int label)
        {
            if (this.active.ContainsKey(fid))
            {
                if (this.active[fid].ContainsKey(label))
                {
                    return this.active[fid][label];
                }
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="fid"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public List<ActiveItem2> lookupACo(int offset, int fid, int label)
        {
            //var tmp;
            if (offset == this.offset)
            {
                try
                {
                    return this.active[fid][label];
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                try
                {
                    return this.actives[offset][fid][label];
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public Dictionary<int, List<ActiveItem2>> labelsAC(int fid)
        {
            return this.active[fid];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="label"></param>
        /// <param name="items"></param>
        public void insertAC(int fid, int label, List<ActiveItem2> items)
        {
            if (this.active.ContainsKey(fid))
            {
                this.active[fid][label] = items;
            }
            else
            {
                var temp = new Dictionary<int, List<ActiveItem2>>();
                temp[label] = items;
                this.active[fid] = temp;
            }         
        }

        /// <summary>
        /// TODO change label to correct type
        /// </summary>
        /// <param name="fid"></param>
        /// <param name="label"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public int? lookupPC(int fid, int label, int offset)
        {
            string key = fid + "." + label + "-" + offset;
            if (this.passive.ContainsKey(key))
            {
                return this.passive[key];
            }

            return null;
        }

        /// <summary>
        /// TODO change label to correct type
        /// </summary>
        /// <param name="fid1"></param>
        /// <param name="label"></param>
        /// <param name="offset"></param>
        /// <param name="fid2"></param>
        public void insertPC(int fid1, int label, int offset, int fid2)
        {
            string key = fid1 + "." + label + "-" + offset;
            this.passive[key] = fid2;
        }

        /// <summary>
        /// 
        /// </summary>
        public void shift()
        {
            this.actives.Add(this.active);
            this.active = new Dictionary<int, Dictionary<int, List<ActiveItem2>>>();
            this.passive = new Dictionary<string, int>();
            this.offset++;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fid"></param>
        /// <returns></returns>
        public List<Production> expandForest(int fid)
        {
            var rules = new List<Production>();

            foreach (Production p in this.forest[fid])
            {
                if (p is ProductionApply || p is ProductionConst)
                {
                    rules.Add(p);
                }
                else if (p is ProductionCoerce) 
                {
                    ProductionCoerce pc = (ProductionCoerce)p;
                    var moreRules = expandForest(pc.InitId);
                    foreach(Production p2 in moreRules)
                    {

                        rules.Add((ProductionApply)p2);
                    }
                }
            }

            return rules;
        }
    }
}
