using System.Collections.Generic;

namespace GlobalsFramework.Linq.DeferredOrdering
{
    internal sealed class IndexedComparer<TKey> : IIndexedComparer
    {
        private readonly List<TKey> _keys;
        private readonly IComparer<TKey> _keyComparer;
        private readonly IComparer<int> _nextComparer;
        private readonly bool _descending;

        public int Compare(int index1, int index2)
        {
            var comparisonResult = _keyComparer.Compare(_keys[index1], _keys[index2]);

            if (comparisonResult == 0)
                return (_nextComparer == null) ? comparisonResult : _nextComparer.Compare(index1, index2);

            return (!_descending) ? comparisonResult : -comparisonResult;
        }

        internal IndexedComparer(List<TKey> keys, IComparer<TKey> keyComparer, bool descending)
        {
            _keys = keys;
            _keyComparer = keyComparer;
            _descending = descending;
        }

        internal IndexedComparer(List<TKey> keys, IComparer<TKey> keyComparer, bool descending, IComparer<int> nextComparer) : 
            this(keys, keyComparer, descending)
        {
            _nextComparer = nextComparer;
        }
    }
}
