using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using InterSystems.Globals;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class TakeQueryProcessor : IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Take";
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult, DataContext context)
        {
            var items = parentResult.GetItems();
            var enumerator = items.GetEnumerator();

            var count = (int)((ConstantExpression)query.Arguments[1]).Value;
            var currentIndex = 0;

            var itemType = parentResult.IsDeferred()
                ? typeof(NodeReference)
                : QueryProcessingHelper.GetSourceParameterType(query);

            var resultList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));

            while ((currentIndex < count) && enumerator.MoveNext())
            {
                resultList.Add(enumerator.Current);
                currentIndex++;
            }

            return new ProcessingResult(true, resultList);
        }
    }
}
