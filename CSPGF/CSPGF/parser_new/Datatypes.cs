/*
Copyright (c) 2011, Christian Ståhlfors (christian.stahlfors@gmail.com), Erik Bergström (erktheorc@gmail.com)
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above copyright
      notice, this list of conditions and the following disclaimer in the
      documentation and/or other materials provided with the distribution.
    * Neither the name of the <organization> nor the
      names of its contributors may be used to endorse or promote products
      derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/
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

    //public class Chart
    //{
    //    public ActiveChart active { get; private set; }
    //    public List<ActiveChart> actives { get; private set; }
    //    public PassiveChart passive { get; private set; }
    //    public List<reader.ProductionSet> forest { get; private set; }
    //    public int nextId { get; private set; }
    //    public int offset { get; private set; }

    //    public Chart(ActiveChart _active, List<ActiveChart> _actives, PassiveChart _passive, List<reader.ProductionSet> _forest, int _nextId, int _offset)
    //    {
    //        active = _active;
    //        actives = _actives;
    //        passive = _passive;
    //        forest = _forest;
    //        nextId = _nextId;
    //        offset = _offset;
    //    }
    //}

    //public class ErrorState
    //{
    //    public reader.Abstract abs { get; private set; }
    //    public reader.Concrete con { get; private set; }
    //    public Chart ch { get; private set; }

    //    public ErrorState(reader.Abstract _abs, reader.Concrete _con, Chart _ch)
    //    {
    //        abs = _abs;
    //        con = _con;
    //        ch = _ch;
    //    }
    //}
}
