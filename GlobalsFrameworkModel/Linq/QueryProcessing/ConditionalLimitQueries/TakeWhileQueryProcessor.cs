using System.Collections;
using System.Linq.Expressions;

namespace GlobalsFramework.Linq.QueryProcessing.ConditionalLimitQueries
{
    internal sealed class TakeWhileQueryProcessor : ConditionalLimitQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "TakeWhile";
        }

        protected override void ConditionTrueCallback(IList resultItems, object item)
        {
            resultItems.Add(item);
        }
    }
}
