using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GlobalsFramework.Linq.DeferredOrdering
{
    internal abstract class OrderedEnumerable<TSource> : IOrderedEnumerable<TSource>, IDeferredOrderedEnumerable
    {
        public abstract IOrderedEnumerable<TSource> CreateOrderedEnumerable<TKey>(Func<TSource, TKey> keySelector,
            IComparer<TKey> comparer, bool descending);

        public abstract IEnumerator<TSource> GetEnumerator();

        public abstract IOrderedEnumerable<TResult> GetLoadedOrderedEnumerable<TResult>();

        internal abstract OrderedEnumerable<TSource> CreateOrderedEnumerable<TKey>(List<TKey> keys, IComparer<TKey> comparer, bool descending);

        internal abstract IIndexedComparer GetIndexedComparer(IIndexedComparer next);

        internal abstract IIndexedComparer GetIndexedComparer();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected List<TSource> Sort(List<TSource> deferredItems)
        {
            var indexes = Enumerable.Range(0, deferredItems.Count).ToList();
            indexes.Sort(GetIndexedComparer());

            var sortedEnumerable = indexes.Select(index => deferredItems[index]).ToList();
            return sortedEnumerable;
        }
    }
}
