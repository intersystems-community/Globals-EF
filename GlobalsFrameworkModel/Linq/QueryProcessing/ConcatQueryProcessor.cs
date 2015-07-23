using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class ConcatQueryProcessor:IQueryProcessor
    {
        public bool CanProcess(MethodCallExpression query)
        {
            return query.Method.Name == "Concat";
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            var source1 = parentResult.GetLoadedItems(QueryProcessingHelper.GetSourceParameterType(query));
            var constantArgument = query.Arguments[1] as ConstantExpression;

            if (constantArgument == null)
                return ProcessingResult.Unsuccessful;

            var source2 = (IEnumerable) constantArgument.Value;

            var resultElementType = QueryProcessingHelper.GetSourceParameterType(query);
            var resultList = (IList)Activator.CreateInstance(typeof (List<>).MakeGenericType(resultElementType));

            foreach (var item in source1)
                resultList.Add(item);

            foreach (var item in source2)
                resultList.Add(item);

            return new ProcessingResult(true, resultList);
        }
    }
}
