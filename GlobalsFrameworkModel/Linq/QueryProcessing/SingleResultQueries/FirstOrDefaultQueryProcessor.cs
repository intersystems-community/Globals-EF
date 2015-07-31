using System;
using System.Collections;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Utils.InstanceCreation;

namespace GlobalsFramework.Linq.QueryProcessing.SingleResultQueries
{
    internal sealed class FirstOrDefaultQueryProcessor : SingleResultQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "FirstOrDefault";
        }

        protected override ProcessingResult ProcessQuery(IEnumerator enumerator, Type elementType)
        {
            if (!enumerator.MoveNext())
                return new ProcessingResult(true, InstanceCreator.GetDefaultValue(elementType), true);

            return new ProcessingResult(true, enumerator.Current, true);
        }
    }
}
