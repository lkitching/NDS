using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS
{
    public class Trie
    {
        private readonly Node root = new Node();

        public void Add(string str)
        {
            Require.NotNull(str, "str");

            int i = 0;
            Node current = this.root;
            Node next = null;

            while (i < str.Length)
            {
                next = current.GetChild(str[i]);
                if (next == null) break;

                current = next;
                ++i;
            }

            Debug.Assert(current != null);

            while (i < str.Length)
            {
                current = current.AddChild(str[i]);
                ++i;
            }
        }

        public bool Contains(string str)
        {
            Require.NotNull(str, "str");

            Node current = this.root;

            foreach(char c in str)
            {
                current = current.GetChild(c);
                if (current == null) return false;
            }

            return true;
        }

        public bool Remove(string str)
        {
            Require.NotNull(str, "str");
            if(str.Length == 0) return false;

            //find all nodes on the path containing str
            Node current = this.root;
            Node[] path = new Node[str.Length + 1];
            path[0] = current;

            for (int i = 0; i < str.Length; ++i)
            {
                current = current.GetChild(str[i]);
                if (current == null) return false;
                path[i + 1] = current;
            }

            //traverse back through the path and remove any nodes which have no children
            Debug.Assert(path.Length > 1);
            for (int i = path.Length - 2; i >= 0; i--)
            {
                Node parent = path[i];
                if (path[i + 1].ChildCount == 0)
                {
                    bool removed = parent.RemoveChild(str[i]);
                    Debug.Assert(removed, "Tried to remove non-existent child");
                }
                else break;
            }

            return true;
        }

        private class Node
        {
            private readonly Dictionary<char, Node> children = new Dictionary<char, Node>();

            public Node GetChild(char c)
            {
                return this.children.GetOrDefault(c);
            }

            public Node AddChild(char c)
            {
                Debug.Assert(!this.children.ContainsKey(c));

                var newNode = new Node();
                this.children.Add(c, newNode);

                return newNode;
            }

            public bool RemoveChild(char c)
            {
                return this.children.Remove(c);
            }

            public int ChildCount
            {
                get { return children.Count; }
            }
        }
    }
}
