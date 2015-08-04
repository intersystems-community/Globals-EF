using System;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class ElementAtQueryProcessor : IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "ElementAt";
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult, DataContext context)
        {
            var index = (int) ((ConstantExpression) query.Arguments[1]).Value;
            var enumerator = parentResult.GetItems().GetEnumerator();

            while (index >= 0)
            {
                if (!enumerator.MoveNext())
                    //ReSharper disable once NotResolvedInText
                    throw new ArgumentOutOfRangeException("index", "Index was out of range. Must be non-negative and less than the size of the collection.");
                index--;
            }

            return new ProcessingResult(true, enumerator.Current, true);
        }
    }
}
