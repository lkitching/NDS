using System.Collections.Generic;

namespace NDS.Tests
{
    public abstract class OrderedMapTests : MapTests
    {
        protected override IMap<TKey, TValue> CreateMap<TKey, TValue>()
        {
            return CreateMap<TKey, TValue>(Comparer<TKey>.Default);
        }

        protected abstract IMap<TKey, TValue> CreateMap<TKey, TValue>(IComparer<TKey> keyComparer);
    }
}
