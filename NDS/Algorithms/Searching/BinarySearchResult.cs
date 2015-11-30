using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Algorithms.Searching
{
    public struct BinarySearchResult
    {
        public BinarySearchResult(int index, bool found)
            : this()
        {
            this.Index = index;
            this.Found = found;
        }

        public bool Found { get; private set; }
        public int Index { get; private set; }
    }
}
