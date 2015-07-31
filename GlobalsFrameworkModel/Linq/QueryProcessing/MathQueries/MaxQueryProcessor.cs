using System;
using System.Collections;
using System.Linq.Expressions;

namespace GlobalsFramework.Linq.QueryProcessing.MathQueries
{
    internal sealed class MaxQueryProcessor : MathQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Max";
        }

        protected override object ProcessQuery(IEnumerable items)
        {
            var enumerator = items.GetEnumerator();

            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");

            var max = (dynamic)enumerator.Current;

            while (enumerator.MoveNext())
            {
                var current = (dynamic)enumerator.Current;

                if (current > max)
                    max = current;
            }

            return max;
        }
    }
}
