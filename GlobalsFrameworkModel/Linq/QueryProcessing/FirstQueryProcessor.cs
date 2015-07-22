using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using GlobalsFramework.Linq.ExpressionProcessing;
using GlobalsFramework.Linq.Helpers;
using GlobalsFramework.Utils.InstanceCreation;

namespace GlobalsFramework.Linq.QueryProcessing
{
    internal sealed class FirstQueryProcessor : IQueryProcessor
    {
        private static readonly Dictionary<string, Func<IEnumerator, Type, ProcessingResult>> OptimizedHandlers =
            new Dictionary<string, Func<IEnumerator, Type, ProcessingResult>>
            {
                {"First", ProcessFirst},
                {"FirstOrDefault", ProcessFirstOrDefault},
            };

        public bool CanProcess(MethodCallExpression query)
        {
            return OptimizedHandlers.ContainsKey(query.Method.Name);
        }

        public ProcessingResult ProcessQuery(MethodCallExpression query, ProcessingResult parentResult)
        {
            return QueryProcessingHelper.ResolvePredicate(query, parentResult, OptimizedHandlers[query.Method.Name]);
        }

        private static ProcessingResult ProcessFirst(IEnumerator enumerator, Type elementType)
        {
            if (!enumerator.MoveNext())
                throw new InvalidOperationException("Sequence contains no elements");

            return new ProcessingResult(true, enumerator.Current, true);
        }

        private static ProcessingResult ProcessFirstOrDefault(IEnumerator enumerator, Type elementType)
        {
            if (!enumerator.MoveNext())
            {
                var result = elementType.IsClass ? null : InstanceCreator.CreateInstance(elementType);
                return new ProcessingResult(true, result, true);
            }

            return new ProcessingResult(true, enumerator.Current, true);
        }
    }
}
