using System;
using System.Collections;
using System.Linq.Expressions;

namespace GlobalsFramework.Linq.QueryProcessing.MathQueries
{
    internal sealed class AverageQueryProcessor : MathQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Average";
        }

        protected override object ProcessQuery(IEnumerable items)
        {
            dynamic sum = 0;
            var count = 0;

            foreach (dynamic item in items)
            {
                sum += item;
                count++;
            }

            if (count == 0)
                throw new InvalidOperationException("Sequence contains no elements");

            return (sum * 1.0) / count;
        }
    }
}
