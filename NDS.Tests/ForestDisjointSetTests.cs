using System.Linq;

using NUnit.Framework;

namespace NDS.Tests
{
    [TestFixture]
    public class ForestDisjointSetTests : DisjointSetTests
    {
        protected override IDisjointSet<int> Create(ListDisjointSet sets)
        {
            var ret = new ForestDisjointSet<int>();

            foreach(var i in sets.Sets.SelectMany(s => s.AllItems))
            {
                ret.MakeSet(i);
            }

            foreach(var set in sets.Sets)
            {
                foreach(var i in set.OtherItems)
                {
                    ret.Merge(set.Representative, i);
                }
            }

            return ret;
        }

        [Test]
        public void Should_Make_Set()
        {
            var set = new ForestDisjointSet<int>();
            var items = TestGen.NRandomInts(1000, 2000).Distinct().ToArray();

            foreach (var item in items)
            {
                set.MakeSet(item);
            }

            foreach(var item in items)
            {
                TestAssert.IsSome(set.FindRepresentative(item), item);
            }
        }
    }
}
