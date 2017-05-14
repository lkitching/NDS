using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Tests
{
    [TestFixture]
    public class IntDisjointSetTests : DisjointSetTests
    {
        protected override IDisjointSet<int> Create(ListDisjointSet sets)
        {
            var ret = new IntDisjointSet(sets.Count);
            foreach(var s in sets.Sets)
            {
                foreach(int i in s.OtherItems)
                {
                    ret.Merge(s.Representative, i);
                }
            }

            return ret;
        }
    }
}
