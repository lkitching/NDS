using FsCheck;
using NDS.Algorithms.Searching;
using System;
using System.Linq;

using NUnit.Framework;

namespace NDS.Tests.Algorithms.Searching
{
    [TestFixture]
    public class QuickSelectTests
    {
        [Test]
        public void Should_Find_Kth_Smallest()
        {
            var gen = from len in Gen.Choose(1, 200)
                      from k in Gen.Choose(0, len - 1)
                      from arr in Gen.ArrayOf<int>(len, Arb.Default.Int32().Generator)
                      select Tuple.Create(arr, k);

            var p = Prop.ForAll(Arb.From(gen), t =>
            {
                var arr = t.Item1;
                int k = t.Item2;

                int kthSmallest = QuickSelect.KthSmallest(arr, k);
                Array.Sort(arr);
                return kthSmallest == arr[k];
            });

            Check.QuickThrowOnFailure(p);
        }
    }
}
