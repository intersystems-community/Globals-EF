using System.Collections;
using System.Linq.Expressions;

namespace GlobalsFramework.Linq.QueryProcessing.ConditionalLimitQueries
{
    internal sealed class SkipWhileQueryProcessor : ConditionalLimitQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "SkipWhile";
        }

        protected override void ConditionTrueCallback(IList resultItems, object item)
        {
            //do nothing - item is skipped
        }

        protected override void ConditionFalseCallback(IList resultItems, object item)
        {
            resultItems.Add(item);
        }
    }
}
