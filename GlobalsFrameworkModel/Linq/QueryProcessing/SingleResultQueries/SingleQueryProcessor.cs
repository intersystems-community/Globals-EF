using System;
using System.Collections;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;

namespace GlobalsFramework.Linq.QueryProcessing.SingleResultQueries
{
    internal sealed class SingleQueryProcessor : SingleResultQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Single";
        }

        protected override ProcessingResult ProcessQuery(IEnumerator enumerator, Type elementType)
        {
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");

            var result = enumerator.Current;

            if (enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains more than one element");

            return new ProcessingResult(true, result, true);
        }
    }
}
