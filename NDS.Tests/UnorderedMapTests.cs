using System.Collections.Generic;

namespace NDS.Tests
{
    public abstract class UnorderedMapTests : MapTests
    {
        protected override IMap<TKey, TValue> CreateMap<TKey, TValue>()
        {
            return CreateMap<TKey, TValue>(EqualityComparer<TKey>.Default);
        }

        protected abstract IMap<TKey, TValue> CreateMap<TKey, TValue>(IEqualityComparer<TKey> keyComparer);
    }
}
