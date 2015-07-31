using System.Collections;
using System.Linq.Expressions;

namespace GlobalsFramework.Linq.QueryProcessing.MathQueries
{
    internal sealed class SumQueryProcessor : MathQueryProcessor
    {
        public override bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Sum";
        }

        protected override object ProcessQuery(IEnumerable items)
        {
            dynamic sum = 0;

            foreach (dynamic item in items)
            {
                sum += item;
            }

            return sum;
        }
    }
}
