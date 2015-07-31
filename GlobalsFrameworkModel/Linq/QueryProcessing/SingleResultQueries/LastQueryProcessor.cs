using System;
using System.Collections;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;

namespace GlobalsFramework.Linq.QueryProcessing.SingleResultQueries
{
    internal sealed class LastQueryProcessor : SingleResultQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Last";
        }

        protected override ProcessingResult ProcessQuery(IEnumerator enumerator, Type elementType)
        {
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");

            var result = enumerator.Current;

            while (enumerator.MoveNext())
                result = enumerator.Current;

            return new ProcessingResult(true, result, true);
        }
    }
}
