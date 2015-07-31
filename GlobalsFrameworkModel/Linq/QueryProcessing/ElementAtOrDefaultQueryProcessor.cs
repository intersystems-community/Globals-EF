using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Utils.InstanceCreation;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class ElementAtOrDefaultQueryProcessor : IQueryProcessor
    {
        public  bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "ElementAtOrDefault";
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            var index = (int)((ConstantExpression)query.Arguments[1]).Value;

            var enumerator = parentResult.GetItems().GetEnumerator();
            var elementType = QueryProcessingHelper.GetReturnParameterType(query);

            while (index >= 0)
            {
                if (!enumerator.MoveNext())
                    return new ProcessingResult(true, InstanceCreator.GetDefaultValue(elementType), true);

                index--;
            }

            return new ProcessingResult(true, enumerator.Current, true);
        }
    }
}
