using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS
{
    public enum RBNodeColour { Red, Black }

    public class RedBlackNode<TKey, TValue> : IBSTNode<RedBlackNode<TKey, TValue>, TKey, TValue>, IHasParent<RedBlackNode<TKey, TValue>>
    {
        public RedBlackNode(TKey key, TValue value, RBNodeColour colour)
        {
            this.Key = key;
            this.Value = value;
            this.Colour = colour;
        }

        public TKey Key { get; internal set; }
        public TValue Value { get; set; }
        public RBNodeColour Colour { get; set; }
        public RedBlackNode<TKey, TValue> Left { get; set; }
        public RedBlackNode<TKey, TValue> Right { get; set; }
        public RedBlackNode<TKey, TValue> Parent { get; set; }

        public override string ToString()
        {
            return string.Format("Key = {0}, Value = {1}", Key, Value);
        }
    }
}
