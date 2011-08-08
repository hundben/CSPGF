
namespace CSPGF.Linearizer_new
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using CSPGF.Reader;
    using CSPGF.Parser_new;

    class Linearizer
    {
        string sentence = string.Empty;
        List<LinTrie> curLvl;
        List<ParseTrie> curParse;
        public Linearizer()
        {
            this.curLvl = new List<LinTrie>();
            this.curParse = new List<ParseTrie>();
        }

        public string Linearize(LinTrie tree)
        {
            this.curLvl.Clear();
            this.curLvl.Add(tree);
            while (this.curLvl.Count != 0) {
                this.sentence += this.Linearizer2(this.curLvl);
                this.NextLevel();
            }
            return this.sentence;
        }

        private void NextLevel()
        {
            List<LinTrie> newlvl = new List<LinTrie>();
            foreach (LinTrie lt in this.curLvl) {
                foreach (LinTrie lt2 in lt.child) {
                    if (lt2 != null) {
                        newlvl.Add(lt2);
                    }
                }
            }
            this.curLvl = newlvl;
        }

        private string Linearizer2(List<LinTrie> trees)
        {
            string tmp = string.Empty;
            for (int i = 0; i < trees.Count; i++) {
                if (trees[i].symbol is AlternToksSymbol) {
                    if (i < (trees.Count - 1) && trees[i + 1].symbol is ToksSymbol) {
                        tmp += this.ATSym2St((AlternToksSymbol)trees[i].symbol, (ToksSymbol)trees[i + 1].symbol);
                        i++;
                    } 
                    else 
                    {
                        throw new LinearizerException("fail med altsymbol, i = " + i + " nextSym = " + trees[i + 1].symbol.GetType());
                    }
                } 
                else if (trees[i].symbol is ToksSymbol) {
                    foreach (string str in ((ToksSymbol)trees[i].symbol).tokens) {
                        tmp += str + " ";
                    }
                    tmp = tmp.TrimEnd();
                } 
                else 
                {
                    throw new LinearizerException("ohanterad typ av symbol: " + trees[i].symbol.GetType());
                }
            }
            return tmp;
        }

        private string ATSym2St(AlternToksSymbol s, ToksSymbol nextToken)
        {
            // Should be possible to optimise this...
            string tmp = string.Empty;
            foreach (Alternative alt in s.alts) {
                foreach (string str in nextToken.tokens) {
                    foreach (string str2 in alt.alt2) {
                        if (str.StartsWith(str2)) {
                            foreach (string str3 in alt.alt1) {
                                tmp += str3 + " ";
                            }
                            return tmp.TrimEnd();
                        }
                    }
                }
            }
            foreach (string str4 in s.tokens) {
                tmp += str4 + " ";
            }
            return tmp.TrimEnd();
        }

        public LinTrie Parse2Lin(ParseTrie pt)
        {
            LinTrie lt = new LinTrie();
            List<string> tok = new List<string>();
            tok.Add(this.Seq2Str(pt.symbol.function.sequences));
            lt.symbol = new ToksSymbol(tok);
            lt.child.Add(this.P2L(pt));
            return null;
        }

        public string Seq2Str(List<Sequence> seqs)
        {
            string str = string.Empty;
            foreach (Sequence seq in seqs) {
                for (int i = 0; i < seq.symbs.Count; i++) {
                    if (seq.symbs[i] is ToksSymbol) {
                        foreach (string str2 in ((ToksSymbol)seq.symbs[i]).tokens) {
                            str += str2 + " ";
                        }
                    }
                    else if (i < (seq.symbs.Count - 1) && seq.symbs[i] is AlternToksSymbol) {
                        str += this.ATSym2St((AlternToksSymbol)seq.symbs[i], (ToksSymbol)seq.symbs[i + 1]);
                        i++;
                    }
                    else {
                        throw new LinearizerException("Failsymbol: " + seq.symbs[i].GetType());
                    }
                }

            }
            return str;
        }

        public LinTrie P2L(ParseTrie pt) {
            LinTrie lt = new LinTrie();
            if (pt.child.Count == 0) {
                return lt;
            }
            else {
                foreach (ParseTrie ptt in pt.child) {
                    lt.child.Add(this.P2L(ptt));
                }
            }
            return lt;
        }
    }

    class LinTrie
    {
        public List<LinTrie> child { get; set; }
        public Symbol symbol { get; set; }
        public LinTrie()
        {
            this.child = new List<LinTrie>();
        }
    }
}