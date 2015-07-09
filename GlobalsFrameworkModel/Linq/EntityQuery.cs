using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace GlobalsFramework.Linq
{
    internal class EntityQuery<TElement> : IOrderedQueryable<TElement>
    {
        internal EntityQuery(IQueryProvider provider, Expression expression)
        {
            Provider = provider;
            Expression = expression;
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return (Provider.Execute<IEnumerable<TElement>>(Expression)).GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (Provider.Execute<IEnumerable>(Expression)).GetEnumerator();
        }

        public Expression Expression { get; private set; }
        public Type ElementType
        {
            get { return typeof (TElement); }
        }
        public IQueryProvider Provider { get; private set; }
    }
}
