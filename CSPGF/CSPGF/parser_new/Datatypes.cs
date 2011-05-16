using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSPGF.parser_new
{
    public class Active
    {
        public int _int { get; private set; }
        public int dotpos { get; private set; }
        public int funid { get; private set; }
        public int seqid { get; private set; }
        public List<PArg> parg { get; private set; }
        public AK activekey { get; private set; }
        public Active(int __int, int _dotpos, int _funid, int _seqid, List<PArg> _parg, AK _activekey)
        {
            _int = __int;
            dotpos = _dotpos;
            funid = _funid;
            seqid = _seqid;
            parg = _parg;
            activekey = _activekey;
        }
    }
    public class AK
    {
        public int Fid { get; private set; }
        public int LIndex { get; private set; }
        AK(int _Fid, int _LIndex)
        {
            Fid = _Fid;
            LIndex = _LIndex;
        }
    }
    public class ActiveSet
    {
        public HashSet<Active> active { get; private set; }
        public ActiveSet(HashSet<Active> _active)
        {
            active = _active;
        }
    }
    public class ActiveChart
    {
        public Dictionary<int, Tuple<ActiveSet, Dictionary<int, HashSet<reader.Production>>>> ac { get; private set; }
        //IntMap.IntMap (IntMap.IntMap (ActiveSet, IntMap.IntMap (Set.Set Production)))
        public ActiveChart(Dictionary<int, Tuple<ActiveSet, Dictionary<int, HashSet<reader.Production>>>> _ac)
        {
            ac = _ac;
        }
    }

    public class PArg
    {
        public List<Tuple<int, int>> list { get; private set; }
        public int fid { get; private set; }
        public PArg(List<Tuple<int, int>> _list, int _fid)
        {
            list = _list;
            fid = _fid;
        }
    }

    public class PK
    {
        public int fid { get; private set; }
        public int lindex { get; private set; }
        public int _int { get; private set; }
        public PK(int _fid, int _lindex, int __int)
        {
            fid = _fid;
            lindex = _lindex;
            _int = __int;
        }
    }
    public class PassiveChart
    {
        public Dictionary<PK, int> pc { get; private set; }
        public PassiveChart(Dictionary<PK, int> _pc)
        {
            pc = _pc;
        }
    }

    /*
     * data Chart
  = Chart
      { active  :: ActiveChart
      , actives :: [ActiveChart]
      , passive :: PassiveChart
      , forest  :: IntMap.IntMap (Set.Set Production)
      , nextId  :: {-# UNPACK #-} !FId
      , offset  :: {-# UNPACK #-} !Int
      }
     * */

    public class Chart
    {
        public ActiveChart active { get; private set; }
        public List<ActiveChart> actives { get; private set; }
        public PassiveChart passive { get; private set; }
        public Dictionary<int, HashSet<reader.Production>> forest { get; private set; }
        public int nextId { get; private set; }
        public int offset { get; private set; }
        public Chart()
        {
            active = new ActiveChart(null);
            actives = new List<ActiveChart>();
            passive = new PassiveChart(null);
            forest = new Dictionary<int, HashSet<reader.Production>>();
        }
    }
}
