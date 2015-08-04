using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Utils.InstanceCreation;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class DefaultIfEmptyProcessor : IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "DefaultIfEmpty";
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult, DataContext context)
        {
            var items = parentResult.GetItems();
            var enumerator = items.GetEnumerator();

            if (enumerator.MoveNext())
                return new ProcessingResult(true, parentResult.Result);

            var elementType = QueryProcessingHelper.GetSourceParameterType(query);
            var hasDefaultValue = query.Arguments.Count > 1;

            var defaultValue = hasDefaultValue
                ? ((ConstantExpression) query.Arguments[1]).Value
                : InstanceCreator.GetDefaultValue(elementType);

            var resultList = (IList)Activator.CreateInstance(typeof (List<>).MakeGenericType(elementType));
            resultList.Add(defaultValue);

            return new ProcessingResult(true, resultList);
        }
    }
}
