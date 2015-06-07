using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS
{
    public class AVLNode<TKey, TValue> : IBSTNode<AVLNode<TKey, TValue>, TKey, TValue>
    {
        public AVLNode(TKey key, TValue value)
        {
            this.Key = key;
            this.Value = value;
        }

        public TKey Key { get; private set; }

        public TValue Value { get; set; }

        public AVLNode<TKey, TValue> Left { get; set; }

        public AVLNode<TKey, TValue> Right { get; set; }

        public int BalanceFactor { get; set; }
    }
}
