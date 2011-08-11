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

namespace CSPGF.Parser_new
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Active
    {
        public Active(int integer, int dotpos, int funid, int seqid, List<PArg> parg, AK activekey)
        {
            this.Integer = integer;
            this.Dotpos = dotpos;
            this.Funid = funid;
            this.Seqid = seqid;
            this.Parg = parg;
            this.Activekey = activekey;
        }

        public int Integer { get; private set; }

        public int Dotpos { get; private set; }

        public int Funid { get; private set; }

        public int Seqid { get; private set; }

        public List<PArg> Parg { get; private set; }

        public AK Activekey { get; private set; }
    }

    public class AK
    {
        public AK(int fid, int index)
        {
            this.Fid = fid;
            this.LIndex = index;
        }

        /// <summary>
        /// Gets Fid
        /// </summary>
        public int Fid { get; private set; }

        public int LIndex { get; private set; }
    }

    public class ActiveSet
    {
        public ActiveSet(HashSet<Active> active)
        {
            this.Active = active;
        }

        public HashSet<Active> Active { get; private set; }
    }

    public class ActiveChart
    {
        //IntMap.IntMap (IntMap.IntMap (ActiveSet, IntMap.IntMap (Set.Set Production)))
        public ActiveChart(Dictionary<int, Tuple<ActiveSet, Dictionary<int, HashSet<Reader.Production>>>> ac)
        {
            this.AC = ac;
        }

        public Dictionary<int, Tuple<ActiveSet, Dictionary<int, HashSet<Reader.Production>>>> AC { get; private set; } 
    }

    public class PArg
    {
        public PArg(List<Tuple<int, int>> list, int fid)
        {
            this.List = list;
            this.Fid = fid;
        }

        public List<Tuple<int, int>> List { get; private set; }

        public int Fid { get; private set; }
    }

    public class PK
    {
        public PK(int fid, int lindex, int integer)
        {
            this.Fid = fid;
            this.Lindex = lindex;
            this.Integer = integer;
        }

        public int Fid { get; private set; }

        public int Lindex { get; private set; }

        public int Integer { get; private set; }
    }

    public class PassiveChart
    {
        public PassiveChart(Dictionary<PK, int> pc)
        {
            this.PC = pc;
        }

        public Dictionary<PK, int> PC { get; private set; }
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
