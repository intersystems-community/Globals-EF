using System;
using System.Collections;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;

namespace GlobalsFramework.Linq.QueryProcessing.SingleResultQueries
{
    internal sealed class FirstQueryProcessor : SingleResultQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "First";
        }

        protected override ProcessingResult ProcessQuery(IEnumerator enumerator, Type elementType)
        {
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");

            return new ProcessingResult(true, enumerator.Current, true);
        }
    }
}
