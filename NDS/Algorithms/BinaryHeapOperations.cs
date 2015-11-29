using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace NDS.Algorithms
{
    internal static class BinaryHeapOperations
    {
        /// <summary>
        /// Moves the item at the given index up into position inside a heap. At each stage, if
        /// the item is smaller than its parent according to <paramref name="comp"/> they will swap positions
        /// and item then compared with its grandparent, great-grandparent etc. until the root is reached.
        /// </summary>
        /// <typeparam name="T">The type of items in the heap.</typeparam>
        /// <param name="items"></param>
        /// <param name="nodeIndex">Index of the item to move up into position inside the heap.</param>
        /// <param name="rootIndex">Index of the root item in the heap inside <paramref name="items"/>.</param>
        /// <param name="comp">Comparer for the elements in the heap.</param>
        internal static void FixUp<T>(T[] items, int nodeIndex, int rootIndex, IComparer<T> comp)
        {
            Contract.Requires(nodeIndex >= rootIndex);
            Contract.Requires(nodeIndex < items.Length);

            int i = nodeIndex;
            while (i > rootIndex)
            {
                int parentIndex = rootIndex + ((i - rootIndex - 1) / 2);
                T parent = items[parentIndex];
                T current = items[i];

                if (comp.CompareResult(current, parent) == ComparisonResult.Less)
                {
                    items.SwapIndexed(parentIndex, i);
                    i = parentIndex;
                }
                else break;
            }
        }

        /// <summary>
        /// Moves an item down to its correct position in a heap in the range [rootIndex..endIndex]. At each stage if the
        /// current item is smaller than either of its children it will be swapped with the smaller child and compared
        /// with its grandchildren, great-grandchildren etc. until the end of the heap is reached.
        /// </summary>
        /// <typeparam name="T">The type of elements in the heap.</typeparam>
        /// <param name="items">The collection containing the heap.</param>
        /// <param name="nodeIndex">Index of the item to move down the heap.</param>
        /// <param name="rootIndex">Root index of the heap.</param>
        /// <param name="endIndex">End index of the heap.</param>
        /// <param name="comp">Comparer for the items in the heap.</param>
        internal static void FixDown<T>(T[] items, int nodeIndex, int rootIndex, int endIndex, IComparer<T> comp)
        {
            Contract.Requires(rootIndex >= 0);
            Contract.Requires(endIndex < items.Length);
            Contract.Requires(nodeIndex >= rootIndex);
            Contract.Requires(nodeIndex <= endIndex);

            int currentIndex = nodeIndex;
            while (true)
            {
                int leftChildIndex = rootIndex + ((currentIndex - rootIndex) * 2) + 1;
                if (leftChildIndex > endIndex) break;

                int rightChildIndex = leftChildIndex + 1;

                //find the index of the minimum child
                int minChildIndex = leftChildIndex;
                if (rightChildIndex <= endIndex)
                {
                    T leftChild = items[leftChildIndex];
                    T rightChild = items[rightChildIndex];
                    if (comp.CompareResult(rightChild, leftChild) == ComparisonResult.Less)
                    {
                        minChildIndex = rightChildIndex;
                    }
                }

                T current = items[currentIndex];
                T minChild = items[minChildIndex];

                if (comp.CompareResult(current, minChild) == ComparisonResult.Greater)
                {
                    items.SwapIndexed(currentIndex, minChildIndex);
                    currentIndex = minChildIndex;
                }
                else break;
            }
        }
    }
}
