using System;
using System.Collections.Generic;
using System.Linq;
using GlobalsFramework.Access;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.DeferredOrdering
{
    internal sealed class OrderedEnumerable<TSource, TKey> : OrderedEnumerable<TSource>
    {
        private readonly IComparer<TKey> _keyComparer;
        private readonly bool _descending;
        private readonly List<TKey> _keys;
        private readonly OrderedEnumerable<TSource> _parent;
        private readonly IEnumerable<TSource> _source;

        internal OrderedEnumerable(IEnumerable<TSource> source, List<TKey> keys, IComparer<TKey> comparer, bool descending)
        {
            _source = source;
            _keys = keys;
            _keyComparer = comparer ?? Comparer<TKey>.Default;
            _descending = descending;
        }

        internal OrderedEnumerable(IEnumerable<TSource> source, List<TKey> keys, IComparer<TKey> comparer, bool descending, OrderedEnumerable<TSource> parent) : 
            this(source, keys, comparer, descending)
        {
            _parent = parent;
        }

        public override IOrderedEnumerable<TSource> CreateOrderedEnumerable<TKey1>(Func<TSource, TKey1> keySelector, IComparer<TKey1> comparer, bool descending)
        {
            return CreateOrderedEnumerable(_source.Select(keySelector).ToList(), comparer, descending);
        }

        public override IEnumerator<TSource> GetEnumerator()
        {
            return Sort(_source.ToList()).GetEnumerator();
        }

        public override IOrderedEnumerable<TResult> GetLoadedOrderedEnumerable<TResult>()
        {
            var nodes = _source as IEnumerable<NodeReference>;
            var isDeferred = nodes != null;

            var loadedSource = isDeferred
                ? (List<TResult>) DatabaseManager.ReadNodes(nodes, typeof (TResult))
                : _source.Cast<TResult>().ToList();

            var parent = (_parent == null)
                ? null
                : (OrderedEnumerable<TResult>) _parent.GetLoadedOrderedEnumerable<TResult>();

            return new OrderedEnumerable<TResult, TKey>(loadedSource, _keys, _keyComparer, _descending, parent);
        }

        internal override OrderedEnumerable<TSource> CreateOrderedEnumerable<TKey1>(List<TKey1> keys, IComparer<TKey1> comparer, bool descending)
        {
            return new OrderedEnumerable<TSource, TKey1>(_source, keys, comparer, descending, this);
        }

        internal override IIndexedComparer GetIndexedComparer()
        {
            IIndexedComparer comparer = new IndexedComparer<TKey>(_keys, _keyComparer, _descending);
            if (_parent != null)
                comparer = _parent.GetIndexedComparer(comparer);
            return comparer;
        }

        internal override IIndexedComparer GetIndexedComparer(IIndexedComparer next)
        {
            IIndexedComparer comparer = new IndexedComparer<TKey>(_keys, _keyComparer, _descending, next);
            if (_parent != null)
                comparer = _parent.GetIndexedComparer(comparer);
            return comparer;
        }
    }
}
