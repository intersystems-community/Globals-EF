using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class CountQueryProcessor : IQueryProcessor
    {
        private static readonly Dictionary<string, Func<IEnumerator, Type, ProcessingResult>> OptimizedHandlers =
            new Dictionary<string, Func<IEnumerator, Type, ProcessingResult>>
            {
                {"Count", ProcessCount},
                {"LongCount", ProcessLongCount},
                {"Any", ProcessAny}
            };
 
        public bool CanProcess(MethodCallExpression query)
        {
            return OptimizedHandlers.ContainsKey(query.Method.Name);
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            return QueryProcessingHelper.ProcessSingleResultQuery(query, parentResult, OptimizedHandlers[query.Method.Name]);
        }

        private static ProcessingResult ProcessCount(IEnumerator enumerator, Type elementType)
        {
            var count = 0;

            while (enumerator.MoveNext())
                checked { count++; }

            return new ProcessingResult(true, count, true);
        }

        private static ProcessingResult ProcessLongCount(IEnumerator enumerator, Type elementType)
        {
            long longCount = 0;

            while (enumerator.MoveNext())
                checked { longCount++; }

            return new ProcessingResult(true, longCount, true);
        }

        private static ProcessingResult ProcessAny(IEnumerator enumerator, Type elementType)
        {
            return new ProcessingResult(true, enumerator.MoveNext(), true);
        }
    }
}
