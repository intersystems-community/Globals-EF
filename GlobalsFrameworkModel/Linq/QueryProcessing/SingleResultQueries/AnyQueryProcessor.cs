using System;
using System.Collections;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;

namespace GlobalsFramework.Linq.QueryProcessing.SingleResultQueries
{
    internal sealed class AnyQueryProcessor : SingleResultQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Any";
        }

        protected override ProcessingResult ProcessQuery(IEnumerator enumerator, Type elementType)
        {
            return new ProcessingResult(true, enumerator.MoveNext(), true);
        }
    }
}
