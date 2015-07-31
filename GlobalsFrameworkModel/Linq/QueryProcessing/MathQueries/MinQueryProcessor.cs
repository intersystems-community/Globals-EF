using System;
using System.Collections;
using System.Linq.Expressions;

namespace GlobalsFramework.Linq.QueryProcessing.MathQueries
{
    internal sealed class MinQueryProcessor : MathQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Min";
        }

        protected override object ProcessQuery(IEnumerable items)
        {
            var enumerator = items.GetEnumerator();

            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");

            var min = (dynamic)enumerator.Current;

            while (enumerator.MoveNext())
            {
                var current = (dynamic)enumerator.Current;

                if (current < min)
                    min = current;
            }

            return min;
        }
    }
}
